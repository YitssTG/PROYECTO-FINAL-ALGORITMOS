using UnityEngine;

public class EnemyMiniTank : EnemyBase
{
    [Header("Configuraci�n miniTank")]
    public float tankDetection = 4f;
    public float tankAttack = 2.5f;
    public float tankSpeed = 3f;

    protected override void Awake()
    {
        base.Awake();
        enemyName = "MiniTank";
        health = 200;
        damage = 15;

        if (movement != null)
        {
            movement.detectionRadius = tankDetection;
            movement.attackRadius = tankAttack;
            movement.speed = tankSpeed;
        }
    }

    public override void Attack()
    {
        Debug.Log($"{enemyName} dispara y ataca con {damage} de da�o.");
    }
}
