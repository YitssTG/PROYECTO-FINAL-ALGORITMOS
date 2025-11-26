using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Referencias UI")]
    public TMP_Text goldText;
    public TMP_Text healthText;
    public TMP_Text experienceText;
    public TMP_Text enemiesDefeatedText;
    public TMP_Text levelText;

    [Header("Referencias de Sistemas")]
    private GoldManager goldManager;
    private PlayerStats playerStats;

    void Start()
    {
        InitializeReferences();
        SetupEventListeners();
        UpdateAllDisplays();
    }

    #region Inicialización
    private void InitializeReferences()
    {
        // Obtener referencias del GameManager
        if (GameManager.Instance != null)
        {
            goldManager = GameManager.Instance.GetGoldManager();
            playerStats = GameManager.Instance.GetPlayerStats();
        }

        // Fallback: buscar directamente si no están en GameManager
        if (goldManager == null)
            goldManager = FindObjectOfType<GoldManager>();

        if (playerStats == null)
            playerStats = FindObjectOfType<PlayerStats>();

        // Verificar componentes críticos
        if (goldManager == null)
            Debug.LogError("❌ GoldManager no encontrado");
        else
            Debug.Log("✅ GoldManager encontrado");

        if (playerStats == null)
            Debug.LogError("❌ PlayerStats no encontrado");
        else
            Debug.Log("✅ PlayerStats encontrado");
    }

    private void SetupEventListeners()
    {
        // Gold Manager events - VERIFICAR QUE OnGoldChanged EXISTA
        if (goldManager != null)
        {
            // Verificar si el evento existe antes de agregar el listener
            var eventField = goldManager.GetType().GetField("OnGoldChanged");
            if (eventField != null)
            {
                goldManager.OnGoldChanged.AddListener(UpdateGoldDisplay);
                Debug.Log("✅ Listener de oro configurado");
            }
            else
            {
                Debug.LogWarning("⚠️ OnGoldChanged no existe en GoldManager, usando actualización manual");
                // Actualización manual en Update
            }
        }

        // Player Stats events
        if (playerStats != null)
        {
            if (playerStats.OnHealthChanged != null)
                playerStats.OnHealthChanged.AddListener(UpdateHealthDisplay);

            if (playerStats.OnExperienceChanged != null)
                playerStats.OnExperienceChanged.AddListener(UpdateExperienceDisplay);

            if (playerStats.OnLevelChanged != null)
                playerStats.OnLevelChanged.AddListener(UpdateLevelDisplay);
        }
    }
    #endregion

    #region Actualización de UI
    private void Update()
    {
        // Actualización manual como fallback si los eventos no funcionan
        UpdateGoldDisplayManual();
        UpdateEnemiesDefeatedDisplay();
    }

    private void UpdateAllDisplays()
    {
        // Actualizar toda la UI al inicio
        UpdateGoldDisplayManual();
        UpdateHealthDisplayManual();
        UpdateExperienceDisplayManual();
        UpdateLevelDisplayManual();
        UpdateEnemiesDefeatedDisplay();
    }

    // Métodos con eventos
    private void UpdateGoldDisplay(int newGoldAmount)
    {
        if (goldText != null)
            goldText.text = $"💰 {newGoldAmount}G";
    }

    private void UpdateHealthDisplay(int newHealthAmount)
    {
        if (healthText != null)
            healthText.text = $"❤️ {newHealthAmount}";
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

    private void UpdateHealthDisplayManual()
    {
        if (healthText != null && playerStats != null)
            healthText.text = $"❤️ {playerStats.currentHealth}";
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

    // Método público para forzar actualización
    public void RefreshAllUI()
    {
        UpdateAllDisplays();
        Debug.Log("🔄 UI actualizada manualmente");
    }
    #endregion

    #region Eventos y Cleanup
    void OnEnable()
    {
        // Escuchar eventos globales si existen
        // EventManager.OnEnemyDefeated += UpdateEnemiesDefeatedDisplay;
    }

    void OnDisable()
    {
        // Remover eventos globales
        // EventManager.OnEnemyDefeated -= UpdateEnemiesDefeatedDisplay;

        // Limpiar listeners
        CleanupEventListeners();
    }

    private void OnDestroy()
    {
        CleanupEventListeners();
    }

    private void CleanupEventListeners()
    {
        // Limpiar todos los listeners
        if (goldManager != null && goldManager.OnGoldChanged != null)
            goldManager.OnGoldChanged.RemoveListener(UpdateGoldDisplay);

        if (playerStats != null)
        {
            if (playerStats.OnHealthChanged != null)
                playerStats.OnHealthChanged.RemoveListener(UpdateHealthDisplay);

            if (playerStats.OnExperienceChanged != null)
                playerStats.OnExperienceChanged.RemoveListener(UpdateExperienceDisplay);

            if (playerStats.OnLevelChanged != null)
                playerStats.OnLevelChanged.RemoveListener(UpdateLevelDisplay);
        }
    }
    #endregion
}