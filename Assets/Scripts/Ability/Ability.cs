using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewAbility", menuName = "Abilities/Ability")]
public class Ability : ScriptableObject
{
    [Header("Datos base")]
    public string abilityName;
    public float cooldown;
    public int maxLevel = 5;
    public bool locked = true; // Bloqueada inicialmente

    [NonSerialized] private float lastCastTime;
    [NonSerialized] public int level = 0;
    [NonSerialized] public Action OnCast;

    public bool CanCast()
    {
        return !locked && level > 0 && Time.time >= lastCastTime + cooldown;
    }

    public void Cast()
    {
        if (CanCast())
        {
            lastCastTime = Time.time;
            Debug.Log($"[{abilityName}] lanzada (Nivel {level})");
            OnCast?.Invoke();
        }
        else if (locked || level == 0)
        {
            Debug.Log($"[{abilityName}] está bloqueada.");
        }
        else
        {
            float remaining = (lastCastTime + cooldown) - Time.time;
            Debug.Log($"[{abilityName}] en cooldown ({remaining:F1}s restantes)");
        }
    }

    public void Upgrade()
    {
        if (locked)
        {
            locked = false;
            level = 1;
            Debug.Log($"[{abilityName}] desbloqueada en nivel {level}");
            return;
        }

        if (level < maxLevel)
        {
            level++;
            cooldown = Mathf.Max(0.5f, cooldown * 0.9f);
            Debug.Log($"[{abilityName}] mejorada a nivel {level} (CD: {cooldown:F1}s)");
        }
        else
        {
            Debug.Log($"[{abilityName}] ya está al nivel máximo.");
        }
    }
}
