using UnityEngine;

public class EnemyMiniTank : EnemyBase
{
    [Header("Comportamiento Tank")]
    [HideInInspector] public float slowAuraRadius = 5f;
    [HideInInspector] public int armor = 10;

    public override void Attack()
    {
        Debug.Log($"🛡️ {enemyName} ataque TANK - Golpe con {damage} de daño");
        ApplySlowAura();
    }

    public override void TakeDamage(int amount)
    {
        int damageAfterArmor = CalculateEffectiveDamage(amount);
        Debug.Log($"{enemyName} reduce daño {amount} -> {damageAfterArmor} (armadura: {armor})");
        base.TakeDamage(damageAfterArmor);
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

    public int CalculateEffectiveDamage(int incomingDamage)
    {
        return Mathf.Max(1, incomingDamage - armor);
    }
}
