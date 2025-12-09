using UnityEngine;

public abstract class EnemyBase : MonoBehaviour, IDamageable
{
    [Header("UI")]
    public EnemyHealthBarUI healthBarInstance; // Referencia directa al slider que ya está en el prefab

    [Header("Stats")]
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

        if (healthBarInstance != null)
        {
            healthBarInstance.SetMaxHealth(health);
            healthBarInstance.SetHealth(health);
        }
    }

    protected virtual void Update()
    {
        if (healthBarInstance != null && Camera.main != null)
            healthBarInstance.transform.LookAt(Camera.main.transform);
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

        if (healthBarInstance != null)
        {
            healthBarInstance.SetMaxHealth(health);
            healthBarInstance.SetHealth(health);
        }
    }

    public virtual void TakeDamage(int amount)
    {
        health -= amount;
        if (healthBarInstance != null)
            healthBarInstance.SetHealth(health);

        if (health <= 0)
            Die();
    }

    public bool IsDead() => health <= 0;
    public int GetCurrentHealth() => health;    

    public virtual void Die()
    {
        if (healthBarInstance != null)
            Destroy(healthBarInstance.gameObject);

        OnEnemyDeath?.Invoke(this);

        Debug.Log($"{enemyName} ha muerto.");
        Destroy(gameObject);
    }

    public abstract void Attack();
}
