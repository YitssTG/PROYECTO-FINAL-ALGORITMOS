using UnityEngine;

[CreateAssetMenu(fileName = "NewAbility", menuName = "Abilities/Ability")]
public class Ability : ScriptableObject
{
    [Header("Parámetros estilo LoL")]
    public float range = 8f;
    public float projectileSpeed = 20f;
    public float explosionRadius = 2f;
    public float damageBase = 40f;
    public float damagePerLevel = 20f;

    [Header("Buffs / Efectos")]
    public float duration = 3f;
    public float dashDistance = 6f;
    public float meteorDelay = 1.5f;
    public float meteorRadius = 4f;

    [Header("Datos base")]
    public string abilityName;
    public float cooldown;
    public int maxLevel = 5;
    public bool locked = true;

    // Helpers de datos (NO lógica de juego)
    public int GetDamageAtLevel(int level)
    {
        return Mathf.RoundToInt(damageBase + damagePerLevel * level);
    }

    public float GetCooldownAtLevel(int level)
    {
        return Mathf.Max(0.5f, cooldown * Mathf.Pow(0.95f, level - 1));
    }
}