using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Configuración de Torres")]
    public TowerSO[] towerSOList;
    private int selectedTowerIndex = 0;

    [Header("Sistemas del Juego")]
    public AbilitySystem abilitySystem;
    public PlayerStats playerStats;
    public EnemySpawner[] spawners;
    public GoldManager goldManager;
    public WaveManager waveManager;

    [Header("Estadísticas del Juego")]
    public int enemigosDerrotados = 0;
    public int monedasTotales = 0;

    [Header("Ability Management")]
    public AbilityManager abilityManager;

    [Header("Shop and Inventory Systems")]
    public ShopManager shopManager;
    public ItemManager itemManager;
    public InventoryManager inventoryManager;

    [Header("Town Life")]
    public float maxTownHealth = 100f;
    public float currentTownHealth;

    public System.Action<float> OnTownHealthChanged;

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

        InitializeManagers();
    }

    void Start()
    {
        itemManager.Initialize();
        currentTownHealth = maxTownHealth;
        Debug.Log("GAME MANAGER INICIALIZADO CORRECTAMENTE");
    }

    private void OnEnable()
    {
        EventManager.OnEnemyDefeated += OnEnemyDead;
        EventManager.OnCoinsCollected += OnCoinsAdded;
        EventManager.OnWaveStarted += OnWaveStarted;
        EventManager.OnWaveCompleted += OnWaveCompleted;
        EventManager.OnPlayerDied += OnPlayerDied; // ⭐ NUEVO
    }

    private void OnDisable()
    {
        EventManager.OnEnemyDefeated -= OnEnemyDead;
        EventManager.OnCoinsCollected -= OnCoinsAdded;
        EventManager.OnWaveStarted -= OnWaveStarted;
        EventManager.OnWaveCompleted -= OnWaveCompleted;
        EventManager.OnPlayerDied -= OnPlayerDied; // ⭐ NUEVO
    }

    #region Verificación de Referencias
    private void VerificarReferencias()
    {
        if (abilitySystem == null)
            Debug.LogWarning("AbilitySystem no está asignado en GameManager.");

        if (playerStats == null)
            Debug.LogWarning("PlayerStats no está asignado en GameManager.");

        if (spawners == null || spawners.Length == 0)
            Debug.LogWarning("No hay spawners asignados en GameManager.");

        if (goldManager == null)
            Debug.LogWarning("GoldManager no está asignado en GameManager.");

        if (waveManager == null)
            Debug.LogWarning("WaveManager no está asignado en GameManager.");

        if (abilityManager == null)
            Debug.LogWarning("AbilityManager no asignado en GameManager.");

        if (shopManager == null)
            Debug.LogWarning("ShopManager no asignado en GameManager.");

        if (itemManager == null)
            Debug.LogWarning("ItemManager no asignado en GameManager.");

        if (inventoryManager == null)
            Debug.LogWarning("InventoryManager no asignado en GameManager.");

        if (towerSOList == null || towerSOList.Length == 0)
            Debug.LogWarning("No hay torretas configuradas en GameManager.");
    }
    #endregion

    #region Sistema de Eventos
    private void OnEnemyDead()
    {
        enemigosDerrotados++;
        playerStats?.AddExperience(25);  
        Debug.Log($"GameManager: Enemigo derrotado. Total: {enemigosDerrotados}");
    }

    private void OnCoinsAdded(int amount)
    {
        monedasTotales += amount;
        goldManager?.AddGold(amount);  
        Debug.Log($"GameManager: +{amount} monedas. Total: {monedasTotales}");
    }

    private void OnWaveStarted(int waveNumber)
    {
        Debug.Log($"GameManager: Oleada {waveNumber} iniciada");
    }

    private void OnWaveCompleted(int waveNumber)
    {
        Debug.Log($"GameManager: Oleada {waveNumber} completada");
        if (goldManager != null)
        {
            int recompensa = waveNumber * 50;
            goldManager.AddGold(recompensa);
            Debug.Log($"Recompensa de oleada: +{recompensa} oro");
        }
    }

    private void OnPlayerDied()
    {
        Debug.Log("GameManager: Player murió - Cargando Menú...");

        // Reinicia stats si quieres
        ResetGameStats();

        // Cargar escena "Menu"
        SceneManager.LoadScene("Menu");
    }
    #endregion

    #region Sistema de Torres
    public TowerSO GetSelectedTowerSO()
    {
        if (towerSOList.Length == 0) return null;
        return towerSOList[selectedTowerIndex];
    }

    public void SelectTower(int index)
    {
        if (index >= 0 && index < towerSOList.Length)
        {
            selectedTowerIndex = index;
            Debug.Log($"Torreta seleccionada: {towerSOList[index].name}");
        }
    }

    public bool CanBuildTower()
    {
        TowerSO selectedTower = GetSelectedTowerSO();
        return selectedTower != null && goldManager != null && goldManager.currentGold >= selectedTower.cost;
    }

    public void TryToBuildTower(BuildSlot slot)
    {
        if (slot == null)
        {
            Debug.LogWarning("Slot de construcción no válido");
            return;
        }

        TowerSO selectedTower = GetSelectedTowerSO();
        if (selectedTower == null)
        {
            Debug.LogWarning("No hay torreta seleccionada");
            return;
        }

        if (goldManager != null && goldManager.SpendGold(selectedTower.cost))
        {
            slot.PlaceTower(selectedTower);
            Debug.Log($"Torreta {selectedTower.name} construida exitosamente");
        }
        else
        {
            Debug.Log($"No tienes suficiente oro para {selectedTower.name}. Costo: {selectedTower.cost}");
        }
    }
    public TowerSO GetTowerByIndex(int index)
    {
        if (index < 0 || index >= towerSOList.Length) return null;
        return towerSOList[index];
    }
    public void DamageVillage(float amount)
    {
        currentTownHealth -= amount;
        currentTownHealth = Mathf.Clamp(currentTownHealth, 0f, maxTownHealth);

        OnTownHealthChanged?.Invoke(currentTownHealth);

        if (currentTownHealth <= 0)
        {
            Debug.Log("El pueblo fue destruido");
            OnPlayerDied();
        }
    }

    public bool CanBuild()
    {
        TowerSO selectedTower = GetSelectedTowerSO();
        return selectedTower != null && towerSOList.Length > 0 && goldManager != null && goldManager.currentGold >= selectedTower.cost;
    }
    #endregion

    #region Inicialización de Managers
    private void InitializeManagers()
    {
        Debug.Log("Inicializando Managers...");

        if (abilityManager != null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                abilityManager.InitializeAbilitySystems(player);
                Debug.Log("AbilityManager inicializado");
            }
            else
            {
                Debug.LogError("No se encontró GameObject con tag 'Player'");
            }
        }

        if (abilitySystem != null && abilitySystem.abilities.Count == 0)
        {
            abilitySystem.Initialize();
            Debug.Log("AbilitySystem inicializado desde GameManager");
        }

        InitializeShopSystems();

        if (waveManager != null)
        {
            Debug.Log("WaveManager referenciado en GameManager");
        }

        Debug.Log("Todos los managers inicializados");
    }

    private void InitializeShopSystems()
    {
        Debug.Log("Inicializando sistema de tienda...");

        if (itemManager != null)
        {
            if (HasInitializeMethod(itemManager))
            {
                itemManager.Initialize();
                Debug.Log("ItemManager inicializado");
            }
            else
            {
                Debug.Log("ItemManager referenciado");
            }
        }

        if (inventoryManager != null)
        {
            if (HasInitializeMethod(inventoryManager))
            {
                inventoryManager.Initialize();
                Debug.Log("InventoryManager inicializado");
            }
            else
            {
                Debug.Log("InventoryManager referenciado");
            }
        }

        if (shopManager != null)
        {
            shopManager.itemManager = itemManager;
            shopManager.goldManager = goldManager;
            shopManager.inventoryManager = inventoryManager;
            shopManager.playerStats = playerStats;

            if (HasInitializeMethod(shopManager))
            {
                shopManager.Initialize();
                Debug.Log("ShopManager inicializado con todas las dependencias");
            }
            else
            {
                Debug.Log("ShopManager configurado con dependencias");
            }
        }

        if (goldManager != null)
        {
            Debug.Log($"Oro inicial: {goldManager.currentGold}");
        }

        Debug.Log("Sistema de tienda completamente inicializado");
    }

    private bool HasInitializeMethod(object obj)
    {
        if (obj == null) return false;
        return obj.GetType().GetMethod("Initialize") != null;
    }
    #endregion

    #region Sistema de Tienda
    public void ToggleShop()
    {
        if (shopManager != null)
        {
            shopManager.ToggleShop();
        }
        else
        {
            Debug.LogWarning("ShopManager no disponible");
        }
    }

    public bool IsShopOpen()
    {
        return shopManager != null && shopManager.IsShopOpen();
    }

    public void PurchaseItem(ItemData item)
    {
        if (shopManager != null && item != null)
        {
            shopManager.PurchaseItem(item);
        }
        else
        {
            Debug.LogWarning("No se puede comprar: ShopManager o Item nulo");
        }
    }

    public bool CanPurchaseItem(ItemData item)
    {
        if (item == null) return false;

        bool canPurchaseFromItemManager = itemManager != null && itemManager.CanPurchaseItem(item);
        bool hasEnoughGold = goldManager != null && goldManager.currentGold >= item.cost;
        bool notPurchased = !item.isPurchased;

        bool finalResult = canPurchaseFromItemManager && hasEnoughGold && notPurchased;

        Debug.Log($"CanPurchase {item.itemName}: " +
                 $"ItemManager={canPurchaseFromItemManager}, " +
                 $"Gold={hasEnoughGold}, " +
                 $"NotPurchased={notPurchased} → {finalResult}");

        return finalResult;
    }
    #endregion

    #region Utilidades Públicas
    public void ResetGameStats()
    {
        enemigosDerrotados = 0;
        monedasTotales = 0;

        if (itemManager != null)
        {
            itemManager.ResetAllItems();
        }

        if (inventoryManager != null)
        {
            inventoryManager.ClearInventory();
        }

        if (goldManager != null)
        {
            goldManager.ResetGold();
        }

        Debug.Log("Estadísticas del juego reiniciadas");
    }

    public int GetEnemiesDefeated() => enemigosDerrotados;
    public int GetTotalGold() => monedasTotales;

    public void AddGold(int amount)
    {
        monedasTotales += amount;
        if (goldManager != null)
        {
            goldManager.AddGold(amount);
        }
    }

    public bool SpendGold(int amount)
    {
        if (goldManager != null && goldManager.SpendGold(amount))
        {
            monedasTotales -= amount;
            return true;
        }
        return false;
    }
    #endregion

    #region Getters Públicos para otros sistemas
    public ShopManager GetShopManager() => shopManager;
    public ItemManager GetItemManager() => itemManager;
    public InventoryManager GetInventoryManager() => inventoryManager;
    public GoldManager GetGoldManager() => goldManager;
    public PlayerStats GetPlayerStats() => playerStats;
    #endregion
}
