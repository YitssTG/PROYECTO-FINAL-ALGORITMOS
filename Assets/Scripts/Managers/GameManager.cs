using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Torretas disponibles")]
    public TowerSO[] towerSOList;
    private int selectedTowerIndex = 0;

    [Header("Referencias principales")]
    public AbilitySystem abilitySystem;
    public PlayerStats playerStats;
    public EnemySpawner[] spawners;

    [Header("Progreso del juego")]
    public int enemigosDerrotados = 0;
    public int monedasTotales = 0;

    [Header("Referencias externas")]
    public GoldManager goldManager;

    void Awake()
    {
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

    private void OnEnemyDead()
    {
        enemigosDerrotados++;
        Debug.Log($"GameManager: Enemigo derrotado. Total: {enemigosDerrotados}");

        if (playerStats != null)
            playerStats.AddExperience(25);
    }

    private void OnCoinsAdded(int amount)
    {
        monedasTotales += amount;
        Debug.Log($"GameManager: +{amount} monedas. Total: {monedasTotales}");
    }
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
