using System.Collections.Generic;
using UnityEngine;

public class TowerTargetingQuickSort : TowerBase
{
    public GameObject bulletPrefab;
    public Transform firePoint;

    protected override EnemyBase GetTarget()
    {
        if (data == null) return null; // <--- PREVENCIÓN

        Collider[] hits = Physics.OverlapSphere(transform.position, AttackRange);
        List<EnemyBase> enemies = new List<EnemyBase>();
        List<int> healthValues = new List<int>();

        foreach (var hit in hits)
        {
            EnemyBase enemy = hit.GetComponent<EnemyBase>();
            if (enemy != null)
            {
                enemies.Add(enemy);
                healthValues.Add(enemy.CurrentHealth);
            }
        }

        if (healthValues.Count == 0) return null;

        QuickSortUtil.QuickSort(healthValues, 0, healthValues.Count - 1);

        int lowest = healthValues[0];
        foreach (var enemy in enemies)
            if (enemy.CurrentHealth == lowest)
                return enemy;

        return null;
    }

    public override void Shoot(EnemyBase enemy)
    {
        GameObject bulletObj =
            Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Bullet b = bulletObj.GetComponent<Bullet>();
        b.SetTarget(enemy, Damage);
    }
}
