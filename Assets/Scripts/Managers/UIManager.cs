using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Referencias UI")]
    public TMP_Text goldText;
    public TMP_Text experienceText;
    public TMP_Text enemiesDefeatedText;
    public TMP_Text levelText;
    public HealthBarUI healthBarUI;

    [Header("Referencias de Sistemas")]
    public GoldManager goldManager; // ⭐ ASIGNAR EN INSPECTOR
    public PlayerStats playerStats; // ⭐ ASIGNAR EN INSPECTOR  
    public PlayerHealth playerHealth; // ⭐ NUEVO: ASIGNAR EN INSPECTOR

    void Start()
    {
        InitializeReferences();
        SetupEventListeners();
        UpdateAllDisplays();
    }

    #region Inicialización
    private void InitializeReferences()
    {
        // Usar GameManager como fallback solo si las referencias no están asignadas
        if (goldManager == null && GameManager.Instance != null)
        {
            goldManager = GameManager.Instance.GetGoldManager();
        }

        if (playerStats == null && GameManager.Instance != null)
        {
            playerStats = GameManager.Instance.GetPlayerStats();
        }

        if (playerHealth == null)
        {
            // Buscar en el mismo objeto o padres/hijos, pero NO FindObjectOfType
            playerHealth = GetComponentInParent<PlayerHealth>();
            if (playerHealth == null)
                playerHealth = GetComponentInChildren<PlayerHealth>();
        }

        // Verificar componentes críticos
        if (goldManager == null)
            Debug.LogError("❌ GoldManager no encontrado. Asigna la referencia en Inspector.");
        else
            Debug.Log("✅ GoldManager encontrado");

        if (playerStats == null)
            Debug.LogError("❌ PlayerStats no encontrado. Asigna la referencia en Inspector.");
        else
            Debug.Log("✅ PlayerStats encontrado");

        if (playerHealth == null)
            Debug.LogError("❌ PlayerHealth no encontrado. Asigna la referencia en Inspector.");
        else
            Debug.Log("✅ PlayerHealth encontrado");

        if (healthBarUI == null)
            Debug.LogError("❌ HealthBarUI no asignado en UIManager");
        else
            Debug.Log("✅ HealthBarUI encontrado");
    }

    private void SetupEventListeners()
    {
        // Gold Manager events
        if (goldManager != null)
        {
            // Usar el evento directamente si existe
            if (goldManager.OnGoldChanged != null)
            {
                goldManager.OnGoldChanged.AddListener(UpdateGoldDisplay);
            }
        }

        // Player Health events
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged.AddListener(OnHealthChanged);
        }

        // Player Stats events
        if (playerStats != null)
        {
            if (playerStats.OnExperienceChanged != null)
                playerStats.OnExperienceChanged.AddListener(UpdateExperienceDisplay);

            if (playerStats.OnLevelChanged != null)
                playerStats.OnLevelChanged.AddListener(UpdateLevelDisplay);
        }

        // Eventos de autocuración
        EventManager.OnHealthRegenStarted += OnHealthRegenStarted;
        EventManager.OnHealthRegenStopped += OnHealthRegenStopped;
        EventManager.OnHealthRegenTick += OnHealthRegenTick;
    }
    #endregion

    #region Actualización de UI
    private void Update()
    {
        // Actualización manual como fallback
        UpdateGoldDisplayManual();
        UpdateEnemiesDefeatedDisplay();
    }

    private void UpdateAllDisplays()
    {
        UpdateGoldDisplayManual();
        UpdateExperienceDisplayManual();
        UpdateLevelDisplayManual();
        UpdateEnemiesDefeatedDisplay();
    }

    private void OnHealthChanged(int currentHealth)
    {
        // HealthBarUI maneja su propia actualización
        Debug.Log($"❤️ Vida actualizada: {currentHealth}");
    }

    private void OnHealthRegenStarted()
    {
        Debug.Log("🔄 Autocuración iniciada - Efectos visuales pueden ir aquí");
    }

    private void OnHealthRegenStopped()
    {
        Debug.Log("⏹️ Autocuración detenida");
    }

    private void OnHealthRegenTick(int healAmount)
    {
        Debug.Log($"💚 Curación tick: +{healAmount} HP");
    }

    // Métodos con eventos
    private void UpdateGoldDisplay(int newGoldAmount)
    {
        if (goldText != null)
            goldText.text = $"💰 {newGoldAmount}G";
    }

    private void UpdateExperienceDisplay(int newExperienceAmount)
    {
        if (experienceText != null)
            experienceText.text = $"📚 {newExperienceAmount}";
    }

    private void UpdateLevelDisplay(int newLevel)
    {
        if (levelText != null)
            levelText.text = $"⭐ Nv {newLevel}";
    }

    // Métodos manuales (fallback)
    private void UpdateGoldDisplayManual()
    {
        if (goldText != null && goldManager != null)
            goldText.text = $"💰 {goldManager.currentGold}G";
    }

    private void UpdateExperienceDisplayManual()
    {
        if (experienceText != null && playerStats != null)
            experienceText.text = $"📚 {playerStats.experience}";
    }

    private void UpdateLevelDisplayManual()
    {
        if (levelText != null && playerStats != null)
            levelText.text = $"⭐ Nv {playerStats.playerLevel}";
    }

    private void UpdateEnemiesDefeatedDisplay()
    {
        if (enemiesDefeatedText != null && GameManager.Instance != null)
        {
            enemiesDefeatedText.text = $"🎯 {GameManager.Instance.enemigosDerrotados}";
        }
    }

    public void RefreshAllUI()
    {
        UpdateAllDisplays();
    }
    #endregion

    #region Eventos y Cleanup
    private void CleanupEventListeners()
    {
        if (goldManager != null && goldManager.OnGoldChanged != null)
            goldManager.OnGoldChanged.RemoveListener(UpdateGoldDisplay);

        if (playerHealth != null)
            playerHealth.OnHealthChanged.RemoveListener(OnHealthChanged);

        if (playerStats != null)
        {
            if (playerStats.OnExperienceChanged != null)
                playerStats.OnExperienceChanged.RemoveListener(UpdateExperienceDisplay);

            if (playerStats.OnLevelChanged != null)
                playerStats.OnLevelChanged.RemoveListener(UpdateLevelDisplay);
        }

        EventManager.OnHealthRegenStarted -= OnHealthRegenStarted;
        EventManager.OnHealthRegenStopped -= OnHealthRegenStopped;
        EventManager.OnHealthRegenTick -= OnHealthRegenTick;
    }

    void OnDisable() => CleanupEventListeners();
    void OnDestroy() => CleanupEventListeners();
    #endregion
}