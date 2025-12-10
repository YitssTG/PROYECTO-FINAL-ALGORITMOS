using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public enum AbilityType
{
    None,
    PrimaryAb,    
    SecondaryAb,  
    ThirdAb,      
    Ultimate     
}

[System.Serializable]
public class AbilityState
{
    public bool unlocked = false;
    public int level = 0;
    public float lastCastTime = -999f;
}

public class AbilitySystem : MonoBehaviour
{
    [Header("Referencia a la base de datos de habilidades")]
    public AbilityDatabase abilityDatabase;

    public Dictionary<AbilityType, Ability> abilities = new();
    public Dictionary<AbilityType, AbilityState> state = new();

    public System.Action<AbilityType> OnAbilityCast;

    private PlayerStats playerStats;
    private bool isInitialized = false;

    void Start()
    {
        TryInitialize();
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        TryInitialize();
    }

    private void TryInitialize()
    {
        if (GameManager.Instance == null)
            return;

        if (GameManager.Instance.playerStats == null)
            return;

        if (isInitialized)
            return;

        Initialize();
    }

    public void Initialize()
    {
        playerStats = GameManager.Instance?.playerStats;

        if (playerStats == null)
        {
            Debug.LogWarning("[AbilitySystem] PlayerStats aún no listo, esperando escena...");
            return;
        }

        abilities.Clear();
        state.Clear();

        abilities[AbilityType.PrimaryAb] = abilityDatabase.GetByType(AbilityType.PrimaryAb);
        abilities[AbilityType.SecondaryAb] = abilityDatabase.GetByType(AbilityType.SecondaryAb);
        abilities[AbilityType.ThirdAb] = abilityDatabase.GetByType(AbilityType.ThirdAb);
        abilities[AbilityType.Ultimate] = abilityDatabase.GetByType(AbilityType.Ultimate);

        foreach (KeyValuePair<AbilityType, Ability> kvp in abilities)
        {
            AbilityType type = kvp.Key;
            Ability ab = kvp.Value;

            if (ab == null) continue;

            AbilityState st = new AbilityState();
            st.unlocked = !ab.locked;
            st.level = ab.locked ? 0 : 1;
            state[type] = st;
        }

        isInitialized = true;
        Debug.Log("AbilitySystem inicializado correctamente");
    }

    public bool CanCast(AbilityType type)
    {
        if (!abilities.ContainsKey(type) || !state.ContainsKey(type))
            return false;

        Ability ab = abilities[type];
        AbilityState st = state[type];

        return st.unlocked && st.level > 0 && Time.time >= st.lastCastTime + ab.cooldown;
    }

    public float GetCooldownRemaining(AbilityType type)
    {
        if (!state.ContainsKey(type)) return 0f;

        AbilityState st = state[type];
        Ability ab = abilities[type];

        float endTime = st.lastCastTime + ab.cooldown;
        return Mathf.Max(0f, endTime - Time.time);
    }

    public int GetCalculatedDamage(AbilityType type)
    {
        if (!abilities.ContainsKey(type) || !state.ContainsKey(type))
            return 0;

        Ability ab = abilities[type];
        AbilityState st = state[type];

        return Mathf.RoundToInt(ab.damageBase + ab.damagePerLevel * st.level);
    }

    public bool IsAbilityReady(AbilityType type)
    {
        return CanCast(type);
    }

    public Ability GetAbility(AbilityType type)
    {
        return abilities.ContainsKey(type) ? abilities[type] : null;
    }

    public bool CanUpgradeAbility(AbilityType type, int playerLevel, int availableSkillPoints)
    {
        if (!abilities.ContainsKey(type)) return false;

        Ability ab = abilities[type];
        AbilityState st = state[type];

        if (availableSkillPoints <= 0) return false;
        if (st.level >= ab.maxLevel) return false;
        if (type == AbilityType.Ultimate && playerLevel < 5) return false;

        return true;
    }

    public bool TryCast(AbilityType type)
    {
        if (!abilities.ContainsKey(type))
            return false;

        Ability ab = abilities[type];
        AbilityState st = state[type];

        if (!st.unlocked || st.level <= 0)
            return false;

        if (!CanCast(type))
            return false;

        st.lastCastTime = Time.time;
        OnAbilityCast?.Invoke(type);

        Debug.Log($"[{ab.abilityName}] lanzada (Nivel {st.level})");
        return true;
    }

    public bool TryUpgradeAbility(AbilityType type, int playerLevel)
    {
        if (!abilities.ContainsKey(type)) return false;

        Ability ab = abilities[type];
        AbilityState st = state[type];

        if (type == AbilityType.Ultimate && playerLevel < 5)
        {
            Debug.Log("[Mejora] Ultimate bloqueada hasta nivel 5 del jugador.");
            return false;
        }

        if (!st.unlocked)
        {
            st.unlocked = true;
            st.level = 1;
            Debug.Log($"[{ab.abilityName}] Desbloqueada (nivel 1)");
            return true;
        }

        if (st.level >= ab.maxLevel)
        {
            Debug.Log($"[{ab.abilityName}] nivel máximo alcanzado.");
            return false;
        }

        st.level++;
        Debug.Log($"[{ab.abilityName}] mejorada a nivel {st.level}");
        return true;
    }
}
