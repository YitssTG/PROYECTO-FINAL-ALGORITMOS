using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBarUI : MonoBehaviour
{
    public Slider healthSlider;
    public Transform target;

    public void SetMaxHealth(int maxHealth)
    {
        if (healthSlider != null)
            healthSlider.maxValue = maxHealth;
    }

    public void SetHealth(int currentHealth)
    {
        if (healthSlider != null)
            healthSlider.value = currentHealth;
    }

    void LateUpdate()
    {
        if (target != null && Camera.main != null)
        {
            transform.position = target.position + Vector3.up * 1.5f;
            transform.LookAt(Camera.main.transform);
        }
    }
}
