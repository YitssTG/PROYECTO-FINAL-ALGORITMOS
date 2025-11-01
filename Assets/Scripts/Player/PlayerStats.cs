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

    void Start()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager.Instance no está inicializado en PlayerStats.");
            enabled = false;
            return;
        }

        gm = GameManager.Instance;
    }

    // 🔹 Ganas experiencia
    public void AddExperience(int amount)
    {
        experience += amount;
        Debug.Log($"Ganaste {amount} XP (Total: {experience}/{experienceToNext})");

        if (experience >= experienceToNext)
        {
            LevelUp();
        }
    }

    // 🔹 Subes un nivel y ganas 1 punto de habilidad
    private void LevelUp()
    {
        experience -= experienceToNext;
        playerLevel++;
        experienceToNext = Mathf.RoundToInt(experienceToNext * 1.5f);
        skillPoints++;
        Debug.Log($"Subiste a nivel {playerLevel}. Puntos disponibles: {skillPoints}");
    }

    // 🔹 Gastas un punto de habilidad para mejorar una habilidad
    public bool SpendSkillPoint(AbilityType abilityKey)
    {
        if (gm == null || gm.abilitySystem == null)
        {
            Debug.LogError("AbilitySystem no encontrado en GameManager.");
            return false;
        }

        if (skillPoints > 0)
        {
            if (gm.abilitySystem.TryUpgradeAbility(abilityKey, playerLevel))
            {
                skillPoints--;
                Debug.Log($"Mejoraste la habilidad {abilityKey}. Puntos restantes: {skillPoints}");
                return true;
            }
            else
            {
                Debug.Log($"No puedes mejorar la habilidad {abilityKey} (nivel requerido o máximo alcanzado).");
            }
        }
        else
        {
            Debug.Log("No tienes puntos de habilidad disponibles.");
        }

        return false;
    }

    // 🔹 Añadir oro
    public void AddGold(int amount)
    {
        gold += amount;
        Debug.Log($"Oro actual: {gold}");
    }

    // 🔹 Gastar oro
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
