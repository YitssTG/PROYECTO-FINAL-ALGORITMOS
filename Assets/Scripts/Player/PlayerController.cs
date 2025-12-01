using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private PlayerAutoAttack autoAttack;
    private AbilityAimingSystem aiming;
    private PlayerStats playerStats;

    void Awake()
    {
        autoAttack = GetComponent<PlayerAutoAttack>();
        aiming = GetComponent<AbilityAimingSystem>();
        playerStats = GetComponent<PlayerStats>();

        // Verificaciones
        if (autoAttack == null) Debug.LogError("❌ PlayerAutoAttack no encontrado");
    }

    void Update()
    {
        // Debug input para testing
        if (Keyboard.current.kKey.wasPressedThisFrame && playerStats != null)
        {
            playerStats.AddExperience(50);
        }
    }

    public void OnRightClick(InputAction.CallbackContext context)
    {
        if (!context.performed || !Mouse.current.rightButton.wasPressedThisFrame)
            return;

        // No procesar click si está apuntando habilidad
        if (aiming != null && aiming.IsAiming && Keyboard.current.shiftKey.isPressed)
            return;

        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // Intentar atacar enemigo
            EnemyBase enemy = hit.collider.GetComponent<EnemyBase>();
            if (enemy != null)
            {
                autoAttack.SetAttackTarget(enemy);
                return;
            }

            // Mover a posición del suelo
            autoAttack.MoveToPosition(hit.point);
        }
    }

    // ⭐ MÉTODOS PÚBLICOS para otros sistemas
    public void CommandMoveTo(Vector3 position)
    {
        autoAttack.MoveToPosition(position);
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