using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    public string enemyName;
    public int health;
    public int damage;

    [Header("Recompensas")]
    public int rewardXP = 50;
    public int rewardGold = 20; // Oro base

    [Header("Movimiento y radios")]
    public EnemyMovement movement;

    protected virtual void Awake()
    {
        if (movement == null)
            movement = GetComponent<EnemyMovement>();
    }

    public abstract void Attack();

    public virtual void TakeDamage(int amount)
    {
        health -= amount;
        Debug.Log($"{enemyName} recibe {amount} de daño. Vida restante: {health}");

        if (health <= 0)
            Die();
    }

    public virtual void Die()
    {
        Debug.Log($"{enemyName} ha muerto.");

        // ⚡ Enviar evento global de muerte
        EventManager.EnemyDefeated();

        // ⚡ Notificar recompensa de oro
        EventManager.CoinsCollected(rewardGold);  // El oro se pasa según rewardGold

        // ⚡ Dar XP al jugador
        GameManager.Instance.playerStats.AddExperience(rewardXP);

        movement.Die();
    }
}
