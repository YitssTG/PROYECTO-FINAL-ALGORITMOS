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

    public float waitTimeout = 5f;

    private Ability currentAbility;

    IEnumerator Start()
    {
        Debug.Log($"🔧 AbilityUI {abilityKey} iniciando...");
        float timer = 0f;

        while ((AbilityManager.Instance == null || !AbilityManager.Instance.IsReady()) && timer < waitTimeout)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        if (AbilityManager.Instance == null)
        {
            Debug.LogError("AbilityManager no disponible en AbilityUI");
            yield break;
        }

        if (!AbilityManager.Instance.IsReady())
        {
            Debug.LogError("AbilityManager no está listo");
            yield break;
        }

        currentAbility = AbilityManager.Instance.GetAbility(abilityKey);
        if (currentAbility == null)
        {
            Debug.LogError($"No se pudo obtener la habilidad {abilityKey} desde AbilityManager");
            yield break;
        }

        if (cooldownMask != null)
            cooldownMask.SetActive(false);

        if (upgradeButton != null)
        {
            upgradeButton.onClick.RemoveAllListeners();
            upgradeButton.onClick.AddListener(OnUpgradeClicked);
        }

        UpdateUI();
        Debug.Log($"AbilityUI {abilityKey} inicializado correctamente");
    }

    void Update()
    {
        if (currentAbility == null) return;

        UpdateUI();
    }

    private void UpdateUI()
    {
        if (currentAbility == null) return;

        int abilityLevel = AbilityManager.Instance.GetAbilityLevel(abilityKey);
        if (levelText != null)
            levelText.text = "Lv " + abilityLevel.ToString();

        if (upgradeButton != null)
        {
            bool canUpgrade = AbilityManager.Instance.CanUpgradeAbility(abilityKey);
            upgradeButton.gameObject.SetActive(canUpgrade);

            if (canUpgrade)
            {
                Debug.Log($"Botón {abilityKey} MOSTRADO - Se puede mejorar");
            }
        }

        UpdateCooldownVisual();
    }

    private void UpdateCooldownVisual()
    {
        if (currentAbility == null || cooldownMask == null) return;

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
        Debug.Log($"Botón de upgrade clickeado para {abilityKey}");
        AbilityManager.Instance.UpgradeAbility(abilityKey);
    }
}