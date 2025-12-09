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

    public int slotTowerIndex = 0;

    void Awake()
    {
        rend = GetComponent<Renderer>();
        UpdateColor();
    }

    void OnMouseDown()
    {
        if (isOccupied)
        {
            Debug.Log("El slot ya está ocupado");
            return;
        }

        TowerSO towerSO = GameManager.Instance.GetTowerByIndex(slotTowerIndex);

        if (towerSO == null)
        {
            Debug.LogWarning("No se encontró TowerSO para este slot");
            return;
        }

        if (GameManager.Instance.SpendGold(towerSO.cost))
        {
            PlaceTower(towerSO);
        }
        else
        {
            Debug.Log($"No tienes suficiente oro para {towerSO.name}. Costo: {towerSO.cost}");
        }
    }

    public void PlaceTower(TowerSO towerSO)
    {
        if (towerSO == null) return;

        if (towerSO.towerPrefab == null)
        {
            Debug.LogError($"TowerSO '{towerSO.name}' no tiene prefab asignado");
            return;
        }

        Debug.Log($"Intentando colocar torre: {towerSO.name}");

        currentTower = Instantiate(towerSO.towerPrefab, transform.position, Quaternion.identity);
        isOccupied = true;
        UpdateColor();

        if (TowerManager.Instance != null)
            TowerManager.Instance.RegisterTower(currentTower);

        Debug.Log($"Torre construida: {currentTower.name}");
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
            Debug.Log("Limpiando slot");
        }

        isOccupied = false;
        currentTower = null;
        UpdateColor();
    }
}
