using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyData", menuName = "Enemies/Enemy Data")]
public class EnemyDataSO : ScriptableObject
{
    [Header("Identidad")]
    public string enemyName = "Enemy";
    public EnemyType enemyType = EnemyType.Melee;

    [Header("Stats Base")]
    public int baseHealth = 100;
    public int baseDamage = 10;
    public float moveSpeed = 3.5f;
    public float attackSpeed = 1f;

    [Header("Rangos")]
    public float detectionRadius = 20f;
    public float attackRadius = 15f;

    [Header("Recompensas")]
    public int rewardXP = 50;
    public int rewardGold = 20;

    [Header("Prefab")]
    public GameObject enemyPrefab;

    public static bool operator ==(EnemyDataSO a, EnemyDataSO b)
    {
        if (a is null) return b is null;
        return a.Equals(b);
    }

    public static bool operator !=(EnemyDataSO a, EnemyDataSO b)
    {
        return !(a == b);
    }

    public override bool Equals(object obj)
    {
        if (obj is EnemyDataSO other)
        {
            return enemyName == other.enemyName &&
                   baseHealth == other.baseHealth &&
                   baseDamage == other.baseDamage;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return (enemyName + baseHealth + baseDamage).GetHashCode();
    }

    public virtual EnemyStats GetScaledStats(int waveNumber)
    {
        float scaleFactor = 1 + (waveNumber * 0.1f);
        return new EnemyStats
        {
            health = Mathf.RoundToInt(baseHealth * scaleFactor),
            damage = Mathf.RoundToInt(baseDamage * scaleFactor),
            moveSpeed = moveSpeed,
            attackSpeed = attackSpeed
        };
    }
}

public enum EnemyType
{
    Melee,
    Ranged,
    MiniTank,
    Boss
}

[System.Serializable]
public struct EnemyStats
{
    public int health;
    public int damage;
    public float moveSpeed;
    public float attackSpeed;

    public static EnemyStats operator +(EnemyStats a, EnemyStats b)
    {
        return new EnemyStats
        {
            health = a.health + b.health,
            damage = a.damage + b.damage,
            moveSpeed = a.moveSpeed + b.moveSpeed,
            attackSpeed = a.attackSpeed + b.attackSpeed
        };
    }

    public static EnemyStats operator *(EnemyStats stats, float multiplier)
    {
        return new EnemyStats
        {
            health = Mathf.RoundToInt(stats.health * multiplier),
            damage = Mathf.RoundToInt(stats.damage * multiplier),
            moveSpeed = stats.moveSpeed * multiplier,
            attackSpeed = stats.attackSpeed * multiplier
        };
    }
}
