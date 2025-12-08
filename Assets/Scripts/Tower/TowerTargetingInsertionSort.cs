using UnityEngine;
using System.Collections.Generic;

public class TowerTargetingInsertionSort : MonoBehaviour
{
    [Header("Config")]
    public TowerSO towerData;
    public GameObject bulletPrefab;
    public Transform firePoint;

    private float attackRange;
    private int damage;
    private float fireRate = 1f;
    private float fireCountdown;

    private Transform player; // referencia al player

    private void Start()
    {
        if (towerData == null)
        {
            Debug.LogError("Falta TowerSO en " + gameObject.name);
            return;
        }

        attackRange = towerData.range;
        damage = (int)towerData.damage;

        player = GameObject.FindWithTag("Player")?.transform;
    }

    void Update()
    {
        fireCountdown -= Time.deltaTime;

        EnemyBase target = GetClosestEnemyUsingInsertion();

        if (target != null && fireCountdown <= 0f)
        {
            Shoot(target);
            fireCountdown = fireRate;
        }
    }

    private EnemyBase GetClosestEnemyUsingInsertion()
    {
        if (player == null) return null;

        Collider[] hits = Physics.OverlapSphere(transform.position, attackRange);

        List<EnemyBase> enemies = new List<EnemyBase>();
        List<int> distances = new List<int>();

        foreach (var hit in hits)
        {
            EnemyBase enemy = hit.GetComponent<EnemyBase>();
            if (enemy != null)
            {
                enemies.Add(enemy);

                int d = Mathf.RoundToInt(
                    Vector3.Distance(player.position, enemy.transform.position) * 100f
                );

                distances.Add(d);
            }
        }

        if (distances.Count == 0) return null;

        // 🔥 usando TU algoritmo de profesor
        InsertionSortUtil.InsertionSort(distances);

        int closestDistance = distances[0];

        // encontrar el enemigo con esa distancia
        foreach (var e in enemies)
        {
            int d = Mathf.RoundToInt(
                Vector3.Distance(player.position, e.transform.position) * 100f
            );

            if (d == closestDistance)
                return e;
        }

        return null;
    }

    private void Shoot(EnemyBase enemy)
    {
        if (bulletPrefab == null || firePoint == null) return;

        GameObject bulletObj = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Bullet bullet = bulletObj.GetComponent<Bullet>();
        bullet.SetTarget(enemy, damage);
    }
}
