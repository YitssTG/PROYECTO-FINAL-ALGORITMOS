using System;
using UnityEngine;

public static class EventManager
{
    // 🎯 EVENTOS DE JUEGO EXISTENTES
    public static event Action<int> OnCoinsCollected;
    public static event Action<int> OnLifeChanged;
    public static event Action OnPlayerDied;
    public static event Action OnVictory;
    public static event Action OnGameOver;
    public static event Action OnEnemyDefeated;
    public static event Action<int> OnWaveStarted;
    public static event Action<int> OnWaveCompleted;
    public static event Action OnHealthRegenStarted;
    public static event Action OnHealthRegenStopped;
    public static event Action<int> OnHealthRegenTick;

    // ⭐ NUEVOS EVENTOS PARA PLAYER
    public static event Action<EnemyBase, int> OnPlayerAttacked; // enemy, damage
    public static event Action<int> OnPlayerLevelUp; // newLevel
    public static event Action OnPlayerStatsChanged;


    // 🎮 INVOCADORES EXISTENTES
    public static void CoinsCollected(int amount) => OnCoinsCollected?.Invoke(amount);
    public static void LifeChanged(int life) => OnLifeChanged?.Invoke(life);
    public static void PlayerDied() => OnPlayerDied?.Invoke();
    public static void Victory() => OnVictory?.Invoke();
    public static void GameOver() => OnGameOver?.Invoke();
    public static void EnemyDefeated() => OnEnemyDefeated?.Invoke();
    public static void WaveStarted(int waveNumber) => OnWaveStarted?.Invoke(waveNumber);
    public static void WaveCompleted(int waveNumber) => OnWaveCompleted?.Invoke(waveNumber);

    // ⭐ NUEVOS INVOCADORES
    public static void PlayerAttacked(EnemyBase enemy, int damage) => OnPlayerAttacked?.Invoke(enemy, damage);
    public static void PlayerLevelUp(int newLevel) => OnPlayerLevelUp?.Invoke(newLevel);
    public static void PlayerStatsChanged() => OnPlayerStatsChanged?.Invoke();
    public static void HealthRegenStarted() => OnHealthRegenStarted?.Invoke();
    public static void HealthRegenStopped() => OnHealthRegenStopped?.Invoke();
    public static void HealthRegenTick(int healAmount) => OnHealthRegenTick?.Invoke(healAmount);

    // 🔧 MÉTODOS DE UTILIDAD PARA LIMPIAR EVENTOS
    public static void ClearAllEvents()
    {
        OnCoinsCollected = null;
        OnLifeChanged = null;
        OnPlayerDied = null;
        OnVictory = null;
        OnGameOver = null;
        OnEnemyDefeated = null;
        OnWaveStarted = null;
        OnWaveCompleted = null;
        OnPlayerAttacked = null;
        OnPlayerLevelUp = null;
        OnPlayerStatsChanged = null;
    }
}
