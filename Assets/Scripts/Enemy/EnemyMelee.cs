using UnityEngine;

public class EnemyMelee : EnemyBase
{
    [Header("Comportamiento Melee")]
    [HideInInspector] public float chargeSpeed = 5f;
    [HideInInspector] public float stunDuration = 0.5f;

    public override void Attack()
    {
        Debug.Log($"⚔️ {enemyName} ataque MELEE - Carga con {damage} de daño");

        if (movement != null && movement.target != null)
        {
            Vector3 chargeDirection = (movement.target.position - transform.position).normalized;
        }
    }

    public override void TakeDamage(int amount)
    {     
        int reducedDamage = Mathf.RoundToInt(amount * 0.8f);
        base.TakeDamage(reducedDamage);
    }

    public void PerformChargeAttack()
    {
        Debug.Log($"{enemyName} realiza carga a velocidad {chargeSpeed}");
    }
}
