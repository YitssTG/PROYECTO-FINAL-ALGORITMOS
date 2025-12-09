using UnityEngine;

public interface IAttacker
{
    int Damage { get; }               
    float AttackRate { get; }       
    void Attack(IDamageable target);   
}