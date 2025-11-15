using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMovement : MonoBehaviour
{
    public Transform target;

    [Header("Rangos y Velocidad")]
    public float detectionRadius = 20f;
    public float attackRadius = 15f;
    public float speed = 3.5f;

    private NavMeshAgent agent;

    private enum State { Idle, Walking, Chasing, Attacking }
    private State currentState = State.Idle;

    private Vector3 walkDestination;
    private bool hasWalkDestination = false;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed;
        agent.stoppingDistance = 0;
    }

    void Update()
    {
        // Si tiene destino pero no target → caminar
        if (target == null && hasWalkDestination)
        {
            WalkToDestination();
            return;
        }

        if (target == null)
        {
            agent.isStopped = true;
            return;
        }

        float dist = Vector3.Distance(transform.position, target.position);

        if (dist <= attackRadius)
            currentState = State.Attacking;
        else if (dist <= detectionRadius)
            currentState = State.Chasing;
        else if (hasWalkDestination)
            currentState = State.Walking;
        else
            currentState = State.Idle;

        switch (currentState)
        {
            case State.Chasing:
                agent.isStopped = false;
                agent.SetDestination(target.position);
                break;

            case State.Attacking:
                agent.isStopped = true;
                LookAtTarget();
                break;

            case State.Walking:
                WalkToDestination();
                break;

            default:
                agent.isStopped = true;
                break;
        }
    }

    private void LookAtTarget()
    {
        if (target == null) return;

        Vector3 dir = (target.position - transform.position).normalized;
        dir.y = 0;

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            Quaternion.LookRotation(dir),
            Time.deltaTime * 8f
        );
    }

    // ⭐⭐ ESTE ES EL MÉTODO QUE TU SPAWNER LLAMA ⭐⭐
    public void SetWalkDestination(Vector3 destination)
    {
        walkDestination = destination;
        hasWalkDestination = true;

        agent.isStopped = false;
        agent.SetDestination(walkDestination);
    }

    private void WalkToDestination()
    {
        if (!hasWalkDestination) return;

        agent.isStopped = false;
        agent.SetDestination(walkDestination);
    }

    // ⭐⭐ ESTE ES movement.Die() QUE LLAMA EnemyBase.Die() ⭐⭐
    public void Die()
    {
        Destroy(gameObject);
    }

    // ⭐ GIZMOS
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);

        if (hasWalkDestination)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, walkDestination);
        }
    }
}
