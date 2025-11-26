using UnityEngine;
using UnityEngine.Events;

public class PlayerStats : MonoBehaviour
{
    [Header("XP y Nivel")]
    public int playerLevel = 1;
    public int experience = 0;
    public int experienceToNext = 100;
    public int skillPoints = 0;

    [Header("Estadísticas Base")]
    public float baseDamage = 10f;
    public float baseSpeed = 5f;
    public float baseArmor = 0f;

    [Header("Salud")]
    public int maxHealth = 100;
    public int currentHealth;
    public bool isInvulnerable = false;

    // Estadísticas actuales (base + items)
    public float CurrentDamage { get; private set; }
    public float CurrentSpeed { get; private set; }
    public float CurrentArmor { get; private set; }

    // ✅ AGREGADO: Propiedad pública temporal para compatibilidad
    public float damage => CurrentDamage;

    // Eventos para UI
    public UnityEvent<int> OnHealthChanged = new();
    public UnityEvent<int> OnExperienceChanged = new();
    public UnityEvent<int> OnLevelChanged = new();

    private void Start()
    {
        InitializeStats();
        SetupNavMeshAgent();
    }

    #region Inicialización
    private void InitializeStats()
    {
        // Inicializar estadísticas
        CurrentDamage = baseDamage;
        CurrentSpeed = baseSpeed;
        CurrentArmor = baseArmor;

        // Inicializar salud
        currentHealth = maxHealth;
        OnHealthChanged.Invoke(currentHealth);

        Debug.Log($"🎯 PlayerStats inicializado - Nivel {playerLevel}, Vida {currentHealth}/{maxHealth}");
    }

    private void SetupNavMeshAgent()
    {
        var agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent != null)
        {
            agent.speed = CurrentSpeed;
            Debug.Log($"[NAVMESH] Velocidad establecida a {CurrentSpeed}");
        }
    }
    #endregion

    #region Sistema de Experiencia y Nivel
    public void AddExperience(int amount)
    {
        if (amount <= 0) return;

        experience += amount;
        OnExperienceChanged.Invoke(experience);
        Debug.Log($"📚 +{amount} XP. Total: {experience}/{experienceToNext}");

        CheckLevelUp();
    }

    private void CheckLevelUp()
    {
        while (experience >= experienceToNext)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        experience -= experienceToNext;
        playerLevel++;
        skillPoints++;

        // Aumentar experiencia requerida para siguiente nivel
        experienceToNext = Mathf.RoundToInt(experienceToNext * 1.5f);

        OnLevelChanged.Invoke(playerLevel);
        Debug.Log($"🎉 ¡Nivel {playerLevel} alcanzado! Puntos de habilidad: {skillPoints}");
    }
    #endregion

    #region Sistema de Salud
    public void TakeDamage(int amount)
    {
        if (isInvulnerable || amount <= 0) return;

        // Aplicar reducción de daño por armadura
        int finalDamage = Mathf.Max(1, amount - Mathf.RoundToInt(CurrentArmor));

        currentHealth -= finalDamage;
        currentHealth = Mathf.Max(0, currentHealth);

        OnHealthChanged.Invoke(currentHealth);
        Debug.Log($"💔 {finalDamage} daño recibido. Vida: {currentHealth}/{maxHealth}");

        CheckDeath();
    }

    public void Heal(int amount)
    {
        if (amount <= 0) return;

        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);

        OnHealthChanged.Invoke(currentHealth);
        Debug.Log($"❤️ Curado {amount}. Vida: {currentHealth}/{maxHealth}");
    }

    private void CheckDeath()
    {
        if (currentHealth <= 0)
        {
            Debug.Log("💀 PLAYER MURIÓ");
            // Aquí podrías llamar a GameManager para game over
            Destroy(gameObject);
        }
    }
    #endregion

    #region Sistema de Items y Mejoras
    public void ApplyItemStats(ItemData item)
    {
        if (item == null) return;

        CurrentDamage += item.bonusDamage;
        CurrentArmor += item.bonusArmor;
        CurrentSpeed += item.bonusSpeed;

        UpdateNavMeshSpeed();

        Debug.Log($"[ITEM] {item.itemName} aplicado → " +
                 $"Daño: +{item.bonusDamage}, Armadura: +{item.bonusArmor}, Velocidad: +{item.bonusSpeed}");
        Debug.Log($"[STATS] Actuales → Daño: {CurrentDamage}, Armadura: {CurrentArmor}, Velocidad: {CurrentSpeed}");
    }

    public void RemoveItemStats(ItemData item)
    {
        if (item == null) return;

        CurrentDamage -= item.bonusDamage;
        CurrentArmor -= item.bonusArmor;
        CurrentSpeed -= item.bonusSpeed;

        UpdateNavMeshSpeed();

        Debug.Log($"[ITEM] {item.itemName} removido → " +
                 $"Daño: -{item.bonusDamage}, Armadura: -{item.bonusArmor}, Velocidad: -{item.bonusSpeed}");
    }

    // ✅ AGREGADO: Métodos para aumentar stats individualmente
    public void IncreaseDamage(float amount)
    {
        CurrentDamage += amount;
        Debug.Log($"⚔️ Daño aumentado: +{amount}. Total: {CurrentDamage}");
    }

    public void IncreaseArmor(float amount)
    {
        CurrentArmor += amount;
        Debug.Log($"🛡️ Armadura aumentada: +{amount}. Total: {CurrentArmor}");
    }

    public void IncreaseSpeed(float amount)
    {
        CurrentSpeed += amount;
        UpdateNavMeshSpeed();
        Debug.Log($"🏃 Velocidad aumentada: +{amount}. Total: {CurrentSpeed}");
    }

    private void UpdateNavMeshSpeed()
    {
        var agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent != null)
        {
            agent.speed = CurrentSpeed;
        }
    }
    #endregion

    #region Sistema de Habilidades
    public bool SpendSkillPoint(AbilityType abilityKey)
    {
        if (skillPoints <= 0)
        {
            Debug.Log("❌ No hay puntos de habilidad disponibles");
            return false;
        }

        if (GameManager.Instance == null || GameManager.Instance.abilitySystem == null)
        {
            Debug.LogError("❌ AbilitySystem no disponible");
            return false;
        }

        bool upgraded = GameManager.Instance.abilitySystem.TryUpgradeAbility(abilityKey, playerLevel);
        if (upgraded)
        {
            skillPoints--;
            Debug.Log($"🔧 Habilidad {abilityKey} mejorada. Puntos restantes: {skillPoints}");
            return true;
        }

        Debug.Log($"⚠️ No se pudo mejorar la habilidad {abilityKey}");
        return false;
    }
    #endregion

    #region Utilidades
    public void AddGold(int amount)
    {
        if (GoldManager.Instance != null)
            GoldManager.Instance.AddGold(amount);
    }

    public bool SpendGold(int cost)
    {
        return GoldManager.Instance != null && GoldManager.Instance.SpendGold(cost);
    }

    public float GetHealthPercentage() => (float)currentHealth / maxHealth;
    #endregion
}