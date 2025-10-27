using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class AbilityUI : MonoBehaviour
{
    [Header("Configuración")]
    public AbilityType abilityKey = AbilityType.PrimaryAb;
    public Button upgradeButton;
    public TextMeshProUGUI levelText;

    public float waitTimeout = 5f;

    private GameManager gm;
    private PlayerStats playerStats;
    private Ability abilityRef;

    IEnumerator Start()
    {
        float timer = 0f;

        // Esperar hasta que el GameManager y la habilidad estén disponibles
        while ((GameManager.Instance == null ||
                GameManager.Instance.abilitySystem == null ||
                !GameManager.Instance.abilitySystem.abilities.ContainsKey(abilityKey) ||
                GameManager.Instance.playerStats == null)
               && timer < waitTimeout)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        if (GameManager.Instance == null || GameManager.Instance.abilitySystem == null || GameManager.Instance.playerStats == null)
        {
            Debug.LogError("AbilityUI: GameManager o PlayerStats no están listos después del timeout.");
            enabled = false;
            yield break;
        }

        gm = GameManager.Instance;
        playerStats = gm.playerStats;
        abilityRef = gm.abilitySystem.abilities[abilityKey];

        if (upgradeButton == null)
        {
            Debug.LogError("AbilityUI: upgradeButton no asignado.");
            enabled = false;
            yield break;
        }

        upgradeButton.onClick.RemoveAllListeners();
        upgradeButton.onClick.AddListener(OnUpgradeClicked);

        upgradeButton.gameObject.SetActive(false); // Inicialmente desactivado
        UpdateUI();
    }

    void Update()
    {
        if (!enabled || abilityRef == null || playerStats == null) return;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (abilityRef == null || playerStats == null) return;

        // Determinar si se puede mejorar la habilidad
        bool hasPoints = playerStats.skillPoints > 0;
        bool notMax = abilityRef.Level < abilityRef.MaxLevel;
        bool canUpgrade = hasPoints && notMax;

        // La habilidad 'R' solo se puede mejorar si el jugador tiene nivel 5 o más
        if (abilityKey == AbilityType.Ultimate && playerStats.playerLevel < 5)
        {
            Debug.Log("La habilidad Ultimate no se puede mejorar porque el nivel del jugador es menor a 5.");
            canUpgrade = false;
        }

        // Habilitar el botón de mejora si se puede mejorar
        upgradeButton.gameObject.SetActive(canUpgrade);

        // Actualizar el texto del nivel de la habilidad
        if (levelText != null)
            levelText.text = "Lv " + abilityRef.Level.ToString();

        Debug.Log($"UI Actualizada - Habilidad: {abilityKey}, Nivel: {abilityRef.Level}, Puntos disponibles: {playerStats.skillPoints}");
    }

    private void OnUpgradeClicked()
    {
        if (playerStats == null) return;

        // Intentar gastar un punto de habilidad y mejorar la habilidad
        bool upgraded = playerStats.SpendSkillPoint(abilityKey);
        if (upgraded)
        {
            Debug.Log($"Habilidad {abilityKey} mejorada.");
        }
        UpdateUI();
    }

    void OnDestroy()
    {
        if (upgradeButton != null)
            upgradeButton.onClick.RemoveListener(OnUpgradeClicked);
    }
}
