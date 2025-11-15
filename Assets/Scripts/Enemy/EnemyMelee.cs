using UnityEngine;

public class EnemyMelee : EnemyBase
{
    [Header("Parámetros")]
    public float detectionRadius = 3f;
    public float attackRadius = 1.5f;
    public float moveSpeed = 3.5f;

    protected override void Awake()
    {
        base.Awake();

        enemyName = "Melee";
        movement.detectionRadius = detectionRadius;
        movement.attackRadius = attackRadius;
        movement.speed = moveSpeed;
    }

    public override void Die()
    {
        rewardGold = 30;
        rewardXP = 60;
        base.Die();
    }

    public override void Attack()
    {
        Debug.Log($"{enemyName} golpea cuerpo a cuerpo con {damage} de daño.");
    }
}
