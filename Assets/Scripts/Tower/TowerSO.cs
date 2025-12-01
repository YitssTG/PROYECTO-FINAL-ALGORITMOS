using UnityEngine;

[CreateAssetMenu(fileName = "New Tower", menuName = "Tower Defense/Tower")]
public class TowerSO : ScriptableObject
{
    public string towerName;
    public int cost;
    public float damage;
    public float range;
    public GameObject towerPrefab;
}
