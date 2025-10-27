using UnityEngine;
using System;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 🎯 EVENTOS DE JUEGO
    public static event Action<int> OnCoinsCollected;
    public static event Action<int> OnLifeChanged;
    public static event Action OnPlayerDied;
    public static event Action OnVictory;
    public static event Action OnGameOver;
    public static event Action OnEnemyDefeated;

    // 🎮 INVOCADORES
    public static void CoinsCollected(int amount)
    {
        if (OnCoinsCollected != null)
            OnCoinsCollected(amount);
    }

    public static void LifeChanged(int life)
    {
        if (OnLifeChanged != null)
            OnLifeChanged(life);
    }

    public static void PlayerDied()
    {
        if (OnPlayerDied != null)
            OnPlayerDied();
    }

    public static void Victory()
    {
        if (OnVictory != null)
            OnVictory();
    }

    public static void GameOver()
    {
        if (OnGameOver != null)
            OnGameOver();
    }

    public static void EnemyDefeated()
    {
        if (OnEnemyDefeated != null)
            OnEnemyDefeated();
    }
}
