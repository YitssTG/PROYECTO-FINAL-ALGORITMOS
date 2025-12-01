using UnityEngine;

public class EnemyDeathHandler : MonoBehaviour
{
    private EnemyBase enemy;

    void Awake()
    {
        enemy = GetComponent<EnemyBase>();
        enemy.OnEnemyDeath += HandleEnemyDeath;
    }

    void OnDestroy()
    {
        if (enemy != null)
            enemy.OnEnemyDeath -= HandleEnemyDeath;
    }

    private void HandleEnemyDeath(EnemyBase deadEnemy)
    {
        // Notificar sistemas globales
        EventManager.EnemyDefeated();
        EventManager.CoinsCollected(deadEnemy.rewardGold); // Actualizado

        if (GameManager.Instance != null && GameManager.Instance.playerStats != null)
        {
            GameManager.Instance.playerStats.AddExperience(deadEnemy.rewardXP); // Actualizado
        }

        // Remover del manager
        EnemyMovement mov = GetComponent<EnemyMovement>();
        if (mov != null && EnemyManager.Instance != null)
        {
            EnemyManager.Instance.Desregistrar(mov);
        }

        Debug.Log($"💰 {deadEnemy.enemyName} muerto - +{deadEnemy.rewardGold} oro, +{deadEnemy.rewardXP} XP"); // Actualizado
    }
}
