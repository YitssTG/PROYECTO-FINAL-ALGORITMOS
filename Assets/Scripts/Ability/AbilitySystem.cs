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
    public Dictionary<AbilityType, Ability> abilities = new Dictionary<AbilityType, Ability>();
    private PlayerStats playerStats;

    private bool isUpgrading = false; // Para bloquear la acción de lanzar mientras mejoramos
    private bool isCtrlPressed = false; // Para saber si el CTRL está presionado

    void Start()
    {
        playerStats = GameManager.Instance?.playerStats; // referencia PlayerStats

        if (playerStats == null)
        {
            Debug.LogError("PlayerStats no está asignado en AbilitySystem.");
            return;
        }

        // Crear habilidades
        Ability q = new Ability("Bola de Fuego", 3f, 5);
        Ability w = new Ability("Escudo", 5f, 5);
        Ability e = new Ability("Dash", 2f, 5);
        Ability r = new Ability("Ulti Explosiva", 10f, 3);

        // Definir las acciones que se realizan al lanzar las habilidades
        q.OnCast = () => Debug.Log("[Habilidad] Bola de fuego lanzada!");
        w.OnCast = () => Debug.Log("[Habilidad] Escudo activado!");
        e.OnCast = () => Debug.Log("[Habilidad] Dash hacia adelante!");
        r.OnCast = () => Debug.Log("[Habilidad] ULTI EXPLOSIVA!");

        abilities[AbilityType.PrimaryAb] = q;
        abilities[AbilityType.SecondaryAb] = w;
        abilities[AbilityType.ThirdAb] = e;
        abilities[AbilityType.Ultimate] = r;

        Debug.Log("[Habilidad] Habilidades inicializadas.");
    }

    // Detecta cuando CTRL está presionado o liberado
    public void OnCtrlPressed(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            isCtrlPressed = true; // CTRL está presionado
            Debug.Log("[CTRL] CTRL presionado.");
        }
        else if (ctx.canceled)
        {
            isCtrlPressed = false; // CTRL está liberado
            Debug.Log("[CTRL] CTRL liberado.");
        }
    }

    // Verifica las teclas para lanzar habilidades solo si CTRL NO está presionado
    public void OnAbilityQ(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && !isCtrlPressed && !isUpgrading) // Solo lanzar si no se está presionando CTRL
        {
            Debug.Log("[Habilidad] Lanzando Bola de Fuego (Q).");
            TryCast(AbilityType.PrimaryAb);
        }
    }

    public void OnAbilityW(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && !isCtrlPressed && !isUpgrading) // Solo lanzar si no se está presionando CTRL
        {
            Debug.Log("[Habilidad] Lanzando Escudo (W).");
            TryCast(AbilityType.SecondaryAb);
        }
    }

    public void OnAbilityE(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && !isCtrlPressed && !isUpgrading) // Solo lanzar si no se está presionando CTRL
        {
            Debug.Log("[Habilidad] Lanzando Dash (E).");
            TryCast(AbilityType.ThirdAb);
        }
    }

    public void OnAbilityR(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && !isCtrlPressed && !isUpgrading) // Solo lanzar si no se está presionando CTRL
        {
            Debug.Log("[Habilidad] Lanzando Ulti Explosiva (R).");
            TryCast(AbilityType.Ultimate);
        }
    }

    // Acciones de mejora (que no se deben bloquear por CTRL)
    public void OnUpgradeQ(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && isCtrlPressed) // Solo mejorar si CTRL está presionado
        {
            Debug.Log("[Mejora] Intentando mejorar la habilidad Bola de Fuego (Q).");
            isUpgrading = true; // Bloquear lanzamiento de habilidad mientras mejoramos
            playerStats?.SpendSkillPoint(AbilityType.PrimaryAb);
            isUpgrading = false; // Liberar una vez terminado
        }
    }

    public void OnUpgradeW(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && isCtrlPressed) // Solo mejorar si CTRL está presionado
        {
            Debug.Log("[Mejora] Intentando mejorar la habilidad Escudo (W).");
            isUpgrading = true; // Bloquear lanzamiento de habilidad mientras mejoramos
            playerStats?.SpendSkillPoint(AbilityType.SecondaryAb);
            isUpgrading = false; // Liberar una vez terminado
        }
    }

    public void OnUpgradeE(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && isCtrlPressed) // Solo mejorar si CTRL está presionado
        {
            Debug.Log("[Mejora] Intentando mejorar la habilidad Dash (E).");
            isUpgrading = true; // Bloquear lanzamiento de habilidad mientras mejoramos
            playerStats?.SpendSkillPoint(AbilityType.ThirdAb);
            isUpgrading = false; // Liberar una vez terminado
        }
    }

    public void OnUpgradeR(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && isCtrlPressed) // Solo mejorar si CTRL está presionado
        {
            Debug.Log("[Mejora] Intentando mejorar la habilidad Ulti Explosiva (R).");
            isUpgrading = true; // Bloquear lanzamiento de habilidad mientras mejoramos
            playerStats?.SpendSkillPoint(AbilityType.Ultimate);
            isUpgrading = false; // Liberar una vez terminado
        }
    }

    private void TryCast(AbilityType abilityType)
    {
        if (abilities.ContainsKey(abilityType))
        {
            Debug.Log("[Habilidad] Ejecutando la habilidad.");
            abilities[abilityType].Cast();
        }
    }

    public void UpgradeAbility(AbilityType abilityType)
    {
        if (abilities.ContainsKey(abilityType))
        {
            Debug.Log("[Mejora] Mejorando la habilidad.");
            abilities[abilityType].Upgrade();
        }
    }

    public bool TryUpgradeAbility(AbilityType abilityType, int playerLevel)
    {
        if (!abilities.ContainsKey(abilityType)) return false;

        Ability ability = abilities[abilityType];

        if (abilityType == AbilityType.Ultimate && playerLevel < 5)
        {
            Debug.Log("[Mejora] La habilidad Ultimate se desbloquea en nivel 5.");
            return false;
        }

        if (!ability.Locked && ability.Level >= ability.MaxLevel)
        {
            Debug.Log("[Mejora] La habilidad ya está al máximo nivel.");
            return false;
        }

        ability.Upgrade();
        return true;
    }
}
