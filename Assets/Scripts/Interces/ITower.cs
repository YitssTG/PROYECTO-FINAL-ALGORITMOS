using UnityEngine;

public interface ITower
{
    float AttackRange { get; }
    int Damage { get; }
    float FireRate { get; }

    void Initialize(TowerSO data);
    void Shoot(EnemyBase enemy);
}
