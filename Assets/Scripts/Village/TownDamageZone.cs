using System.Collections.Generic;
using UnityEngine;

public class TownDamageZone : MonoBehaviour
{
    public float damagePerSecond = 5f;
    private Collider zoneCollider;
    private List<GameObject> enemiesInside = new List<GameObject>();

    private void Awake()
    {
        zoneCollider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (!enemiesInside.Contains(other.gameObject))
                enemiesInside.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (enemiesInside.Contains(other.gameObject))
                enemiesInside.Remove(other.gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // Ignorar colisión con Player para que pase libre
        if (other.CompareTag("Player") && zoneCollider != null)
        {
            Physics.IgnoreCollision(other, zoneCollider, true);
        }
    }

    private void Update()
    {
        if (enemiesInside.Count == 0) return;
        if (GameManager.Instance == null) return;

        GameManager.Instance.DamageVillage(damagePerSecond * Time.deltaTime);
    }
}
