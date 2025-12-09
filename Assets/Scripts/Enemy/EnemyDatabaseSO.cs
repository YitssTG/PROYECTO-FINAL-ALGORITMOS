using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "EnemyDatabase", menuName = "Enemies/Enemy Database")]
public class EnemyDatabaseSO : ScriptableObject
{
    public List<EnemyDataSO> allEnemies = new List<EnemyDataSO>();

    public EnemyDataSO GetEnemyByName(string enemyName)
    {
        return allEnemies.Find(enemy => enemy.enemyName == enemyName);
    }

    public List<EnemyDataSO> GetEnemiesByType(EnemyType type)
    {
        return allEnemies.FindAll(enemy => enemy.enemyType == type);
    }

    public EnemyDataSO GetRandomEnemy()
    {
        if (allEnemies.Count == 0) return null;
        return allEnemies[Random.Range(0, allEnemies.Count)];
    }

    public EnemyDataSO GetEnemy(int index)
    {
        return (index >= 0 && index < allEnemies.Count) ? allEnemies[index] : null;
    }

    public EnemyDataSO GetEnemy(string name)
    {
        return GetEnemyByName(name);
    }
}