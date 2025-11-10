using UnityEngine;
using UnityEngine.Events;

public class PlayerStats : MonoBehaviour
{
    [Header("XP y Nivel")]
    public int playerLevel = 1;
    public int experience = 0;
    public int experienceToNext = 100;
    public int skillPoints = 0;

    [Header("Estadísticas base")]
    public float damage = 10f;
    public float speed = 5f;
    public float armor = 0f;

    // Salud del jugador
    public int maxHealth = 100;
    public int currentHealth;

    // Eventos para actualizar la UI
    public UnityEvent<int> OnHealthChanged = new();
    public UnityEvent<int> OnExperienceChanged = new();

    private GameManager gm;

    void Start()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager.Instance no está inicializado en PlayerStats.");
            enabled = false;
            return;
        }

        gm = GameManager.Instance;

        // Inicializar la salud del jugador
        currentHealth = maxHealth;
        OnHealthChanged.Invoke(currentHealth);  // Notificar a la UI el valor inicial de la vida

        var agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent != null)
        {
            agent.speed = speed;
            Debug.Log($"[INIT SPEED SET] NavMeshAgent speed seteado a {speed} desde PlayerStats");
        }
    }

    // Métodos para añadir experiencia y subir de nivel
    public void AddExperience(int amount)
    {
        experience += amount;
        OnExperienceChanged.Invoke(experience);  // Notificar a la UI el cambio en experiencia

        if (experience >= experienceToNext)
        {
            LevelUp();
        }
    }

    // Subir de nivel
    private void LevelUp()
    {
        experience -= experienceToNext;
        playerLevel++;
        experienceToNext = Mathf.RoundToInt(experienceToNext * 1.5f);
        skillPoints++;  // El jugador gana un punto de habilidad por nivel
    }

    // Añadir oro
    public void AddGold(int amount)
    {
        GoldManager.Instance.AddGold(amount);  // Usamos GoldManager para añadir oro
    }

    // Gastar oro
    public bool SpendGold(int cost)
    {
        return GoldManager.Instance.SpendGold(cost);  // Usamos GoldManager para gastar oro
    }

    // Método para recibir daño
    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth < 0) currentHealth = 0;

        // Notificar a la UI del cambio en la salud
        OnHealthChanged.Invoke(currentHealth);
        Debug.Log($"Player recibió {amount} de daño. Vida actual: {currentHealth}");
        if (currentHealth <= 0)
        {
            Debug.Log("PLAYER MURIÓ");
            Destroy(gameObject);
        }
    }

    // Método para curarse
    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;

        // Notificar a la UI del cambio en la salud
        OnHealthChanged.Invoke(currentHealth);
    }

    // Método para gastar un punto de habilidad
    public bool SpendSkillPoint(AbilityType abilityKey)
    {
        if (skillPoints > 0)
        {
            // Asegurarse de que AbilitySystem esté listo y que se pueda mejorar la habilidad
            if (gm != null && gm.abilitySystem != null)
            {
                bool upgraded = gm.abilitySystem.TryUpgradeAbility(abilityKey, playerLevel);
                if (upgraded)
                {
                    skillPoints--;  // Se gasta un punto de habilidad
                    Debug.Log($"Habilidad {abilityKey} mejorada. Puntos restantes: {skillPoints}");
                    return true;
                }
                else
                {
                    Debug.Log($"No se pudo mejorar la habilidad {abilityKey}.");
                    return false;
                }
            }
        }
        else
        {
            Debug.Log("No tienes puntos de habilidad disponibles.");
        }

        return false;
    }
    public void ApplyItemStats(ItemData item)
    {
        Debug.Log($"[DEBUG ITEM] Data en SO → Damage={item.bonusDamage} Armor={item.bonusArmor} Speed={item.bonusSpeed}");

        damage += item.bonusDamage;
        armor += item.bonusArmor;
        speed += item.bonusSpeed;

        var agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent != null)
            agent.speed = speed;

        Debug.Log($"[MEJORA ITEM] + {item.itemName} aplicado → Damage +{item.bonusDamage} | Armor +{item.bonusArmor} | Speed +{item.bonusSpeed}");
        Debug.Log($"[STATS ACTUALES] Damage={damage} | Armor={armor} | Speed={speed}");
    }

    public void RemoveItemStats(ItemData item)
    {
        damage -= item.bonusDamage;
        armor -= item.bonusArmor;
        speed -= item.bonusSpeed;

        var agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent != null)
            agent.speed = speed;

        Debug.Log($"[UNDO ITEM] - {item.itemName} revertido → Damage -{item.bonusDamage} | Armor -{item.bonusArmor} | Speed -{item.bonusSpeed}");
        Debug.Log($"[STATS ACTUALES] Damage={damage} | Armor={armor} | Speed={speed}");
    }
}
