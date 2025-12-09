using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [Header("Configuración de Salud")]
    public int maxHealth = 100;
    public int currentHealth;
    public bool isInvulnerable = false;
    public bool isDead = false;

    [Header("Opciones de Muerte")]
    public bool destroyOnDeath = true;
    public float deathDelay = 0.1f;

    [Header("Eventos de Salud")]
    public UnityEvent<int> OnHealthChanged = new();
    public UnityEvent OnPlayerDiedEvent = new();
    public UnityEvent OnPlayerRespawned = new();

    private HealthRegenSystem healthRegen;
    private PlayerStats playerStats;

    void Awake()
    {
        healthRegen = GetComponent<HealthRegenSystem>();
        playerStats = GetComponent<PlayerStats>();
    }

    void Start()
    {
        InitializeHealth();
    }

    #region Inicialización
    public void InitializeHealth()
    {
        currentHealth = maxHealth;
        isDead = false;
        OnHealthChanged?.Invoke(currentHealth);
        EventManager.LifeChanged(currentHealth);

        Debug.Log($"PlayerHealth inicializado - Vida: {currentHealth}/{maxHealth}");
    }
    #endregion

    #region Daño y Curación
    public void TakeDamage(int amount)
    {
        if (isInvulnerable || amount <= 0 || isDead) return;

        int finalDamage = CalculateFinalDamage(amount);

        currentHealth -= finalDamage;
        currentHealth = Mathf.Max(0, currentHealth);

        OnHealthChanged?.Invoke(currentHealth);
        EventManager.LifeChanged(currentHealth);

        Debug.Log($"{finalDamage} daño recibido. Vida: {currentHealth}/{maxHealth}");

        healthRegen?.OnDamageTaken();

        if (currentHealth <= 0 && !isDead)
            Die();
    }

    private int CalculateFinalDamage(int incomingDamage)
    {
        float armor = playerStats?.CurrentArmor ?? 0f;
        return Mathf.Max(1, incomingDamage - Mathf.RoundToInt(armor));
    }

    public void Heal(int amount)
    {
        if (amount <= 0 || isDead) return;

        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);

        OnHealthChanged?.Invoke(currentHealth);
        EventManager.LifeChanged(currentHealth);

        Debug.Log($"Curado {amount}. Vida: {currentHealth}/{maxHealth}");
    }
    #endregion

    #region IDamageable
    public bool IsDead() => isDead;

    public int GetCurrentHealth() => currentHealth;

    public bool IsAlive()
    {
        return !isDead && currentHealth > 0;
    }
    #endregion

    #region Sistema de Muerte
    private void Die()
    {
        if (isDead) return;

        isDead = true;
        currentHealth = 0;

        Debug.Log("PLAYER MURIÓ - Vida en 0");

        OnPlayerDiedEvent?.Invoke();
        EventManager.PlayerDied();

        DisablePlayerComponents();

        if (destroyOnDeath)
        {
            if (deathDelay <= 0)
                Destroy(gameObject);
            else
                Invoke(nameof(DestroyPlayer), deathDelay);
        }
    }

    private void DestroyPlayer()
    {
        Destroy(gameObject);
    }

    private void DisablePlayerComponents()
    {
        var autoAttack = GetComponent<PlayerAutoAttack>();
        if (autoAttack != null)
        {
            autoAttack.enabled = false;
            autoAttack.StopAllActions();
        }

        var agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent != null)
        {
            agent.isStopped = true;
            agent.enabled = false;
        }

        var controller = GetComponent<PlayerController>();
        if (controller != null) controller.enabled = false;

        var abilitySystem = GetComponent<AbilitySystem>();
        if (abilitySystem != null) abilitySystem.enabled = false;

        var col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        healthRegen?.ForceStopRegeneration();

        Debug.Log("Player components disabled.");
    }
    #endregion

    #region Respawn
    public void Respawn(Vector3 respawnPosition)
    {
        if (!isDead) return;

        isDead = false;
        currentHealth = maxHealth;

        transform.position = respawnPosition;

        ReactivatePlayerComponents();

        OnHealthChanged?.Invoke(currentHealth);
        EventManager.LifeChanged(currentHealth);
        OnPlayerRespawned?.Invoke();

        Debug.Log($"Player respawned in {respawnPosition}");
    }

    private void ReactivatePlayerComponents()
    {
        var col = GetComponent<Collider>();
        if (col != null) col.enabled = true;

        var agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent != null)
        {
            agent.enabled = true;
            agent.isStopped = false;
        }

        foreach (var comp in GetComponents<MonoBehaviour>())
        {
            if (comp != this)
                comp.enabled = true;
        }
    }
    #endregion
}
