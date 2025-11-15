using UnityEngine;

public class EnemyMiniTank : EnemyBase
{
    protected override void Awake()
    {
        base.Awake();
        enemyName = "MiniTank";
    }

    public override void Attack()
    {
        Debug.Log("MiniTank dispara proyectil pesado");
    }
}
