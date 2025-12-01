using UnityEngine;

public class BuildSlot : MonoBehaviour
{
    [Header("Slot State")]
    public bool isOccupied = false;
    public GameObject currentTower;

    [Header("Visuals")]
    public Color freeColor = Color.green;
    public Color occupiedColor = Color.red;

    private Renderer rend;

    void Awake()
    {
        rend = GetComponent<Renderer>();
        UpdateColor();
    }

    void OnMouseDown()
    {
        if (isOccupied)
        {
            Debug.Log("❌ El slot ya está ocupado");
            return;  // El slot ya está ocupado, no se puede construir más aquí
        }

        // Siempre intentar construir, pero chequeamos oro dentro
        TowerSO towerSO = GameManager.Instance.GetSelectedTowerSO();
        if (towerSO == null)
        {
            Debug.LogWarning("❌ No hay torre seleccionada");
            return;
        }

        if (GameManager.Instance.SpendGold(towerSO.cost))
        {
            PlaceTower(towerSO);
        }
        else
        {
            Debug.Log($"❌ No tienes suficiente oro para construir {towerSO.name}. Costo: {towerSO.cost}");
        }
    }

    public void PlaceTower(TowerSO towerSO)
    {
        if (towerSO == null) return;
        if (towerSO.towerPrefab == null)
        {
            Debug.LogError($"❌ TowerSO '{towerSO.name}' no tiene prefab asignado");
            return;
        }

        Debug.Log($"Intentando colocar torre: {towerSO.name}");

        currentTower = Instantiate(towerSO.towerPrefab, transform.position, Quaternion.identity);
        isOccupied = true;
        UpdateColor();

        if (TowerManager.Instance != null)
            TowerManager.Instance.RegisterTower(currentTower);

        TowerHealth health = currentTower.GetComponent<TowerHealth>();
        if (health == null)
        {
            health = currentTower.AddComponent<TowerHealth>();
            health.maxHealth = 100;
        }

        Debug.Log($"✅ Torre construida: {currentTower.name}");
    }

    void UpdateColor()
    {
        if (rend != null)
            rend.material.color = isOccupied ? occupiedColor : freeColor;
    }

    public void ClearSlot()
    {
        if (currentTower != null && TowerManager.Instance != null)
        {
            TowerManager.Instance.UnregisterTower(currentTower);
            Debug.Log("🧹 Limpiando slot");
        }

        isOccupied = false;
        currentTower = null;
        UpdateColor();
    }
}
