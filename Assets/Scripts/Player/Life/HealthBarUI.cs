using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class HealthBarUI : MonoBehaviour
{
    [Header("Referencias de Barra")]
    public Slider healthSlider;
    public Image healthFillImage;
    public TextMeshProUGUI healthText;

    [Header("Referencias del Jugador")]
    public PlayerHealth playerHealth; // ⭐ ASIGNAR EN INSPECTOR
    public HealthRegenSystem regenSystem; // ⭐ ASIGNAR EN INSPECTOR

    [Header("Colores y Efectos")]
    public Color highHealthColor = Color.green;
    public Color mediumHealthColor = Color.yellow;
    public Color lowHealthColor = Color.red;
    public Color regenEffectColor = new Color(0, 1, 0, 0.3f);

    [Header("Animaciones")]
    public float regenPulseSpeed = 2f;
    public float damageFlashDuration = 0.2f;

    private int maxHealth;
    private Coroutine flashCoroutine;
    private Coroutine regenEffectCoroutine;

    void Start()
    {
        InitializeReferences();
        SetupEventListeners();
        UpdateHealthDisplay();
    }

    private void InitializeReferences()
    {
        // Verificar referencias asignadas en Inspector
        if (playerHealth == null)
        {
            Debug.LogError("❌ HealthBarUI: PlayerHealth no asignado en Inspector");
            enabled = false;
            return;
        }

        maxHealth = playerHealth.maxHealth;

        if (regenSystem == null)
        {
            Debug.LogWarning("⚠️ HealthBarUI: HealthRegenSystem no asignado en Inspector");
        }

        Debug.Log("✅ HealthBarUI inicializado correctamente");
    }

    private void SetupEventListeners()
    {
        playerHealth.OnHealthChanged.AddListener(OnHealthChanged);
    }

    private void OnHealthChanged(int currentHealth)
    {
        UpdateHealthDisplay();

        if (currentHealth < maxHealth)
        {
            ShowDamageFlash();
        }
    }

    private void UpdateHealthDisplay()
    {
        if (playerHealth == null) return;

        int currentHealth = playerHealth.currentHealth;

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }

        if (healthText != null)
        {
            healthText.text = $"{currentHealth}/{maxHealth}";
        }

        UpdateHealthColor(currentHealth);
    }

    private void UpdateHealthColor(int currentHealth)
    {
        if (healthFillImage == null) return;

        float healthPercentage = (float)currentHealth / maxHealth;
        healthFillImage.color = GetHealthColor(healthPercentage);
    }

    void Update()
    {
        UpdateRegenEffect();
    }

    private void UpdateRegenEffect()
    {
        if (regenSystem == null || healthFillImage == null) return;

        if (regenSystem.isRegenerating && regenEffectCoroutine == null)
        {
            regenEffectCoroutine = StartCoroutine(RegenPulseEffect());
        }
        else if (!regenSystem.isRegenerating && regenEffectCoroutine != null)
        {
            StopCoroutine(regenEffectCoroutine);
            regenEffectCoroutine = null;
            healthFillImage.color = GetHealthColor((float)playerHealth.currentHealth / maxHealth);
        }
    }

    private IEnumerator RegenPulseEffect()
    {
        Image fillImage = healthFillImage;
        Color originalColor = GetHealthColor((float)playerHealth.currentHealth / maxHealth);

        while (regenSystem != null && regenSystem.isRegenerating)
        {
            float pulse = (Mathf.Sin(Time.time * regenPulseSpeed) + 1) * 0.5f;
            fillImage.color = Color.Lerp(originalColor, regenEffectColor, pulse * 0.5f);
            yield return null;
        }

        fillImage.color = originalColor;
        regenEffectCoroutine = null;
    }

    private void ShowDamageFlash()
    {
        if (flashCoroutine != null)
            StopCoroutine(flashCoroutine);

        flashCoroutine = StartCoroutine(DamageFlashRoutine());
    }

    private IEnumerator DamageFlashRoutine()
    {
        if (healthFillImage == null) yield break;

        Color originalColor = healthFillImage.color;
        healthFillImage.color = Color.red;

        yield return new WaitForSeconds(damageFlashDuration);

        healthFillImage.color = originalColor;
        flashCoroutine = null;
    }

    private Color GetHealthColor(float healthPercentage)
    {
        if (healthPercentage > 0.6f)
            return highHealthColor;
        else if (healthPercentage > 0.3f)
            return mediumHealthColor;
        else
            return lowHealthColor;
    }

    #region API Pública
    public void UpdateMaxHealth(int newMaxHealth)
    {
        maxHealth = newMaxHealth;
        UpdateHealthDisplay();
    }

    public void ShowHealthBar(bool show)
    {
        gameObject.SetActive(show);
    }

    public void SetPlayerHealthReference(PlayerHealth newPlayerHealth)
    {
        // Remover listener antiguo
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged.RemoveListener(OnHealthChanged);
        }

        // Asignar nueva referencia
        playerHealth = newPlayerHealth;

        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged.AddListener(OnHealthChanged);
            maxHealth = playerHealth.maxHealth;
            UpdateHealthDisplay();
        }
    }
    #endregion

    void OnDestroy()
    {
        if (flashCoroutine != null)
            StopCoroutine(flashCoroutine);

        if (regenEffectCoroutine != null)
            StopCoroutine(regenEffectCoroutine);

        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged.RemoveListener(OnHealthChanged);
        }
    }
}