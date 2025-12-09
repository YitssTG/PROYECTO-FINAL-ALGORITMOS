using UnityEngine;

public abstract class TowerBase : MonoBehaviour, ITower, IAttacker
{
    public TowerSO data;

    public float AttackRange => data.range;
    public int Damage => (int)data.damage;

    public float FireRate => 1f;
    public float AttackRate => FireRate;

    protected float fireTimer;

    public virtual void Initialize(TowerSO towerData)
    {
        data = towerData;
    }

    protected virtual void Update()
    {
        if (data == null) return;

        fireTimer -= Time.deltaTime;

        EnemyBase target = GetTarget();
        if (target != null && fireTimer <= 0f)
        {
            Attack(target); 
            fireTimer = AttackRate;
        }
    }

    public void Attack(IDamageable target)
    {
        Shoot((EnemyBase)target);
    }

    protected abstract EnemyBase GetTarget();
    public abstract void Shoot(EnemyBase enemy);
}
