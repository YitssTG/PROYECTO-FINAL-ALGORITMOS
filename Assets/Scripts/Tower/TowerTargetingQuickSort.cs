using UnityEngine;
using System.Collections.Generic;

public class TowerTargetingQuickSort : MonoBehaviour
{
    [Header("Config")]
    public TowerSO towerData;
    public GameObject bulletPrefab;
    public Transform firePoint;

    private float attackRange;
    private int damage;
    private float fireRate = 1f;
    private float fireCountdown;

    private void Start()
    {
        if (towerData == null) return;

        attackRange = towerData.range;
        damage = (int)towerData.damage;
    }

    void Update()
    {
        fireCountdown -= Time.deltaTime;

        EnemyBase target = GetLowestHealthEnemy();

        if (target != null && fireCountdown <= 0f)
        {
            Shoot(target);
            fireCountdown = fireRate;
        }
    }

    private EnemyBase GetLowestHealthEnemy()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, attackRange);

        List<EnemyBase> enemies = new List<EnemyBase>();
        List<int> healthValues = new List<int>();

        for (int i = 0; i < hits.Length; i++)
        {
            EnemyBase enemy = hits[i].GetComponent<EnemyBase>();
            if (enemy != null && enemy.CurrentHealth > 0)
            {
                enemies.Add(enemy);
                healthValues.Add(enemy.CurrentHealth);
            }
        }

        if (healthValues.Count == 0) return null;

        QuickSortUtil.QuickSort(healthValues, 0, healthValues.Count - 1);

        int lowestHealth = healthValues[0];

        for (int i = 0; i < enemies.Count; i++)
        {
            if (enemies[i].CurrentHealth == lowestHealth)
                return enemies[i];
        }

        return null;
    }

    private void Shoot(EnemyBase enemy)
    {
        if (bulletPrefab == null || firePoint == null) return;

        GameObject bulletObj = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Bullet bullet = bulletObj.GetComponent<Bullet>();
        bullet.SetTarget(enemy, damage);

        Debug.Log($"Disparo a {enemy.EnemyName} con {enemy.CurrentHealth} HP");
    }
}
