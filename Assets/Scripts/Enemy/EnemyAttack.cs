using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public float attackCooldown = 1f;
    private float nextAttack = 0f;

    private EnemyBase baseEnemy;
    private EnemyMovement mov;

    void Awake()
    {
        baseEnemy = GetComponent<EnemyBase>();
        mov = GetComponent<EnemyMovement>();
    }

    void Update()
    {
        if (mov == null || baseEnemy == null) return;
        if (mov.target == null) return;

        float dist = Vector3.Distance(transform.position, mov.target.position);

        // está dentro del attackRadius
        if (dist <= mov.attackRadius)
        {
            TryAttack();
        }
    }

    void TryAttack()
    {
        if (Time.time < nextAttack) return;
        nextAttack = Time.time + attackCooldown;

        // interactúa con PlayerStats
        GameManager.Instance.playerStats.TakeDamage(baseEnemy.damage);

        baseEnemy.Attack(); // llama el mensaje override tipo "Melee ataca"
    }
}
