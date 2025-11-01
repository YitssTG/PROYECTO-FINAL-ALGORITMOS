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

    public override void Die()
    {
        rewardGold = 40; // Oro para enemigo Ranged
        rewardXP = 80;
        base.Die(); // Llamamos a la lógica común de la base
    }

    public override void Attack()
    {
        Debug.Log($"{enemyName} dispara con {damage} de daño.");
    }
}
