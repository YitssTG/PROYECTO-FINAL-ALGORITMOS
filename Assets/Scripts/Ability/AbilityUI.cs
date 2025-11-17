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
    public GameObject cooldownMask;      // Panel negro encima del icono
    public Image cooldownFill;           // Cuadrado negro con Fill
    public TextMeshProUGUI cooldownText; // Segundos

    public float waitTimeout = 5f;

    private GameManager gm;
    private PlayerStats playerStats;
    private Ability abilityRef;

    IEnumerator Start()
    {
        float timer = 0f;

        // ESPERAR A QUE SE INICIALICEN LOS SISTEMAS
        while ((GameManager.Instance == null ||
                GameManager.Instance.abilitySystem == null ||
                !GameManager.Instance.abilitySystem.abilities.ContainsKey(abilityKey) ||
                GameManager.Instance.playerStats == null)
               && timer < waitTimeout)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        gm = GameManager.Instance;
        playerStats = gm.playerStats;
        abilityRef = gm.abilitySystem.abilities[abilityKey];

        // 🔹 COOLdown UI inicia apagado SIEMPRE
        if (cooldownMask != null)
            cooldownMask.SetActive(false);

        // BOTÓN DE UPGRADE CONFIG
        upgradeButton.onClick.RemoveAllListeners();
        upgradeButton.onClick.AddListener(OnUpgradeClicked);

        UpdateUI();
    }

    void Update()
    {
        if (abilityRef == null) return;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (abilityRef == null) return;

        // ---------- SISTEMA DE UPGRADE ----------
        bool hasPoints = playerStats.skillPoints > 0;
        bool notMax = abilityRef.level < abilityRef.maxLevel;
        bool canUpgrade = hasPoints && notMax;

        if (abilityKey == AbilityType.Ultimate && playerStats.playerLevel < 5)
            canUpgrade = false;

        upgradeButton.gameObject.SetActive(canUpgrade);
        levelText.text = "Lv " + abilityRef.level.ToString();

        // ---------- COOLDOWN VISUAL ----------
        float remaining = abilityRef.GetCooldownRemaining();
        bool onCooldown = remaining > 0.05f;  // evita activar por errores de precisión

        if (onCooldown)
        {
            // ACTIVAR MÁSCARA (si no está ya activa)
            if (!cooldownMask.activeSelf)
                cooldownMask.SetActive(true);

            // TEXTO DE SEGUNDOS RESTANTES
            if (cooldownText != null)
                cooldownText.text = Mathf.Ceil(remaining).ToString();

            // FILL DEL CUADRADO (0 a 1)
            if (cooldownFill != null && abilityRef.cooldown > 0f)
            {
                cooldownFill.fillAmount = remaining / abilityRef.cooldown;
            }
        }
        else
        {
            // SI YA ACABÓ EL COOLDOWN, OCULTAR LA MÁSCARA
            if (cooldownMask.activeSelf)
                cooldownMask.SetActive(false);
        }
    }

    private void OnUpgradeClicked()
    {
        bool upgraded = playerStats.SpendSkillPoint(abilityKey);
        if (upgraded)
            Debug.Log($"Habilidad {abilityKey} mejorada.");

        UpdateUI();
    }
}
