//using UnityEngine;

//public class PlayerHealth : MonoBehaviour
//{
//    [Header("Configuración de Vida")]
//    public int maxHealth = 100;
//    public int currentHealth;

//    private bool isDead = false;

//    void Start()
//    {
//        currentHealth = maxHealth;
//        EventManager.LifeChanged(currentHealth); // Notificar al inicio
//    }

//    public void TakeDamage(int amount)
//    {
//        if (isDead) return;

//        currentHealth -= amount;
//        if (currentHealth < 0) currentHealth = 0;

//        Debug.Log($"Jugador recibió {amount} de daño. Vida actual: {currentHealth}");

//        EventManager.LifeChanged(currentHealth); // Actualiza UI

//        if (currentHealth <= 0)
//        {
//            Die();
//        }
//    }

//    public void Heal(int amount)
//    {
//        if (isDead) return;

//        currentHealth += amount;
//        if (currentHealth > maxHealth) currentHealth = maxHealth;

//        Debug.Log($"Jugador recuperó {amount} de vida. Vida actual: {currentHealth}");

//        EventManager.LifeChanged(currentHealth);
//    }

//    private void Die()
//    {
//        if (isDead) return;

//        isDead = true;
//        Debug.Log("💀 El jugador ha muerto.");
//        EventManager.PlayerDied();
//    }
//}
