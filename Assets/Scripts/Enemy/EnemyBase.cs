using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    [HideInInspector] public string enemyName;
    [HideInInspector] public int health;
    [HideInInspector] public int damage;
    [HideInInspector] public int rewardXP;
    [HideInInspector] public int rewardGold;
    [HideInInspector] public EnemyMovement movement;

    public delegate void EnemyDeathHandler(EnemyBase deadEnemy);
    public event EnemyDeathHandler OnEnemyDeath;

    public int CurrentHealth => health;
    public int CurrentDamage => damage;
    public string EnemyName => enemyName;

    protected virtual void Awake()
    {
        if (movement == null)
            movement = GetComponent<EnemyMovement>();
    }

    public virtual void Initialize(EnemyDataSO data, int waveNumber = 1)
    {
        if (data == null) return;

        enemyName = data.enemyName;
        health = data.baseHealth + waveNumber * 10;
        damage = data.baseDamage + waveNumber * 5;
        rewardXP = data.rewardXP;
        rewardGold = data.rewardGold;

        if (movement != null)
        {
            movement.speed = data.moveSpeed;
            movement.detectionRadius = data.detectionRadius;
            movement.attackRadius = data.attackRadius;
        }
    }

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

        OnEnemyDeath?.Invoke(this);

        Destroy(gameObject);
    }

    public abstract void Attack();
}
