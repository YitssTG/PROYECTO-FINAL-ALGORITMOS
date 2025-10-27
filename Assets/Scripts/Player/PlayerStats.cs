using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("XP y Nivel")]
    public int playerLevel = 1;
    public int experience = 0;
    public int experienceToNext = 100;
    public int skillPoints = 0;

    [Header("Estadísticas base (para ítems y mejoras)")]
    public float damage = 10f;
    public float speed = 5f;
    public float armor = 0f;
    public int gold = 0;



    private GameManager gm;

    void Awake()
    {
        gm = GameManager.Instance;
    }

    // Método para agregar experiencia
    public void AddExperience(int amount)
    {
        experience += amount;
        Debug.Log($"Ganaste {amount} XP (Total: {experience}/{experienceToNext})");

        if (experience >= experienceToNext)
        {
            LevelUp();
        }
    }

    // Subir de nivel
    private void LevelUp()
    {
        experience -= experienceToNext;
        playerLevel++;
        experienceToNext = Mathf.RoundToInt(experienceToNext * 1.5f);
        skillPoints++; // Ganar puntos de habilidad al subir de nivel
        Debug.Log($"Subiste a nivel {playerLevel}. Puntos disponibles: {skillPoints}");
    }

    // Método para gastar puntos de habilidad
    public bool SpendSkillPoint(AbilityType abilityKey)
    {
        if (skillPoints > 0 && gm.abilitySystem != null)
        {
            if (gm.abilitySystem.TryUpgradeAbility(abilityKey, playerLevel))
            {
                skillPoints--; // Disminuir puntos de habilidad al gastar
                Debug.Log($"Mejoraste la habilidad {abilityKey}. Puntos restantes: {skillPoints}");
                return true;
            }
            else
            {
                Debug.Log($"No puedes mejorar la habilidad {abilityKey} (nivel requerido o máximo alcanzado).");
            }
        }
        return false;
    }

    // --------------------------------------------
    // 🔽 NUEVOS MÉTODOS PARA ÍTEMS 🔽
    // --------------------------------------------

    public void AddGold(int amount)
    {
        gold += amount;
        Debug.Log($"Oro actual: {gold}");
    }

    public bool SpendGold(int cost)
    {
        if (gold >= cost)
        {
            gold -= cost;
            Debug.Log($"Compraste un ítem por {cost}. Oro restante: {gold}");
            return true;
        }

        Debug.Log("No tienes suficiente oro.");
        return false;
    }
}
