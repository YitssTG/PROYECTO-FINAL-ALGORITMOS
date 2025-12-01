using UnityEngine;

public class EnemyRanged : EnemyBase
{
    [Header("Comportamiento Ranged")]
    [HideInInspector] public float optimalRange = 8f;
    [HideInInspector] public float fleeDistance = 3f;

    private EnemyAttack attack;

    public override void Attack()
    {
        Debug.Log($"🏹 {enemyName} ataque RANGED - Dispara con {damage} de daño");

        if (movement != null && movement.target != null)
        {
            float distance = Vector3.Distance(transform.position, movement.target.position);
            if (distance < fleeDistance)
            {
                Debug.Log($"{enemyName} huye del jugador");
            }
        }
    }

    public override void Initialize(EnemyDataSO data, int waveNumber = 1)
    {
        base.Initialize(data, waveNumber);

        if (attack != null)
        {
            attack.isRanged = true;
        }
    }

    public void MaintainDistance()
    {
        Debug.Log($"{enemyName} mantiene distancia óptima: {optimalRange}");
    }
}
