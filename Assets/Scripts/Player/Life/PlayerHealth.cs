using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    [Header("Configuración de Salud")]
    public int maxHealth = 100;
    public int currentHealth;
    public bool isInvulnerable = false;
    public bool isDead = false;

    [Header("Opciones de Muerte")]
    public bool destroyOnDeath = true; // ⭐ NUEVO: Controlar si se destruye
    public float deathDelay = 0.1f;    // ⭐ REDUCIDO: Muy poco tiempo

    [Header("Eventos de Salud")]
    public UnityEvent<int> OnHealthChanged = new();
    public UnityEvent OnPlayerDiedEvent = new();
    public UnityEvent OnPlayerRespawned = new();

    // Componentes relacionados
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

        Debug.Log($"❤️ PlayerHealth inicializado - Vida: {currentHealth}/{maxHealth}");
    }
    #endregion

    #region Sistema de Daño y Curación
    public void TakeDamage(int amount)
    {
        if (isInvulnerable || amount <= 0 || isDead) return;

        // Calcular daño final con armadura
        int finalDamage = CalculateFinalDamage(amount);

        currentHealth -= finalDamage;
        currentHealth = Mathf.Max(0, currentHealth);

        OnHealthChanged?.Invoke(currentHealth);
        EventManager.LifeChanged(currentHealth);

        Debug.Log($"💔 {finalDamage} daño recibido. Vida: {currentHealth}/{maxHealth}");

        // Notificar sistema de regeneración
        healthRegen?.OnDamageTaken();

        // ⭐ CUANDO VIDA LLEGA A 0 - PLAYER MUERE AL INSTANTE
        if (currentHealth <= 0 && !isDead)
        {
            Die();
        }
    }

    private int CalculateFinalDamage(int incomingDamage)
    {
        float armor = playerStats?.CurrentArmor ?? 0f;
        int damageAfterArmor = Mathf.Max(1, incomingDamage - Mathf.RoundToInt(armor));
        return damageAfterArmor;
    }

    public void Heal(int amount)
    {
        if (amount <= 0 || isDead) return;

        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);

        OnHealthChanged?.Invoke(currentHealth);
        EventManager.LifeChanged(currentHealth);

        Debug.Log($"❤️ Curado {amount}. Vida: {currentHealth}/{maxHealth}");
    }
    #endregion

    #region ⭐ SISTEMA DE MUERTE MEJORADO (MUERTE INMEDIATA)
    private void Die()
    {
        if (isDead) return;

        isDead = true;
        currentHealth = 0;

        Debug.Log("💀 PLAYER MURIÓ INMEDIATAMENTE - Vida llegó a 0");

        // 1. Eventos de muerte (INMEDIATOS)
        OnHealthChanged?.Invoke(currentHealth);
        OnPlayerDiedEvent?.Invoke();
        EventManager.PlayerDied();

        // 2. Desactivar componentes del player (INMEDIATO)
        DisablePlayerComponents();

        // 3. ⭐ OPCIÓN 1: Destrucción inmediata
        if (destroyOnDeath && deathDelay <= 0f)
        {
            DestroyPlayerImmediate();
        }
        // ⭐ OPCIÓN 2: Destrucción con delay muy corto
        else if (destroyOnDeath && deathDelay > 0f)
        {
            Invoke(nameof(DestroyPlayer), deathDelay);
        }
        // ⭐ OPCIÓN 3: No destruir (para respawn)
        else
        {
            Debug.Log("🔵 Player muerto pero no destruido (listo para respawn)");
        }
    }

    private void DisablePlayerComponents()
    {
        // Desactivar combate
        var autoAttack = GetComponent<PlayerAutoAttack>();
        if (autoAttack != null)
        {
            autoAttack.enabled = false;
            autoAttack.StopAllActions(); // ⭐ DETENER ACCIONES ACTUALES
        }

        // Desactivar movimiento (INMEDIATO)
        var movement = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (movement != null)
        {
            movement.isStopped = true;
            movement.enabled = false;
        }

        // Desactivar control
        var controller = GetComponent<PlayerController>();
        if (controller != null) controller.enabled = false;

        // Desactivar habilidades
        var abilitySystem = GetComponent<AbilitySystem>();
        if (abilitySystem != null) abilitySystem.enabled = false;

        // Desactivar colisiones (IMPORTANTE para que no siga recibiendo daño)
        var collider = GetComponent<Collider>();
        if (collider != null) collider.enabled = false;

        // Detener regeneración
        if (healthRegen != null) healthRegen.ForceStopRegeneration();

        Debug.Log("🔴 Componentes del player desactivados INMEDIATAMENTE");
    }

    // ⭐ NUEVO: Destrucción inmediata
    private void DestroyPlayerImmediate()
    {
        Debug.Log("💀 Destruyendo GameObject del player INMEDIATAMENTE");
        Destroy(gameObject);
    }

    private void DestroyPlayer()
    {
        Debug.Log("💀 Destruyendo GameObject del player");
        Destroy(gameObject);
    }

    public void Respawn(Vector3 respawnPosition)
    {
        if (!isDead) return;

        isDead = false;
        currentHealth = maxHealth;

        // Cancelar cualquier destrucción pendiente
        CancelInvoke(nameof(DestroyPlayer));

        ReactivatePlayerComponents();
        transform.position = respawnPosition;

        OnHealthChanged?.Invoke(currentHealth);
        EventManager.LifeChanged(currentHealth);
        OnPlayerRespawned?.Invoke();

        Debug.Log($"🔵 Player revivido en {respawnPosition}");
    }

    private void ReactivatePlayerComponents()
    {
        // Reactivar colisiones
        var collider = GetComponent<Collider>();
        if (collider != null) collider.enabled = true;

        var components = GetComponents<MonoBehaviour>();
        foreach (var comp in components)
        {
            if (comp != this && comp is not HealthRegenSystem)
                comp.enabled = true;
        }

        var movement = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (movement != null)
        {
            movement.enabled = true;
            movement.isStopped = false;
        }
    }
    #endregion

    #region Utilidades
    public float GetHealthPercentage() => (float)currentHealth / maxHealth;
    public bool IsAlive() => !isDead && currentHealth > 0;
    public bool IsDead() => isDead;
    public bool IsFullHealth() => currentHealth >= maxHealth;
    public bool IsLowHealth() => GetHealthPercentage() <= 0.3f;

    public void SetInvulnerable(bool invulnerable, float duration = 0f)
    {
        isInvulnerable = invulnerable;
        Debug.Log($"{(invulnerable ? "🛡️" : "❌")} Invulnerabilidad {(invulnerable ? "activada" : "desactivada")}");

        if (duration > 0f)
        {
            Invoke(nameof(RemoveInvulnerability), duration);
        }
    }

    private void RemoveInvulnerability()
    {
        isInvulnerable = false;
    }

    // ⭐ NUEVO: Forzar muerte inmediata (para testing)
    public void ForceDie()
    {
        currentHealth = 0;
        Die();
    }
    #endregion
}