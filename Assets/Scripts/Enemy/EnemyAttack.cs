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
        {
            TryAttack(mov.target); // Atacamos directamente al jugador
        }
    }

    void TryAttack(Transform attackTarget)
    {
        if (Time.time < nextAttack) return;
        nextAttack = Time.time + attackCooldown;

        if (isRanged)
            Shoot(attackTarget);
        else
            ApplyMeleeDamage(attackTarget);
    }

    void ApplyMeleeDamage(Transform attackTarget)
    {
        if (attackTarget.CompareTag("Player"))
        {
            GameManager.Instance.playerStats.TakeDamage(enemy.CurrentDamage);
            Debug.Log($"{name} aplicó {enemy.CurrentDamage} daño al jugador");
        }
    }

    void Shoot(Transform attackTarget)
    {
        if (projectilePrefab == null || firePoint == null) return;

        Vector3 dir = (attackTarget.position - firePoint.position).normalized;
        firePoint.rotation = Quaternion.LookRotation(dir);

        GameObject bullet = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        EnemyProjectile proj = bullet.GetComponent<EnemyProjectile>();

        if (proj != null)
        {
            proj.damage = enemy.CurrentDamage;
            proj.speed = projectileSpeed;
            proj.SetTarget(attackTarget);
            Debug.Log($"{name} disparó un proyectil al jugador");
        }
    }
}
