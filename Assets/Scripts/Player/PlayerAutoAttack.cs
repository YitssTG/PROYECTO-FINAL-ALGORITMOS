using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class PlayerAutoAttack : MonoBehaviour
{
    public float attackRange = 2f;
    public float attackCooldown = 1f;

    private PlayerStats stats;
    private NavMeshAgent agent;
    private EnemyBase currentTarget;
    private float nextAttackTime = 0f;

    void Awake()
    {
        stats = GetComponent<PlayerStats>();
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (currentTarget == null) return;

        float dist = Vector3.Distance(transform.position, currentTarget.transform.position);

        if (dist > attackRange)
        {
            // seguir moviendo hacia target
            agent.isStopped = false;
            agent.SetDestination(currentTarget.transform.position);
        }
        else
        {
            // dentro del rango, detenerse y atacar
            agent.isStopped = true;
            TryAttack();
        }
    }

    void TryAttack()
    {
        if (Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + attackCooldown;

            // daño al enemigo
            currentTarget.TakeDamage(Mathf.RoundToInt(stats.damage));
            Debug.Log($"ATACASTE → {currentTarget.enemyName} por {stats.damage} daño");
        }
    }

    // ESTE MÉTODO LO LLAMA TU PlayerController cuando haces click derecho
    public void SetAttackTarget(EnemyBase enemy)
    {
        currentTarget = enemy;
        if (enemy != null)
        {
            agent.isStopped = false;
            agent.SetDestination(enemy.transform.position);
        }
    }

    public void ClearTarget()
    {
        currentTarget = null;
        agent.isStopped = false;
    }
}
