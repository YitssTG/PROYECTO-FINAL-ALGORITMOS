using System;
using UnityEngine;

public class Ability
{
    public string Name;
    public float Cooldown;
    private float lastCastTime;
    public int Level { get; private set; }
    public int MaxLevel;
    public bool Locked = true; // La habilidad está bloqueada inicialmente

    public Action OnCast;

    public Ability(string name, float cooldown, int maxLevel = 5)
    {
        Name = name;
        Cooldown = cooldown;
        lastCastTime = -cooldown;
        Level = 0;
        MaxLevel = maxLevel;
    }

    // Verificar si la habilidad puede lanzarse
    public bool CanCast()
    {
        return !Locked && Level > 0 && Time.time >= lastCastTime + Cooldown;
    }

    // Lanzar la habilidad
    public void Cast()
    {
        if (CanCast())
        {
            lastCastTime = Time.time;
            Debug.Log($"La habilidad {Name} ha sido lanzada (Nivel {Level})");
            OnCast?.Invoke();
            return;
        }

        if (Locked || Level == 0)
        {
            Debug.Log($"La habilidad {Name} está bloqueada.");
            return;
        }

        float remaining = (lastCastTime + Cooldown) - Time.time;
        Debug.Log($"La habilidad {Name} está en cooldown. Faltan {remaining:F1}s");
    }

    // Mejorar la habilidad
    public void Upgrade()
    {
        if (Locked)
        {
            Locked = false;
            Level = 1;
            Debug.Log($"La habilidad {Name} ha sido desbloqueada en el nivel {Level}!");
            return;
        }

        if (Level < MaxLevel)
        {
            Level++;
            Cooldown = Mathf.Max(0.5f, Cooldown * 0.9f); // Reducir el cooldown
            Debug.Log($"La habilidad {Name} ha sido mejorada a nivel {Level}, Cooldown: {Cooldown:F1}s");
        }
        else
        {
            Debug.Log($"La habilidad {Name} ya está al máximo nivel!");
        }
    }
}
