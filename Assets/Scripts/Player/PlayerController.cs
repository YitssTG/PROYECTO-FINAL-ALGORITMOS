using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    [Header("Modelo 3D")]
    [SerializeField] private Transform model3D; // Tu modelo importado

    private PlayerAutoAttack autoAttack;
    private AbilityAimingSystem aiming;
    private PlayerStats playerStats;

    public PlayerInput input;

    [Header("Indicador de destino")]
    public GameObject indicatorPrefab;

    [Header("FreeMode")]
    public bool freeMode = false;

    [Header("Rotación suave")]
    public float rotationSpeed = 720f;
    public Vector3 modelRotationOffset = new Vector3(0, 90, 0);

    private void Awake()
    {
        autoAttack = GetComponent<PlayerAutoAttack>();
        aiming = GetComponent<AbilityAimingSystem>();
        playerStats = GetComponent<PlayerStats>();
    }

    private void Update()
    {
        // Rotación suave siguiendo la dirección del NavMeshAgent
        if (autoAttack.agent != null && model3D != null)
        {
            Vector3 velocity = autoAttack.agent.velocity;
            velocity.y = 0;

            if (velocity.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(velocity);
                targetRotation *= Quaternion.Euler(modelRotationOffset);
                model3D.rotation = Quaternion.RotateTowards(model3D.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
            freeMode = !freeMode;

        if (Keyboard.current.kKey.wasPressedThisFrame && playerStats != null)
            playerStats.AddExperience(50);
    }

    public void OnRightClick(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (Mouse.current.rightButton.wasPressedThisFrame == false) return;

        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (!Physics.Raycast(ray, out RaycastHit hit)) return;

        // Spawn indicador
        if (indicatorPrefab != null)
            Instantiate(indicatorPrefab, hit.point, Quaternion.identity);

        // Si clic en enemigo
        EnemyBase enemy = hit.collider.GetComponent<EnemyBase>();
        if (enemy != null)
        {
            autoAttack.SetAttackTarget(enemy);
            return;
        }

        // Movimiento directo con rotación siguiendo el path
        autoAttack.StopAllActions();
        autoAttack.MoveToPosition(hit.point);
    }

    public void OnMiddleClick(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (!Physics.Raycast(ray, out RaycastHit hit)) return;

        if (indicatorPrefab != null)
            Instantiate(indicatorPrefab, hit.point, Quaternion.identity);

        // Encolar movimiento y rotación seguirá automáticamente por NavMeshAgent
        autoAttack.EnqueueMovePosition(hit.point);
    }

    public void CommandMoveTo(Vector3 position)
    {
        autoAttack.EnqueueMovePosition(position);
    }

    public void CommandAttack(EnemyBase enemy)
    {
        autoAttack.SetAttackTarget(enemy);
    }

    public void CommandStop()
    {
        autoAttack.StopAllActions();
    }

    public bool IsBusy() => autoAttack.HasTarget() || autoAttack.IsAttacking();
}