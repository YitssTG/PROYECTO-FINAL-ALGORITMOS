using UnityEngine;

public class EnemyMiniTank : EnemyBase
{
    [Header("Comportamiento Tank")]
    [HideInInspector] public float slowAuraRadius = 5f;

    public override void Attack()
    {
        Debug.Log($"{enemyName} ataque TANK - Golpe con {damage} de daño");
        ApplySlowAura();
    }

    public override void TakeDamage(int amount)
    {
        base.TakeDamage(amount);
    }

    private void ApplySlowAura()
    {
        Collider[] playersInRange = Physics.OverlapSphere(transform.position, slowAuraRadius);
        foreach (var collider in playersInRange)
        {
            if (collider.CompareTag("Player"))
            {
                Debug.Log($"{enemyName} aplica slow en área de {slowAuraRadius}m");
            }
        }
    }

}
