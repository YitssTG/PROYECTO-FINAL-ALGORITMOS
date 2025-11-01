using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Torretas disponibles")]
    public TowerSO[] towerSOList;  // Aquí usas el SO en lugar de GameObjects
    private int selectedTowerIndex = 0;

    [Header("Referencias principales")]
    public AbilitySystem abilitySystem;
    public PlayerStats playerStats;
    public EnemySpawner[] spawners;

    [Header("Progreso del juego")]
    public int enemigosDerrotados = 0;
    public int monedasTotales = 0;
    public int oleadaActual = 1;
    public int incrementoDificultad = 1;

    [Header("Referencias externas")]
    public GoldManager goldManager; // Referencia al GoldManager

    void Awake()
    {
        // Singleton básico
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Verificar que todas las referencias estén asignadas en el Inspector
        VerificarReferencias();
    }

    private void VerificarReferencias()
    {
        if (abilitySystem == null)
            Debug.LogWarning("⚠️ AbilitySystem no está asignado en GameManager.");

        if (playerStats == null)
            Debug.LogWarning("⚠️ PlayerStats no está asignado en GameManager.");

        if (spawners == null || spawners.Length == 0)
            Debug.LogWarning("⚠️ No hay spawners asignados en GameManager.");

        if (goldManager == null)
            Debug.LogWarning("⚠️ GoldManager no está asignado en GameManager.");
    }

    void OnEnable()
    {
        EventManager.OnEnemyDefeated += OnEnemyDead;
        EventManager.OnCoinsCollected += OnCoinsAdded;
    }

    void OnDisable()
    {
        EventManager.OnEnemyDefeated -= OnEnemyDead;
        EventManager.OnCoinsCollected -= OnCoinsAdded;
    }

    // ──────────────────────────────
    // EVENTOS
    // ──────────────────────────────
    private void OnEnemyDead()
    {
        enemigosDerrotados++;
        Debug.Log($"GameManager: Enemigo derrotado. Total: {enemigosDerrotados}");

        // Otorgar XP al jugador
        if (playerStats != null)
            playerStats.AddExperience(25);

        // Verificar si se avanza de oleada
        if (enemigosDerrotados % 10 == 0)
            SiguienteOleada();
    }

    private void OnCoinsAdded(int amount)
    {
        monedasTotales += amount;
        Debug.Log($"GameManager: +{amount} monedas. Total: {monedasTotales}");
    }

    // ──────────────────────────────
    // CONTROL DE OLEADAS Y SPAWNERS
    // ──────────────────────────────
    public void SiguienteOleada()
    {
        oleadaActual++;
        foreach (EnemySpawner spawner in spawners)
        {
            if (spawner != null)
                spawner.ActualizarDificultad(oleadaActual, incrementoDificultad);
        }

        Debug.Log($"🔥 Iniciando oleada {oleadaActual}");
    }

    public void ActivarSpawners(bool estado)
    {
        foreach (EnemySpawner spawner in spawners)
        {
            if (spawner != null)
                spawner.SetActive(estado);
        }

        Debug.Log($"Spawner(s) {(estado ? "activados" : "desactivados")}");
    }

    // ──────────────────────────────
    // SISTEMA DE TORRETAS
    // ──────────────────────────────
    public bool CanBuild()
    {
        TowerSO selectedTower = GetSelectedTowerSO();
        return selectedTower != null && towerSOList.Length > 0 && goldManager.currentGold >= selectedTower.cost;
    }

    public TowerSO GetSelectedTowerSO()
    {
        return towerSOList[selectedTowerIndex];
    }

    public void SelectTower(int index)
    {
        if (index >= 0 && index < towerSOList.Length)
            selectedTowerIndex = index;
    }

    // Método para construir torreta en una casilla
    public void TryToBuildTower(BuildSlot slot)
    {
        TowerSO selectedTower = GetSelectedTowerSO();
        if (goldManager.SpendGold(selectedTower.cost))
        {
            slot.PlaceTower(selectedTower);
        }
        else
        {
            Debug.Log("No tienes suficiente oro para colocar esta torreta.");
        }
    }
}
