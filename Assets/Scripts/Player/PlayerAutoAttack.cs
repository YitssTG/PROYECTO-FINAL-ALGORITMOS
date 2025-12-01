using UnityEngine;
using UnityEngine.AI;

public class PlayerAutoAttack : MonoBehaviour
{
    [Header("Configuración de Combate")]
    public float attackRange = 2f;
    public float attackCooldown = 1f;

    [Header("Referencias")]
    public NavMeshAgent agent;
    public Camera cam;

    private PlayerStats stats;
    private EnemyBase currentTarget;
    private float nextAttackTime = 0f;

    void Awake()
    {
        stats = GetComponent<PlayerStats>();
        agent = GetComponent<NavMeshAgent>();

        if (cam == null)
            cam = Camera.main;

        if (stats == null)
            Debug.LogError("❌ PlayerStats no encontrado en PlayerAutoAttack");
        if (agent == null)
            Debug.LogError("❌ NavMeshAgent no encontrado en PlayerAutoAttack");
    }

    void Update()
    {
        UpdateAgentSpeed();

        if (currentTarget == null || !IsTargetValid())
            return;

        HandleCombat();
    }

    private void UpdateAgentSpeed()
    {
        if (stats != null && agent.speed != stats.CurrentSpeed)
        {
            agent.speed = stats.CurrentSpeed;
        }
    }

    private void HandleCombat()
    {
        float distanceToTarget = Vector3.Distance(transform.position, currentTarget.transform.position);

        if (distanceToTarget > attackRange)
        {
            ChaseTarget();
        }
        else
        {
            AttackTarget();
        }
    }

    private void ChaseTarget()
    {
        agent.isStopped = false;
        agent.SetDestination(currentTarget.transform.position);
    }

    private void AttackTarget()
    {
        agent.isStopped = true;
        LookAtTarget();
        TryPerformAttack();
    }

    private void LookAtTarget()
    {
        if (currentTarget == null) return;

        Vector3 direction = (currentTarget.transform.position - transform.position).normalized;
        direction.y = 0;

        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(direction),
                Time.deltaTime * 10f
            );
        }
    }

    private void TryPerformAttack()
    {
        if (Time.time >= nextAttackTime && stats != null)
        {
            nextAttackTime = Time.time + attackCooldown;

            int damage = Mathf.RoundToInt(stats.CurrentDamage);
            currentTarget.TakeDamage(damage);

            // ⭐ USAR EL NUEVO EVENTO
            EventManager.PlayerAttacked(currentTarget, damage);

            Debug.Log($"⚔️ Atacaste a {currentTarget.enemyName} por {damage} daño");
        }
    }

    #region API Pública
    public void SetAttackTarget(EnemyBase enemy)
    {
        if (!IsTargetValid(enemy)) return;

        currentTarget = enemy;
        agent.isStopped = false;
        Debug.Log($"🎯 Objetivo de ataque: {enemy.enemyName}");
    }

    public void ClearTarget()
    {
        currentTarget = null;
        agent.isStopped = false;
    }

    public void MoveToPosition(Vector3 position)
    {
        // ⭐ AGREGAR ESTA VERIFICACIÓN
        if (agent == null || !agent.isActiveAndEnabled || !agent.isOnNavMesh)
        {
            Debug.LogWarning("⚠️ Player Agent no está listo");
            return;
        }

        ClearTarget();
        agent.isStopped = false; // ⭐ Solo si está activo
        agent.SetDestination(position);
    }

    public void StopAllActions()
    {
        ClearTarget();
        agent.isStopped = true;
    }
    #endregion

    #region Utilidades
    private bool IsTargetValid(EnemyBase enemy = null)
    {
        EnemyBase targetToCheck = enemy ?? currentTarget;
        return targetToCheck != null &&
               targetToCheck.gameObject.activeInHierarchy &&
               targetToCheck.CurrentHealth > 0;
    }

    public bool HasTarget() => IsTargetValid();
    public EnemyBase GetCurrentTarget() => currentTarget;

    // ⭐ MÉTODO QUE FALTABA - Para PlayerController.IsBusy()
    public bool IsAttacking() => HasTarget() && !agent.isStopped;

    // ⭐ MÉTODO ADICIONAL útil
    public bool IsInAttackRange() => HasTarget() &&
        Vector3.Distance(transform.position, currentTarget.transform.position) <= attackRange;
    #endregion

    #region Debug Visual
    private void OnDrawGizmosSelected()
    {
        // Dibujar rango de ataque
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // Dibujar línea hacia el target actual
        if (currentTarget != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, currentTarget.transform.position);
        }
    }
    #endregion
}