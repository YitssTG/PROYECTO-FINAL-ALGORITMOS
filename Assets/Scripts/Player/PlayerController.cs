using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public NavMeshAgent agent;
    public Camera cam;

    [Header("Settings")]
    [SerializeField] private float stoppingDistance = 0.5f;

    [Header("Click Indicator")]
    public GameObject clickIndicatorPrefab;

    private PlayerStats playerStats;
    private AbilityAimingSystem aiming;   // ← referencia al sistema de apuntado

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        cam = Camera.main;
        playerStats = GameManager.Instance.playerStats;

        aiming = GetComponent<AbilityAimingSystem>();
    }

    void Update()
    {
        if (Keyboard.current.kKey.wasPressedThisFrame)
        {
            if (playerStats != null)
                playerStats.AddExperience(50);
        }
    }

    public void OnRightClick(InputAction.CallbackContext context)
    {
        if (!context.performed || !Mouse.current.rightButton.wasPressedThisFrame)
            return;

        // ──────────────────────────────────────────────
        // BLOQUEAR MOVIMIENTO SI:
        //  - NO estamos apuntando
        //  - Y SHIFT está presionado
        // ──────────────────────────────────────────────
        if (aiming != null && !aiming.IsAiming && Keyboard.current.shiftKey.isPressed)
        {
            // Estás manteniendo Shift para entrar a modo apuntado → no se mueve
            return;
        }

        // A PARTIR DE AQUÍ: movimiento normal (sirve tanto si estabas apuntando
        // y cancelas con click derecho, como si no estabas apuntando).

        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (clickIndicatorPrefab != null)
            {
                Instantiate(clickIndicatorPrefab, hit.point + Vector3.up * 0.05f, Quaternion.identity);
            }

            EnemyBase enemy = hit.collider.GetComponent<EnemyBase>();
            PlayerAutoAttack attack = GetComponent<PlayerAutoAttack>();

            if (enemy != null)
            {
                attack.SetAttackTarget(enemy);
                return;
            }

            // click en suelo
            attack.ClearTarget();
            agent.stoppingDistance = stoppingDistance;
            agent.SetDestination(hit.point);
        }
    }

    public bool IsMoving()
    {
        return agent.hasPath && agent.remainingDistance > agent.stoppingDistance;
    }
}
