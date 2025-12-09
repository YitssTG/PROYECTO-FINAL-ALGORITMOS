using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMovement : MonoBehaviour, IMovable
{
    [HideInInspector] public Transform target;
    [HideInInspector] public float detectionRadius = 20f;
    [HideInInspector] public float attackRadius = 15f;
    [HideInInspector] public float speed = 3.5f;

    private NavMeshAgent agent;
    private Vector3 walkDestination;
    private bool hasWalkDestination = false;

    private enum State { Idle, Walking, Chasing, Attacking }
    private State currentState = State.Idle;

    // === IMovable Properties ===
    public Vector3 CurrentPosition => transform.position;
    public Vector3 CurrentVelocity => agent != null ? agent.velocity : Vector3.zero;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed;
        agent.stoppingDistance = 0;
    }

    void Update()
    {
        if (target == null && hasWalkDestination) WalkToDestination();
        else if (target == null) agent.isStopped = true;

        if (target != null)
        {
            float dist = Vector3.Distance(transform.position, target.position);
            if (dist <= attackRadius) currentState = State.Attacking;
            else if (dist <= detectionRadius) currentState = State.Chasing;
            else if (hasWalkDestination) currentState = State.Walking;
            else currentState = State.Idle;
        }

        switch (currentState)
        {
            case State.Chasing:
                agent.isStopped = false;
                agent.SetDestination(target.position);
                break;
            case State.Attacking:
                agent.isStopped = true;
                break;
            case State.Walking:
                WalkToDestination();
                break;
            default:
                agent.isStopped = true;
                break;
        }
    }

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

    public void Die() => Destroy(gameObject);

    public void MoveTo(Vector3 position)
    {
        SetWalkDestination(position);
    }

    public void StopMovement()
    {
        hasWalkDestination = false;
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
}
