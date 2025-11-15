using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Referencias UI")]
    public TMP_Text goldText;
    public TMP_Text healthText;
    public TMP_Text experienceText;

    [Header("Referencias de los sistemas")]
    public GoldManager goldManager;
    public PlayerStats playerStats;

    void Start()
    {
        // Asegurarse de que los managers estén asignados
        if (goldManager != null)
            goldManager.OnGoldChanged.AddListener(UpdateGoldDisplay);

        if (playerStats != null)
        {
            playerStats.OnHealthChanged.AddListener(UpdateHealthDisplay);
            playerStats.OnExperienceChanged.AddListener(UpdateExperienceDisplay);
        }
        else
        {
            Debug.LogError("❌ PlayerStats no asignado en UIManager.");
        }

        // Inicialización al comenzar
        UpdateGoldDisplay(goldManager.currentGold);
        UpdateHealthDisplay(playerStats.currentHealth);  // Usar currentHealth
        UpdateExperienceDisplay(playerStats.experience);
    }

    private void OnDestroy()
    {
        // Eliminar listeners para evitar problemas al destruir el objeto
        if (goldManager != null)
            goldManager.OnGoldChanged.RemoveListener(UpdateGoldDisplay);

        if (playerStats != null)
        {
            playerStats.OnHealthChanged.RemoveListener(UpdateHealthDisplay);
            playerStats.OnExperienceChanged.RemoveListener(UpdateExperienceDisplay);
        }
    }

    // Métodos para actualizar la UI
    private void UpdateGoldDisplay(int newGoldAmount)
    {
        if (goldText != null)
            goldText.text = $"{newGoldAmount}";
    }

    private void UpdateHealthDisplay(int newHealthAmount)
    {
        if (healthText != null)
            healthText.text = $"Vida: {newHealthAmount}";
    }

    private void UpdateExperienceDisplay(int newExperienceAmount)
    {
        if (experienceText != null)
            experienceText.text = $"XP: {newExperienceAmount}";
    }
}
