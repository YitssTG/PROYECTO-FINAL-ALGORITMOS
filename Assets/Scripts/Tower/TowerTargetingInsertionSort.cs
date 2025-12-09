using System.Collections.Generic;
using UnityEngine;

public class TowerTargetingInsertionSort : TowerBase
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    private Transform player;

    private void Awake()
    {
        player = GameObject.FindWithTag("Player")?.transform;
    }

    protected override EnemyBase GetTarget()
    {
        if (data == null) return null; 
        if (player == null) return null;

        Collider[] hits = Physics.OverlapSphere(transform.position, AttackRange);
        List<EnemyBase> enemies = new List<EnemyBase>();
        List<int> distances = new List<int>();

        foreach (var hit in hits)
        {
            EnemyBase e = hit.GetComponent<EnemyBase>();
            if (e != null)
            {
                enemies.Add(e);
                int dist = Mathf.RoundToInt(Vector3.Distance(player.position, e.transform.position) * 100f);
                distances.Add(dist);
            }
        }

        if (distances.Count == 0) return null;

        InsertionSortUtil.InsertionSort(distances);

        int closest = distances[0];
        foreach (var e in enemies)
        {
            int d = Mathf.RoundToInt(Vector3.Distance(player.position, e.transform.position) * 100f);
            if (d == closest)
                return e;
        }

        return null;
    }

    public override void Shoot(EnemyBase enemy)
    {
        GameObject bulletObj =
            Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        Bullet bullet = bulletObj.GetComponent<Bullet>();
        bullet.SetTarget(enemy, Damage);
    }
}
