using UnityEngine;
using UnityEngine.UI;

public class TownHealthUI : MonoBehaviour
{
    public Slider townSlider;

    private void Start()
    {
        if (GameManager.Instance == null) return;

        townSlider.maxValue = GameManager.Instance.maxTownHealth;
        townSlider.value = GameManager.Instance.currentTownHealth;

        GameManager.Instance.OnTownHealthChanged += UpdateUI;
    }

    private void UpdateUI(float value)
    {
        townSlider.value = value;
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnTownHealthChanged -= UpdateUI;
    }
}
