using UnityEngine;

public class EnemyRanged : EnemyBase
{
    [Header("Configuración ranged")]
    public float rangedDetection = 5f;
    public float rangedAttack = 2.5f;
    public float rangedSpeed = 4.2f;

    protected override void Awake()
    {
        base.Awake();
        enemyName = "Ranged";
        health = 80;
        damage = 20;

        if (movement != null)
        {
            movement.detectionRadius = rangedDetection;
            movement.attackRadius = rangedAttack;
            movement.speed = rangedSpeed;
        }
    }
    protected override void Die()
    {
        rewardGold = 25;
        rewardXP = 80;
        base.Die();
    }

    public override void Attack()
    {
        Debug.Log($"{enemyName} dispara con {damage} de daño.");
    }
}
