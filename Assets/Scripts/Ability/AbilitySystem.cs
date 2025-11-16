using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public enum AbilityType
{
    None,
    PrimaryAb,    // Q
    SecondaryAb,  // W
    ThirdAb,      // E
    Ultimate      // R
}

[System.Serializable]
public class AbilityState
{
    public bool unlocked = false;
    public int level = 0;
    public float lastCastTime = -999f;
}

public class AbilitySystem : MonoBehaviour
{
    [Header("Referencia a la base de datos de habilidades")]
    public AbilityDatabase abilityDatabase;

    public Dictionary<AbilityType, Ability> abilities = new();
    public Dictionary<AbilityType, AbilityState> state = new();

    private PlayerStats playerStats;
    private AbilityAimingSystem aimingSystem;

    private bool isCtrlPressed = false;

    void Start()
    {
        playerStats = GameManager.Instance?.playerStats;
        aimingSystem = GetComponent<AbilityAimingSystem>();

        if (playerStats == null)
        {
            Debug.LogError("[AbilitySystem] PlayerStats no asignado.");
            enabled = false;
            return;
        }

        // Cargar habilidades
        abilities[AbilityType.PrimaryAb] = abilityDatabase.GetByType(AbilityType.PrimaryAb);
        abilities[AbilityType.SecondaryAb] = abilityDatabase.GetByType(AbilityType.SecondaryAb);
        abilities[AbilityType.ThirdAb] = abilityDatabase.GetByType(AbilityType.ThirdAb);
        abilities[AbilityType.Ultimate] = abilityDatabase.GetByType(AbilityType.Ultimate);

        // Cargar estados
        foreach (var kvp in abilities)
        {
            AbilityType type = kvp.Key;
            Ability ab = kvp.Value;

            if (ab == null) continue;

            AbilityState st = new AbilityState();
            st.unlocked = !ab.locked;
            st.level = ab.level;

            state[type] = st;
        }
    }

    private bool IsAiming()
    {
        return aimingSystem != null && aimingSystem.IsAiming;
    }

    // ---------------- CTRL PARA MEJORAS ----------------
    public void OnCtrlPressed(InputAction.CallbackContext ctx)
    {
        if (ctx.performed) isCtrlPressed = true;
        if (ctx.canceled) isCtrlPressed = false;
    }

    // ---------------- BLOQUE DE CASTEO NORMAL ----------------
    public void OnAbilityQ(InputAction.CallbackContext ctx)
    {
        if (ctx.performed &&
            !isCtrlPressed &&
            !Keyboard.current.shiftKey.isPressed &&
            !IsAiming())
        {
            TryCast(AbilityType.PrimaryAb);
        }
    }

    public void OnAbilityW(InputAction.CallbackContext ctx)
    {
        if (ctx.performed &&
            !isCtrlPressed &&
            !Keyboard.current.shiftKey.isPressed &&
            !IsAiming())
        {
            TryCast(AbilityType.SecondaryAb);
        }
    }

    public void OnAbilityE(InputAction.CallbackContext ctx)
    {
        if (ctx.performed &&
            !isCtrlPressed &&
            !Keyboard.current.shiftKey.isPressed &&
            !IsAiming())
        {
            TryCast(AbilityType.ThirdAb);
        }
    }

    public void OnAbilityR(InputAction.CallbackContext ctx)
    {
        if (ctx.performed &&
            !isCtrlPressed &&
            !Keyboard.current.shiftKey.isPressed &&
            !IsAiming())
        {
            TryCast(AbilityType.Ultimate);
        }
    }

    // ---------------- BLOQUE DE MEJORA ----------------
    public void OnUpgradeQ(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && isCtrlPressed)
            playerStats?.SpendSkillPoint(AbilityType.PrimaryAb);
    }

    public void OnUpgradeW(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && isCtrlPressed)
            playerStats?.SpendSkillPoint(AbilityType.SecondaryAb);
    }

    public void OnUpgradeE(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && isCtrlPressed)
            playerStats?.SpendSkillPoint(AbilityType.ThirdAb);
    }

    public void OnUpgradeR(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && isCtrlPressed)
            playerStats?.SpendSkillPoint(AbilityType.Ultimate);
    }

    // ---------------- LANZAR HABILIDAD ----------------
    public void TryCast(AbilityType type)
    {
        if (!abilities.ContainsKey(type)) return;

        Ability ab = abilities[type];
        AbilityState st = state[type];

        if (!st.unlocked || st.level <= 0)
        {
            Debug.Log($"[{type}] No desbloqueada");
            return;
        }

        if (Time.time < st.lastCastTime + ab.cooldown)
        {
            return;
        }

        st.lastCastTime = Time.time;
        ab.level = st.level;

        ab.OnCast?.Invoke();
    }

    // ---------------- MEJORAR HABILIDADES ----------------
    public bool TryUpgradeAbility(AbilityType type, int playerLevel)
    {
        if (!abilities.ContainsKey(type)) return false;

        Ability ab = abilities[type];
        AbilityState st = state[type];

        // Regla: Ulti exige nivel base 5
        if (type == AbilityType.Ultimate && playerLevel < 5)
        {
            Debug.Log("[Mejora] Ultimate bloqueada hasta nivel 5 del jugador.");
            return false;
        }

        // Primer desbloqueo
        if (!st.unlocked)
        {
            st.unlocked = true;
            st.level = 1;
            ab.level = 1;
            Debug.Log($"[{ab.abilityName}] Desbloqueada (nivel 1)");
            return true;
        }

        // Ya está al máximo
        if (st.level >= ab.maxLevel)
        {
            Debug.Log($"[{ab.abilityName}] nivel máximo alcanzado.");
            return false;
        }

        // Subir nivel
        st.level++;
        ab.level = st.level;

        Debug.Log($"[{ab.abilityName}] mejorada a nivel {st.level}");
        return true;
    }
}
