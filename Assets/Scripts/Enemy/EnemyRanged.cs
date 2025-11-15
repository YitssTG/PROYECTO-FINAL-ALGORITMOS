using UnityEngine;

public class EnemyRanged : EnemyBase
{
    protected override void Awake()
    {
        base.Awake();
        enemyName = "Ranged";
    }

    public override void Attack()
    {
        Debug.Log("Ranged dispara");
    }
}
