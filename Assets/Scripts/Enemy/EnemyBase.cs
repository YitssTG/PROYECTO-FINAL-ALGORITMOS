using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    [Header("Identidad")]
    public string enemyName = "Enemy";

    [Header("Stats")]
    public int health = 100;
    public int damage = 10;

    [Header("Recompensas")]
    public int rewardXP = 50;
    public int rewardGold = 20;

    [Header("Movimiento")]
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

        EventManager.EnemyDefeated();
        EventManager.CoinsCollected(rewardGold);
        GameManager.Instance.playerStats.AddExperience(rewardXP);

        movement.Die();
    }
}
