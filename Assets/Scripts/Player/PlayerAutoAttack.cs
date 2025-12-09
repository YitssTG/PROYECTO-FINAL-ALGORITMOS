using UnityEngine;
using UnityEngine.AI;

public class PlayerAutoAttack : MonoBehaviour
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

    void Awake()
    {
        stats = GetComponent<PlayerStats>();
        agent = GetComponent<NavMeshAgent>();

        if (cam == null)
            cam = Camera.main;

        if (stats == null) Debug.LogWarning("PlayerStats no encontrado en PlayerAutoAttack");
        if (agent == null) Debug.LogWarning("NavMeshAgent no encontrado en PlayerAutoAttack");
    }

    void Update()
    {
        UpdateAgentSpeed();

        if (currentTarget == null || !IsTargetValid())
        {
            ProcessMovementQueue();
        }
        else
        {
            HandleCombat();
        }
    }

    private void UpdateAgentSpeed()
    {
        if (stats != null && agent != null && agent.speed != stats.CurrentSpeed)
        {
            agent.speed = stats.CurrentSpeed;
        }
    }

    private void ProcessMovementQueue()
    {
        if (agent == null)
            return;

        if (!agent.isOnNavMesh)
        {
            Debug.LogWarning("Agent NO está en NavMesh. moveQueue Count=" + moveQueue.Count);
            return;
        }

        if (moveQueue.Count == 0)
        {
            if (agent.hasPath)
                agent.ResetPath();
            return;
        }

        Vector3 nextPoint = moveQueue.Peek();

        bool destinationDiffers = Vector3.Distance(agent.destination, nextPoint) > 0.1f;
        bool needSet = !agent.hasPath || destinationDiffers || agent.pathStatus == NavMeshPathStatus.PathInvalid;

        if (needSet && !agent.pathPending)
        {
            agent.isStopped = false;
            bool success = agent.SetDestination(nextPoint);
            Debug.Log($"SetDestination solicitado. Punto: {nextPoint} | SetDestination returned: {success} | Cola: {moveQueue.Count}");
        }

        if (!agent.pathPending && agent.hasPath)
        {
            float remaining = agent.remainingDistance;
            if (remaining == Mathf.Infinity || float.IsNaN(remaining))
                remaining = Vector3.Distance(transform.position, agent.destination);

            if (remaining <= stopDistance)
            {
                Vector3 removed = moveQueue.Dequeue();
                Debug.Log($"Punto alcanzado y desencolado: {removed} | Quedan: {moveQueue.Count}");

                if (moveQueue.Count > 0)
                {
                    Vector3 siguiente = moveQueue.Peek();
                    agent.isStopped = false;
                    agent.SetDestination(siguiente);
                    Debug.Log($"Nuevo destino tomado de la cola: {siguiente}");
                }
                else
                {
                    agent.ResetPath();
                    agent.isStopped = true;
                }
            }
        }
    }

    public void EnqueueMovePosition(Vector3 position)
    {
        if (agent == null || !agent.isOnNavMesh) return;

        ClearTarget();

        if (moveQueue.Count >= maxQueuePoints)
        {
            Debug.Log("Límite de puntos alcanzado");
            return;
        }

        moveQueue.Enqueue(position);
        Debug.Log("Encolado: " + position + " Total: " + moveQueue.Count);

        if (!agent.hasPath && !agent.pathPending)
        {
            Vector3 next = moveQueue.Peek();
            agent.isStopped = false;
            agent.SetDestination(next);
        }
    }

    public void MoveToPosition(Vector3 position)
    {
        EnqueueMovePosition(position);
    }

    private void HandleCombat()
    {
        if (currentTarget == null) return;

        float distanceToTarget = Vector3.Distance(transform.position, currentTarget.transform.position);

        if (distanceToTarget > attackRange)
            ChaseTarget();
        else
            AttackTarget();
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
            Debug.Log($"Atacaste a {currentTarget.enemyName} por {damage}");
        }
    }

    #region API Pública
    public void SetAttackTarget(EnemyBase enemy)
    {
        if (!IsTargetValid(enemy)) return;

        moveQueue.Clear();
        currentTarget = enemy;
        agent.isStopped = false;
        Debug.Log("Target fijado: " + enemy.enemyName);
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
        Debug.Log("StopAllActions: cola limpia y agente detenido");
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
}
