using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    public static AbilityManager Instance;

    [Header("Configuración")]
    public AbilityDatabase abilityDatabase;

    [Header("Referencias")]
    public AbilitySystem abilitySystem;
    public AbilityAimingSystem aimingSystem;
    public AbilityEffectsController effectsController;
    public AbilityInputHandler inputHandler;

    private bool isInitialized = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Debug.Log("AbilityManager Instance creada");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void InitializeAbilitySystems(GameObject player)
    {
        if (isInitialized)
        {
            Debug.Log("AbilityManager ya estaba inicializado");
            return;
        }

        if (player == null)
        {
            Debug.LogError("Player no asignado para inicializar AbilityManager");
            return;
        }

        Debug.Log("Inicializando sistemas de habilidades...");

        abilitySystem = player.GetComponent<AbilitySystem>();
        aimingSystem = player.GetComponent<AbilityAimingSystem>();
        effectsController = player.GetComponent<AbilityEffectsController>();
        inputHandler = player.GetComponent<AbilityInputHandler>();

        if (abilitySystem == null)
        {
            Debug.LogError("AbilitySystem no encontrado en el player");
            return;
        }

        if (abilityDatabase != null)
        {
            abilitySystem.abilityDatabase = abilityDatabase;
        }
        else
        {
            Debug.LogError("AbilityDatabase no asignado en AbilityManager");
        }

        abilitySystem.Initialize();

        if (aimingSystem != null)
        {
            aimingSystem.abilitySystem = abilitySystem;
            Debug.Log("AbilityAimingSystem conectado");
        }

        if (effectsController != null)
        {
            effectsController.abilitySystem = abilitySystem;
            Debug.Log("AbilityEffectsController conectado");
        }

        if (inputHandler != null)
        {
            inputHandler.abilitySystem = abilitySystem;
            inputHandler.aimingSystem = aimingSystem;
            Debug.Log("AbilityInputHandler conectado");
        }

        isInitialized = true;
        Debug.Log("AbilityManager completamente inicializado");
    }

    public Ability GetAbility(AbilityType type)
    {
        if (abilitySystem == null)
        {
            Debug.LogWarning("AbilitySystem no disponible en GetAbility");
            return null;
        }

        Ability ability = abilitySystem.GetAbility(type);
        if (ability == null)
        {
            Debug.LogWarning($"Habilidad {type} no encontrada");
        }
        return ability;
    }

    public int GetAbilityLevel(AbilityType type)
    {
        if (abilitySystem == null || !abilitySystem.state.ContainsKey(type))
        {
            Debug.LogWarning($"No se pudo obtener nivel de {type}");
            return 0;
        }
        return abilitySystem.state[type].level;
    }

    public bool CanUpgradeAbility(AbilityType type)
    {
        if (abilitySystem == null || GameManager.Instance == null)
        {
            Debug.LogWarning("AbilitySystem o GameManager no disponible en CanUpgradeAbility");
            return false;
        }

        PlayerStats playerStats = GameManager.Instance.playerStats;
        if (playerStats == null)
        {
            Debug.LogWarning("PlayerStats no disponible");
            return false;
        }

        bool canUpgrade = abilitySystem.CanUpgradeAbility(type, playerStats.playerLevel, playerStats.skillPoints);
        Debug.Log($"CanUpgrade {type}: {canUpgrade} (Level: {playerStats.playerLevel}, SkillPoints: {playerStats.skillPoints})");

        return canUpgrade;
    }

    public void UpgradeAbility(AbilityType type)
    {
        Debug.Log($"UpgradeAbility llamado para {type}");

        if (GameManager.Instance != null && GameManager.Instance.playerStats != null)
        {
            bool upgraded = GameManager.Instance.playerStats.SpendSkillPoint(type);
            if (upgraded)
            {
                Debug.Log($"AbilityManager: {type} mejorada exitosamente");
            }
            else
            {
                Debug.Log($"AbilityManager: No se pudo mejorar {type}");
            }
        }
        else
        {
            Debug.LogError("GameManager o PlayerStats no disponible para upgrade");
        }
    }

    public bool IsAbilityReady(AbilityType type)
    {
        return abilitySystem != null && abilitySystem.IsAbilityReady(type);
    }

    public float GetCooldownRemaining(AbilityType type)
    {
        if (abilitySystem == null)
        {
            Debug.LogWarning("AbilitySystem no disponible en GetCooldownRemaining");
            return 0f;
        }
        return abilitySystem.GetCooldownRemaining(type);
    }

    public bool IsReady()
    {
        return isInitialized && abilitySystem != null;
    }
}