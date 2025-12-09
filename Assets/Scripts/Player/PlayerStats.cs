using UnityEngine;
using UnityEngine.Events;

public class PlayerStats : MonoBehaviour
{
    [Header("XP y Nivel")]
    public int playerLevel = 1;
    public int experience = 0;
    public int experienceToNext = 100;
    public int skillPoints = 0;

    [Header("Estadísticas de Combate")]
    public float baseDamage = 10f;
    public float baseSpeed = 5f;
    public float baseArmor = 0f;

    public float CurrentDamage { get; private set; }
    public float CurrentSpeed { get; private set; }
    public float CurrentArmor { get; private set; }

    private PlayerHealth _playerHealth;
    private PlayerHealth PlayerHealth
    {
        get
        {
            if (_playerHealth == null)
                _playerHealth = GetComponent<PlayerHealth>();
            return _playerHealth;
        }
    }

    public UnityEvent<int> OnExperienceChanged = new();
    public UnityEvent<int> OnLevelChanged = new();
    public UnityEvent OnStatsChanged = new();

    void Start()
    {
        InitializeStats();
    }

    #region Inicialización
    private void InitializeStats()
    {
        CurrentDamage = baseDamage;
        CurrentSpeed = baseSpeed;
        CurrentArmor = baseArmor;

        UpdateMovementSpeed();
        Debug.Log($"PlayerStats inicializado - Nivel {playerLevel}");
    }

    private void UpdateMovementSpeed()
    {
        var agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent != null)
        {
            agent.speed = CurrentSpeed;
        }
    }
    #endregion

    #region ⭐ MÉTODOS DE REDIRECCIÓN PARA SISTEMAS EXISTENTES

    public bool isInvulnerable
    {
        get
        {
            Debug.LogWarning("Usa playerHealth.isInvulnerable en lugar de playerStats.isInvulnerable");
            return PlayerHealth != null && PlayerHealth.isInvulnerable;
        }
        set
        {
            Debug.LogWarning("Usa playerHealth.isInvulnerable en lugar de playerStats.isInvulnerable");
            if (PlayerHealth != null)
                PlayerHealth.isInvulnerable = value;
        }
    }

    public void TakeDamage(int amount)
    {
        Debug.LogWarning("Usa playerHealth.TakeDamage() en lugar de playerStats.TakeDamage()");
        if (PlayerHealth != null)
        {
            PlayerHealth.TakeDamage(amount);
        }
        else
        {
            Debug.LogError("PlayerHealth no encontrado");
        }
    }

    public void Heal(int amount)
    {
        Debug.LogWarning("Usa playerHealth.Heal() en lugar de playerStats.Heal()");
        if (PlayerHealth != null)
        {
            PlayerHealth.Heal(amount);
        }
    }

    public int currentHealth
    {
        get
        {
            Debug.LogWarning("Usa playerHealth.currentHealth en lugar de playerStats.currentHealth");
            return PlayerHealth != null ? PlayerHealth.currentHealth : 0;
        }
        set
        {
            Debug.LogWarning("Usa playerHealth.currentHealth en lugar de playerStats.currentHealth");
            if (PlayerHealth != null)
                PlayerHealth.currentHealth = value;
        }
    }

    public int maxHealth
    {
        get
        {
            Debug.LogWarning("Usa playerHealth.maxHealth en lugar de playerStats.maxHealth");
            return PlayerHealth != null ? PlayerHealth.maxHealth : 100;
        }
        set
        {
            Debug.LogWarning("Usa playerHealth.maxHealth en lugar de playerStats.maxHealth");
            if (PlayerHealth != null)
                PlayerHealth.maxHealth = value;
        }
    }

    public bool isDead
    {
        get
        {
            Debug.LogWarning("Usa playerHealth.isDead en lugar de playerStats.isDead");
            return PlayerHealth != null && PlayerHealth.isDead;
        }
        set
        {
            Debug.LogWarning("Usa playerHealth.isDead en lugar de playerStats.isDead");
            if (PlayerHealth != null)
                PlayerHealth.isDead = value;
        }
    }
    #endregion

    #region Sistema de Experiencia y Nivel
    public void AddExperience(int amount)
    {
        if (amount <= 0 || (PlayerHealth != null && PlayerHealth.isDead)) return;

        experience += amount;
        OnExperienceChanged?.Invoke(experience);
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
        experienceToNext = Mathf.RoundToInt(experienceToNext * 1.5f);

        OnLevelChanged?.Invoke(playerLevel);
        EventManager.PlayerLevelUp(playerLevel);

        Debug.Log($"¡Nivel {playerLevel} alcanzado! Puntos restantes: {skillPoints}");
    }
    #endregion

    #region Sistema de Habilidades
    public bool SpendSkillPoint(AbilityType abilityKey)
    {
        if (skillPoints <= 0 || (PlayerHealth != null && PlayerHealth.isDead))
        {
            Debug.Log("No hay puntos de habilidad disponibles");
            return false;
        }

        AbilitySystem abilitySystem = GetComponent<AbilitySystem>();
        if (abilitySystem == null)
        {
            Debug.LogError("AbilitySystem no encontrado en el player");
            return false;
        }

        bool upgraded = abilitySystem.TryUpgradeAbility(abilityKey, playerLevel);
        if (upgraded)
        {
            skillPoints--;
            Debug.Log($"Habilidad {abilityKey} mejorada. Puntos restantes: {skillPoints}");
            return true;
        }

        Debug.Log($"No se pudo mejorar la habilidad {abilityKey}");
        return false;
    }
    #endregion

    #region Métodos de Mejora de Stats
    public void IncreaseDamage(float amount)
    {
        CurrentDamage += amount;
        OnStatsChanged?.Invoke();
        EventManager.PlayerStatsChanged();
        Debug.Log($"Daño aumentado: +{amount}. Total: {CurrentDamage}");
    }

    public void IncreaseArmor(float amount)
    {
        CurrentArmor += amount;
        OnStatsChanged?.Invoke();
        EventManager.PlayerStatsChanged();
        Debug.Log($"Armadura aumentada: +{amount}. Total: {CurrentArmor}");
    }

    public void IncreaseSpeed(float amount)
    {
        CurrentSpeed += amount;
        UpdateMovementSpeed();
        OnStatsChanged?.Invoke();
        EventManager.PlayerStatsChanged();
        Debug.Log($"Velocidad aumentada: +{amount}. Total: {CurrentSpeed}");
    }
    #endregion

    #region Sistema de Items y Mejoras
    public void ApplyItemStats(ItemData item)
    {
        if (item == null || (PlayerHealth != null && PlayerHealth.isDead)) return;

        CurrentDamage += item.bonusDamage;
        CurrentArmor += item.bonusArmor;
        CurrentSpeed += item.bonusSpeed;

        UpdateMovementSpeed();
        OnStatsChanged?.Invoke();
        EventManager.PlayerStatsChanged();

        Debug.Log($"[ITEM] {item.itemName} aplicado");
    }

    public void RemoveItemStats(ItemData item)
    {
        if (item == null) return;

        CurrentDamage -= item.bonusDamage;
        CurrentArmor -= item.bonusArmor;
        CurrentSpeed -= item.bonusSpeed;

        UpdateMovementSpeed();
        OnStatsChanged?.Invoke();
        EventManager.PlayerStatsChanged();

        Debug.Log($"[ITEM] {item.itemName} removido");
    }
    #endregion

    #region Utilidades
    public void AddGold(int amount)
    {
        if (PlayerHealth != null && PlayerHealth.isDead) return;
        GoldManager.Instance?.AddGold(amount);
    }
    

    public bool SpendGold(int cost)
    {
        if (PlayerHealth != null && PlayerHealth.isDead) return false;
        return GoldManager.Instance?.SpendGold(cost) ?? false;
    }

    public bool IsAlive()
    {
        return PlayerHealth != null ? PlayerHealth.IsAlive() : true;
    }
    #endregion
}