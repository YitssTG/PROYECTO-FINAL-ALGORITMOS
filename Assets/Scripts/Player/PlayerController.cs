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
    private AbilityAimingSystem aiming;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        cam = Camera.main;

        // ✅ CORREGIDO: Obtener referencias de manera segura
        playerStats = GetComponent<PlayerStats>();
        aiming = GetComponent<AbilityAimingSystem>();

        // ✅ CORREGIDO: Verificar referencias críticas
        if (playerStats == null)
        {
            Debug.LogError("❌ PlayerStats no encontrado en el player");
        }
    }

    void Update()
    {
        // ✅ CORREGIDO: Verificar antes de usar
        if (Keyboard.current.kKey.wasPressedThisFrame && playerStats != null)
        {
            playerStats.AddExperience(50);
        }
    }

    public void OnRightClick(InputAction.CallbackContext context)
    {
        if (!context.performed || !Mouse.current.rightButton.wasPressedThisFrame)
            return;

        // ✅ CORREGIDO: Verificar aiming antes de usar
        if (aiming != null && !aiming.IsAiming && Keyboard.current.shiftKey.isPressed)
        {
            return;
        }

        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (clickIndicatorPrefab != null)
            {
                Instantiate(clickIndicatorPrefab, hit.point + Vector3.up * 0.05f, Quaternion.identity);
            }

            EnemyBase enemy = hit.collider.GetComponent<EnemyBase>();
            PlayerAutoAttack attack = GetComponent<PlayerAutoAttack>();

            // ✅ CORREGIDO: Verificar componentes antes de usar
            if (enemy != null && attack != null)
            {
                attack.SetAttackTarget(enemy);
                return;
            }

            // click en suelo
            if (attack != null)
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