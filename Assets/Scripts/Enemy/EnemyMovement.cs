using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMovement : MonoBehaviour
{
    [HideInInspector] public Transform target;

    [HideInInspector] public float detectionRadius = 20f;
    [HideInInspector] public float attackRadius = 15f;
    [HideInInspector] public float speed = 3.5f;

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

    public void Die()
    {
        Destroy(gameObject);
    }
}
