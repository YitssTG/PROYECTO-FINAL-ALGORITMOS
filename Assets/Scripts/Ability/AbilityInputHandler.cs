using UnityEngine;
using UnityEngine.InputSystem;

public class AbilityInputHandler : MonoBehaviour
{
    [Header("Referencias")]
    public AbilitySystem abilitySystem;
    public AbilityAimingSystem aimingSystem;

    private bool isCtrlPressed = false;
    private bool isShiftPressed = false;

    // Input para modificadores
    public void OnCtrlPressed(InputAction.CallbackContext ctx)
    {
        if (ctx.performed) isCtrlPressed = true;
        if (ctx.canceled) isCtrlPressed = false;
    }

    public void OnShiftPressed(InputAction.CallbackContext ctx)
    {
        if (ctx.performed) isShiftPressed = true;
        if (ctx.canceled) isShiftPressed = false;
    }

    // CASTEO RÁPIDO (sin modificadores)
    public void OnAbilityQ(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && !isCtrlPressed && !isShiftPressed && !IsAiming())
        {
            abilitySystem?.TryCast(AbilityType.PrimaryAb);
        }
    }

    public void OnAbilityW(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && !isCtrlPressed && !isShiftPressed && !IsAiming())
        {
            abilitySystem?.TryCast(AbilityType.SecondaryAb);
        }
    }

    public void OnAbilityE(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && !isCtrlPressed && !isShiftPressed && !IsAiming())
        {
            abilitySystem?.TryCast(AbilityType.ThirdAb);
        }
    }

    public void OnAbilityR(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && !isCtrlPressed && !isShiftPressed && !IsAiming())
        {
            abilitySystem?.TryCast(AbilityType.Ultimate);
        }
    }

    // CASTEO APUNTADO (Shift + Tecla)
    public void OnAimAbilityQ(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && isShiftPressed && !isCtrlPressed && !IsAiming())
        {
            aimingSystem?.StartAiming(AbilityType.PrimaryAb);
        }
    }

    public void OnAimAbilityW(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && isShiftPressed && !isCtrlPressed && !IsAiming())
        {
            aimingSystem?.StartAiming(AbilityType.SecondaryAb);
        }
    }

    public void OnAimAbilityR(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && isShiftPressed && !isCtrlPressed && !IsAiming())
        {
            aimingSystem?.StartAiming(AbilityType.Ultimate);
        }
    }

    // MEJORA (Ctrl + Tecla)
    public void OnUpgradeQ(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && isCtrlPressed && !isShiftPressed)
        {
            GameManager.Instance?.playerStats?.SpendSkillPoint(AbilityType.PrimaryAb);
        }
    }

    public void OnUpgradeW(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && isCtrlPressed && !isShiftPressed)
        {
            GameManager.Instance?.playerStats?.SpendSkillPoint(AbilityType.SecondaryAb);
        }
    }

    public void OnUpgradeE(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && isCtrlPressed && !isShiftPressed)
        {
            GameManager.Instance?.playerStats?.SpendSkillPoint(AbilityType.ThirdAb);
        }
    }

    public void OnUpgradeR(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && isCtrlPressed && !isShiftPressed)
        {
            GameManager.Instance?.playerStats?.SpendSkillPoint(AbilityType.Ultimate);
        }
    }

    private bool IsAiming()
    {
        return aimingSystem != null && aimingSystem.IsAiming;
    }
}