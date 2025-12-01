using UnityEngine;
using System.Collections;

public class HealthRegenSystem : MonoBehaviour
{
    [Header("Configuración Autocuración")]
    public bool enableHealthRegen = true;
    public float regenDelay = 5f;
    public float regenRate = 10f;
    public float regenInterval = 0.5f;

    [Header("Estadísticas")]
    public float timeSinceLastDamage = 0f;
    public bool isRegenerating = false;
    public float lastDamageAmount = 0f;

    [Header("Referencias")]
    public PlayerHealth playerHealth; // ⭐ ASIGNAR EN INSPECTOR

    private Coroutine regenCoroutine;

    void Awake()
    {
        // Intentar obtener referencia automáticamente en el mismo GameObject
        if (playerHealth == null)
            playerHealth = GetComponent<PlayerHealth>();

        if (playerHealth == null)
        {
            Debug.LogError("❌ HealthRegenSystem: PlayerHealth no encontrado. Asigna la referencia en el Inspector.");
            enabled = false;
            return;
        }
    }

    void Start()
    {
        playerHealth.OnHealthChanged.AddListener(OnHealthChanged);
        Debug.Log("✅ Sistema de autocuración inicializado");
    }

    void Update()
    {
        if (!enableHealthRegen || playerHealth.currentHealth >= playerHealth.maxHealth)
            return;

        timeSinceLastDamage += Time.deltaTime;

        if (timeSinceLastDamage >= regenDelay && !isRegenerating)
        {
            StartRegeneration();
        }
    }

    private void OnHealthChanged(int currentHealth)
    {
        if (currentHealth < playerHealth.maxHealth)
        {
            timeSinceLastDamage = 0f;
            StopRegeneration();
            lastDamageAmount = playerHealth.maxHealth - currentHealth;
        }
    }

    public void OnDamageTaken()
    {
        timeSinceLastDamage = 0f;
        StopRegeneration();
    }

    private void StartRegeneration()
    {
        if (isRegenerating) return;

        isRegenerating = true;
        regenCoroutine = StartCoroutine(RegenerationRoutine());
        EventManager.HealthRegenStarted();
    }

    private void StopRegeneration()
    {
        if (!isRegenerating) return;

        isRegenerating = false;

        if (regenCoroutine != null)
        {
            StopCoroutine(regenCoroutine);
            regenCoroutine = null;
        }

        EventManager.HealthRegenStopped();
    }

    private IEnumerator RegenerationRoutine()
    {
        while (isRegenerating && playerHealth.currentHealth < playerHealth.maxHealth)
        {
            float healAmount = regenRate * regenInterval;
            int healAmountInt = Mathf.RoundToInt(healAmount);
            healAmountInt = Mathf.Min(healAmountInt, playerHealth.maxHealth - playerHealth.currentHealth);

            if (healAmountInt > 0)
            {
                playerHealth.Heal(healAmountInt);
                EventManager.HealthRegenTick(healAmountInt);
            }

            yield return new WaitForSeconds(regenInterval);
        }

        isRegenerating = false;
    }

    #region API Pública
    public void SetRegenRate(float newRate) => regenRate = newRate;
    public void SetRegenDelay(float newDelay) => regenDelay = newDelay;

    public void EnableHealthRegen(bool enable)
    {
        enableHealthRegen = enable;
        if (!enable) StopRegeneration();
    }

    public void ForceStartRegeneration()
    {
        timeSinceLastDamage = regenDelay;
        StartRegeneration();
    }

    public void ForceStopRegeneration()
    {
        StopRegeneration();
        timeSinceLastDamage = 0f;
    }

    public float GetRegenProgress() => Mathf.Clamp01(timeSinceLastDamage / regenDelay);

    public bool CanRegenerate()
    {
        return timeSinceLastDamage >= regenDelay &&
               playerHealth.currentHealth < playerHealth.maxHealth;
    }
    #endregion

    void OnDestroy()
    {
        StopRegeneration();
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged.RemoveListener(OnHealthChanged);
        }
    }
}