using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public float attackCooldown = 1f;
    private float nextAttack = 0f;

    [Header("Ranged")]
    public bool isRanged = false;
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float projectileSpeed = 20f;

    private EnemyBase enemy;
    private EnemyMovement mov;

    void Awake()
    {
        enemy = GetComponent<EnemyBase>();
        mov = GetComponent<EnemyMovement>();
    }

    void Update()
    {
        if (mov == null || mov.target == null) return;

        float dist = Vector3.Distance(transform.position, mov.target.position);

        if (dist <= mov.attackRadius)
            TryAttack();
    }

    void TryAttack()
    {
        if (Time.time < nextAttack) return;
        nextAttack = Time.time + attackCooldown;

        if (isRanged)
            Shoot();
        else
            GameManager.Instance.playerStats.TakeDamage(enemy.damage);

        enemy.Attack();
    }

    void Shoot()
    {
        if (projectilePrefab == null || firePoint == null) return;

        // mirar al player antes de disparar
        Vector3 dir = (mov.target.position - firePoint.position).normalized;
        firePoint.rotation = Quaternion.LookRotation(dir);

        GameObject bullet = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        EnemyProjectile proj = bullet.GetComponent<EnemyProjectile>();
        proj.damage = enemy.damage;
        proj.speed = projectileSpeed;
    }
}
