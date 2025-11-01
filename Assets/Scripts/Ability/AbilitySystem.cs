using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public enum AbilityType
{
    None,
    PrimaryAb,
    SecondaryAb,
    ThirdAb,
    Ultimate
}

public class AbilitySystem : MonoBehaviour
{
    [Header("Referencia a la base de datos de habilidades")]
    public AbilityDatabase abilityDatabase;

    public Dictionary<AbilityType, Ability> abilities = new();
    private PlayerStats playerStats;

    private bool isUpgrading = false;
    private bool isCtrlPressed = false;

    void Start()
    {
        playerStats = GameManager.Instance?.playerStats;

        if (playerStats == null)
        {
            Debug.LogError("PlayerStats no está asignado en AbilitySystem.");
            return;
        }

        // Asignar habilidades desde el Database
        abilities[AbilityType.PrimaryAb] = abilityDatabase.GetByType(AbilityType.PrimaryAb);
        abilities[AbilityType.SecondaryAb] = abilityDatabase.GetByType(AbilityType.SecondaryAb);
        abilities[AbilityType.ThirdAb] = abilityDatabase.GetByType(AbilityType.ThirdAb);
        abilities[AbilityType.Ultimate] = abilityDatabase.GetByType(AbilityType.Ultimate);

        Debug.Log("[Habilidad] Habilidades inicializadas desde Database.");
    }

    public void OnCtrlPressed(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            isCtrlPressed = true;
            Debug.Log("[CTRL] Presionado.");
        }
        else if (ctx.canceled)
        {
            isCtrlPressed = false;
            Debug.Log("[CTRL] Liberado.");
        }
    }

    public void OnAbilityQ(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && !isCtrlPressed && !isUpgrading)
            TryCast(AbilityType.PrimaryAb);
    }

    public void OnAbilityW(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && !isCtrlPressed && !isUpgrading)
            TryCast(AbilityType.SecondaryAb);
    }

    public void OnAbilityE(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && !isCtrlPressed && !isUpgrading)
            TryCast(AbilityType.ThirdAb);
    }

    public void OnAbilityR(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && !isCtrlPressed && !isUpgrading)
            TryCast(AbilityType.Ultimate);
    }

    public void OnUpgradeQ(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && isCtrlPressed)
        {
            isUpgrading = true;
            playerStats?.SpendSkillPoint(AbilityType.PrimaryAb);
            isUpgrading = false;
        }
    }

    public void OnUpgradeW(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && isCtrlPressed)
        {
            isUpgrading = true;
            playerStats?.SpendSkillPoint(AbilityType.SecondaryAb);
            isUpgrading = false;
        }
    }

    public void OnUpgradeE(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && isCtrlPressed)
        {
            isUpgrading = true;
            playerStats?.SpendSkillPoint(AbilityType.ThirdAb);
            isUpgrading = false;
        }
    }

    public void OnUpgradeR(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && isCtrlPressed)
        {
            isUpgrading = true;
            playerStats?.SpendSkillPoint(AbilityType.Ultimate);
            isUpgrading = false;
        }
    }

    private void TryCast(AbilityType type)
    {
        if (abilities.ContainsKey(type))
            abilities[type].Cast();
    }

    public void UpgradeAbility(AbilityType type)
    {
        if (abilities.ContainsKey(type))
            abilities[type].Upgrade();
    }

    public bool TryUpgradeAbility(AbilityType type, int playerLevel)
    {
        if (!abilities.ContainsKey(type)) return false;

        Ability ab = abilities[type];

        if (type == AbilityType.Ultimate && playerLevel < 5)
        {
            Debug.Log("[Mejora] La Ultimate se desbloquea en nivel 5.");
            return false;
        }

        if (!ab.locked && ab.level >= ab.maxLevel)
        {
            Debug.Log("[Mejora] Ya está al máximo nivel.");
            return false;
        }

        ab.Upgrade();
        return true;
    }
}
