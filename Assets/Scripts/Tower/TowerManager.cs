using UnityEngine;
using System.Collections.Generic;

public class TowerManager : MonoBehaviour
{
    public static TowerManager Instance { get; private set; }

    private List<GameObject> activeTowers = new List<GameObject>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        Debug.Log("TowerManager inicializado correctamente");
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void RegisterTower(GameObject tower)
    {
        if (!activeTowers.Contains(tower))
        {
            activeTowers.Add(tower);
            Debug.Log($"Torre registrada. Total: {activeTowers.Count}");
        }
    }

    public void UnregisterTower(GameObject tower)
    {
        if (activeTowers.Contains(tower))
        {
            activeTowers.Remove(tower);
            Debug.Log($"Torre removida. Total: {activeTowers.Count}");
        }
    }

    public GameObject GetClosestTower(Vector3 position, float maxRange = Mathf.Infinity)
    {
        if (activeTowers.Count == 0) return null;

        GameObject closestTower = null;
        float closestDistance = maxRange;

        foreach (GameObject tower in activeTowers)
        {
            if (tower == null) continue;
            float distance = Vector3.Distance(position, tower.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTower = tower;
            }
        }
        return closestTower;
    }

    public int GetTowerCount() => activeTowers.Count;
}
