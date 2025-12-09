//using UnityEngine;
//using UnityEngine.AI;
//using System.Collections.Generic;

//[RequireComponent(typeof(NavMeshAgent))]
//public class PlayerMovement : MonoBehaviour
//{
//    [Header("Movimiento")]
//    public float stopDistance = 0.2f;
//    public int maxQueuePoints = 3;

//    [HideInInspector] public NavMeshAgent agent;
//    private Queue<Vector3> moveQueue = new Queue<Vector3>();

//    public Vector3 CurrentPosition => transform.position;
//    public Vector3 CurrentVelocity => agent != null ? agent.velocity : Vector3.zero;

//    void Awake()
//    {
//        agent = GetComponent<NavMeshAgent>();
//        if (!agent.isOnNavMesh) Debug.LogWarning("PlayerMovement: Agent no está en NavMesh");
//    }

//    void Update()
//    {
//        ProcessMovementQueue();
//    }

//    private void ProcessMovementQueue()
//    {
//        if (agent == null || !agent.isOnNavMesh) return;

//        if (moveQueue.Count == 0)
//        {
//            if (agent.hasPath) agent.ResetPath();
//            return;
//        }

//        Vector3 nextPoint = moveQueue.Peek();
//        if (!agent.hasPath || Vector3.Distance(agent.destination, nextPoint) > 0.1f || agent.pathStatus == NavMeshPathStatus.PathInvalid)
//        {
//            agent.isStopped = false;
//            agent.SetDestination(nextPoint);
//        }

//        if (!agent.pathPending && agent.hasPath && agent.remainingDistance <= stopDistance)
//        {
//            moveQueue.Dequeue();
//            if (moveQueue.Count > 0) agent.SetDestination(moveQueue.Peek());
//            else agent.isStopped = true;
//        }
//    }

//    // Movimiento inmediato: Right Click
//    public void MoveTo(Vector3 position)
//    {
//        Debug.Log("[PlayerMovement] MoveTo called: " + position);
//        moveQueue.Clear();
//        if (agent == null || !agent.isOnNavMesh)
//        {
//            Debug.LogWarning("[PlayerMovement] Agent nulo o no está en NavMesh");
//            return;
//        }

//        agent.isStopped = false;
//        agent.ResetPath();
//        agent.SetDestination(position);
//    }

//    // Movimiento encolado: Middle Click
//    public void EnqueueMovePosition(Vector3 position)
//    {
//        Debug.Log("[PlayerMovement] EnqueueMovePosition called: " + position);
//        if (agent == null || !agent.isOnNavMesh) return;
//        if (moveQueue.Count >= maxQueuePoints) return;

//        moveQueue.Enqueue(position);
//        if (!agent.hasPath && !agent.pathPending)
//        {
//            agent.isStopped = false;
//            agent.SetDestination(moveQueue.Peek());
//        }
//    }

//    public void StopMovement()
//    {
//        Debug.Log("[PlayerMovement] StopMovement called");
//        moveQueue.Clear();
//        if (agent != null)
//        {
//            agent.isStopped = true;
//            agent.ResetPath();
//        }
//    }

//    public bool IsMoving()
//    {
//        return agent != null && agent.velocity.sqrMagnitude > 0.001f;
//    }
//}
