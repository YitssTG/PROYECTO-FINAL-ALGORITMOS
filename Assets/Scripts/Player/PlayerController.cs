using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private PlayerAutoAttack autoAttack;
    private AbilityAimingSystem aiming;
    private PlayerStats playerStats;

    public PlayerInput input; 

    private void Awake()
    {
        autoAttack = GetComponent<PlayerAutoAttack>();
        aiming = GetComponent<AbilityAimingSystem>();
        playerStats = GetComponent<PlayerStats>();
    }

    private void Update()
    {
        if (Keyboard.current.kKey.wasPressedThisFrame && playerStats != null)
            playerStats.AddExperience(50);
    }

    public void OnRightClick(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (!Physics.Raycast(ray, out RaycastHit hit))
            return;

        EnemyBase enemy = hit.collider.GetComponent<EnemyBase>();
        if (enemy != null)
        {
            autoAttack.SetAttackTarget(enemy);
            return;
        }

        autoAttack.StopAllActions();
        autoAttack.MoveToPosition(hit.point);
    }

    public void OnMiddleClick(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (!Physics.Raycast(ray, out RaycastHit hit))
            return;

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
