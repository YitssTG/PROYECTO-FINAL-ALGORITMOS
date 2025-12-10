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

    [Header("Cooldown UI")]
    public GameObject cooldownMask;
    public Image cooldownFill;
    public TextMeshProUGUI cooldownText;

    public float waitTimeout = 5f; // ya no se usa, pero lo dejamos por compatibilidad

    private Ability currentAbility;

    IEnumerator Start()
    {
        Debug.Log($"🔧 AbilityUI {abilityKey} iniciando...");

        // Esperar hasta que AbilityManager esté listo (sin morir por timeout)
        while (AbilityManager.Instance == null || !AbilityManager.Instance.IsReady())
        {
            yield return null;
        }

        Setup();
    }

    private void Setup()
    {
        currentAbility = AbilityManager.Instance.GetAbility(abilityKey);

        if (currentAbility == null)
        {
            Debug.LogError($"No se pudo obtener la habilidad {abilityKey}");
            return;
        }

        if (cooldownMask != null)
            cooldownMask.SetActive(false);

        if (upgradeButton != null)
        {
            upgradeButton.onClick.RemoveAllListeners();
            upgradeButton.onClick.AddListener(OnUpgradeClicked);
        }

        UpdateUI();
        Debug.Log($"AbilityUI {abilityKey} listo ✅");
    }

    void Update()
    {
        if (currentAbility == null) return;

        UpdateUI();
    }

    private void UpdateUI()
    {
        if (currentAbility == null || AbilityManager.Instance == null) return;

        int abilityLevel = AbilityManager.Instance.GetAbilityLevel(abilityKey);

        if (levelText != null)
            levelText.text = "Lv " + abilityLevel.ToString();

        if (upgradeButton != null)
        {
            bool canUpgrade = AbilityManager.Instance.CanUpgradeAbility(abilityKey);
            upgradeButton.gameObject.SetActive(canUpgrade);
        }

        UpdateCooldownVisual();
    }

    private void UpdateCooldownVisual()
    {
        if (currentAbility == null || cooldownMask == null || AbilityManager.Instance == null) return;

        float remaining = AbilityManager.Instance.GetCooldownRemaining(abilityKey);
        bool onCooldown = remaining > 0.05f;

        if (onCooldown)
        {
            if (!cooldownMask.activeSelf)
                cooldownMask.SetActive(true);

            if (cooldownText != null)
                cooldownText.text = Mathf.Ceil(remaining).ToString();

            if (cooldownFill != null && currentAbility.cooldown > 0f)
            {
                cooldownFill.fillAmount = remaining / currentAbility.cooldown;
            }
        }
        else
        {
            if (cooldownMask.activeSelf)
                cooldownMask.SetActive(false);
        }
    }

    private void OnUpgradeClicked()
    {
        if (AbilityManager.Instance == null) return;

        Debug.Log($"Botón de upgrade clickeado para {abilityKey}");
        AbilityManager.Instance.UpgradeAbility(abilityKey);
    }
}
