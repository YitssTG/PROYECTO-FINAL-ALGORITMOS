using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class PlayerAutoAttack : MonoBehaviour, IAttacker, IMovable
{
    [Header("Configuración de Combate")]
    public float attackRange = 2f;
    public float attackCooldown = 1f;

    [Header("Movimiento")]
    public float stopDistance = 0.2f;

    [Header("Referencias")]
    public NavMeshAgent agent;
    public Camera cam;

    private PlayerStats stats;
    private EnemyBase currentTarget;
    private float nextAttackTime = 0f;
    public int maxQueuePoints = 3;

    public Vector3 CurrentVelocity { get; private set; } = Vector3.zero;

    private Queue<Vector3> moveQueue = new Queue<Vector3>();

    // === IMovable Properties ===
    public Vector3 CurrentPosition => transform.position;

    void Awake()
    {
        stats = GetComponent<PlayerStats>();
        agent = GetComponent<NavMeshAgent>();
        if (cam == null) cam = Camera.main;
    }

    void Update()
    {
        UpdateAgentSpeed();

        if (currentTarget == null || !IsTargetValid())
            ProcessMovementQueue();
        else
            HandleCombat();
    }

    private void UpdateAgentSpeed()
    {
        if (stats != null && agent != null && agent.speed != stats.CurrentSpeed)
            agent.speed = stats.CurrentSpeed;
    }

    private void ProcessMovementQueue()
    {
        if (agent == null || !agent.isOnNavMesh) return;

        if (moveQueue.Count == 0)
        {
            if (agent.hasPath) agent.ResetPath();
            return;
        }

        Vector3 nextPoint = moveQueue.Peek();
        if (!agent.hasPath || Vector3.Distance(agent.destination, nextPoint) > 0.1f || agent.pathStatus == NavMeshPathStatus.PathInvalid)
        {
            agent.isStopped = false;
            agent.SetDestination(nextPoint);
        }

        if (!agent.pathPending && agent.hasPath && agent.remainingDistance <= stopDistance)
        {
            moveQueue.Dequeue();
            if (moveQueue.Count > 0) agent.SetDestination(moveQueue.Peek());
            else agent.isStopped = true;
        }
    }

    // === IMovable Implementation ===
    public void MoveTo(Vector3 position)
    {
        moveQueue.Clear();
        moveQueue.Enqueue(position);
        agent.isStopped = false;
        agent.SetDestination(position);
    }

    public void StopMovement()
    {
        moveQueue.Clear();
        if (agent != null)
        {
            agent.isStopped = true;
            agent.ResetPath();
        }
    }

    public bool IsMoving()
    {
        return agent != null && agent.velocity.sqrMagnitude > 0.01f;
    }

    // === Wrapper para compatibilidad con PlayerController ===
    public void MoveToPosition(Vector3 position)
    {
        MoveTo(position);
    }

    public void EnqueueMovePosition(Vector3 position)
    {
        if (agent == null || !agent.isOnNavMesh) return;
        ClearTarget();
        if (moveQueue.Count >= maxQueuePoints) return;
        moveQueue.Enqueue(position);
        if (!agent.hasPath && !agent.pathPending) agent.SetDestination(moveQueue.Peek());
    }

    // === Combate existente ===
    private void HandleCombat()
    {
        if (currentTarget == null) return;

        float distanceToTarget = Vector3.Distance(transform.position, currentTarget.transform.position);
        if (distanceToTarget > attackRange) ChaseTarget();
        else AttackTarget();
    }

    private void ChaseTarget()
    {
        moveQueue.Clear();
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
        Vector3 dir = (currentTarget.transform.position - transform.position).normalized;
        dir.y = 0;
        if (dir != Vector3.zero)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 10f);
    }

    private void TryPerformAttack()
    {
        if (Time.time >= nextAttackTime && stats != null && currentTarget != null)
        {
            nextAttackTime = Time.time + attackCooldown;
            int damage = Mathf.RoundToInt(stats.CurrentDamage);
            currentTarget.TakeDamage(damage);
            EventManager.PlayerAttacked(currentTarget, damage);
        }
    }

    #region API Pública
    public void SetAttackTarget(EnemyBase enemy)
    {
        if (!IsTargetValid(enemy)) return;
        moveQueue.Clear();
        currentTarget = enemy;
        agent.isStopped = false;
    }

    public void ClearTarget()
    {
        currentTarget = null;
        if (agent != null) agent.isStopped = false;
    }

    public void StopAllActions()
    {
        ClearTarget();
        moveQueue.Clear();
        if (agent != null) agent.isStopped = true;
    }
    #endregion

    #region Utilidades
    private bool IsTargetValid(EnemyBase enemy = null)
    {
        EnemyBase t = enemy ?? currentTarget;
        return t != null && t.gameObject.activeInHierarchy && t.CurrentHealth > 0;
    }

    public bool HasTarget() => IsTargetValid();
    public EnemyBase GetCurrentTarget() => currentTarget;
    public bool IsAttacking() => HasTarget() && !agent.isStopped;
    public bool IsInAttackRange() => HasTarget() && Vector3.Distance(transform.position, currentTarget.transform.position) <= attackRange;
    #endregion

    #region IAttacker
    public int Damage => stats != null ? Mathf.RoundToInt(stats.CurrentDamage) : 0;
    public float AttackRate => attackCooldown;

    public void Attack(IDamageable target)
    {
        if (target != null && !target.IsDead())
        {
            target.TakeDamage(Damage);
        }
    }
    #endregion
}