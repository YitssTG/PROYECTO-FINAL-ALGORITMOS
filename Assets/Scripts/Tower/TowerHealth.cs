using UnityEngine;
using System.Collections;

public class TowerHealth : MonoBehaviour
{
    [Header("Configuración de Salud")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("Efectos Visuales")]
    public GameObject destructionEffect;

    public static System.Action<GameObject> OnTowerDestroyed;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"{gameObject.name} recibió {damage} de daño. Vida: {currentHealth}/{maxHealth}");

        if (damage > 0)
        {
            StartCoroutine(FlashDamage());
        }

        if (currentHealth <= 0)
        {
            DestroyTower();
        }
    }

    private IEnumerator FlashDamage()
    {
        Renderer rend = GetComponent<Renderer>();
        if (rend != null)
        {
            Color originalColor = rend.material.color;
            rend.material.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            rend.material.color = originalColor;
        }
    }

    private void DestroyTower()
    {
        Debug.Log($"{gameObject.name} DESTRUIDA!");

        if (destructionEffect != null)
        {
            Instantiate(destructionEffect, transform.position, Quaternion.identity);
        }

        OnTowerDestroyed?.Invoke(gameObject);

        BuildSlot slot = GetComponentInParent<BuildSlot>();
        if (slot != null)
        {
            slot.ClearSlot(); 
        }

        if (TowerManager.Instance != null)
        {
            TowerManager.Instance.UnregisterTower(gameObject);
        }

        Destroy(gameObject);
    }

    void OnMouseDown()
    {
        Debug.Log($"{gameObject.name} - Vida: {currentHealth}/{maxHealth}");
    }
}
