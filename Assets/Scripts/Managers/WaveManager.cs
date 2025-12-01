using UnityEngine;
using System.Collections;
using TMPro;

public class WaveManager : MonoBehaviour
{
    [Header("Tiempos de fases")]
    public float preparationTime = 15f;

    [Header("Spawners")]
    public EnemySpawner[] spawners;

    [Header("UI")]
    public TextMeshProUGUI timerText;

    private int currentWave = 0;
    private int maxWaves = 3;
    private bool isWaveActive = false;

    void Start()
    {
        StartCoroutine(WaveRoutine());
    }

    private IEnumerator WaveRoutine()
    {
        while (currentWave < maxWaves)
        {
            // --- PREPARACIÓN ---
            currentWave++;
            float prepTimer = preparationTime;
            UpdateTimerUI("PREPARACIÓN", prepTimer, true);

            while (prepTimer > 0)
            {
                prepTimer -= Time.deltaTime;
                UpdateTimerUI("PREPARACIÓN", prepTimer, true);
                yield return null;
            }

            // --- INICIO DE OLEADA ---
            isWaveActive = true;
            Debug.Log($"🔥 Comienza la oleada {currentWave}");

            // Activar spawners
            for (int i = 0; i < spawners.Length; i++)
            {
                spawners[i].SetActive(true);
                spawners[i].ActualizarDificultad(currentWave);
                StartCoroutine(spawners[i].SpawnOleada(currentWave));
            }

            // --- DURANTE OLEADA: contar enemigos y tiempo ---
            float waveTimer = 0f;
            while (EnemyManager.Instance.GetEnemigosVivosCount() > 0)
            {
                waveTimer += Time.deltaTime;
                UpdateTimerUI("OLEADA", waveTimer, false);
                yield return null;
            }

            isWaveActive = false;
            Debug.Log($"✅ Oleada {currentWave} finalizada");

            // Desactivar spawners
            for (int i = 0; i < spawners.Length; i++)
                spawners[i].SetActive(false);

            yield return null;
        }

        Debug.Log("🎉 ¡GANASTE! Todas las oleadas completadas.");
        if (timerText != null) timerText.text = "🎉 GANASTE 🎉";
    }

    private void UpdateTimerUI(string fase, float tiempo, bool countdown)
    {
        if (timerText == null) return;

        if (countdown)
        {
            int segundos = Mathf.CeilToInt(tiempo);
            timerText.text = $"{fase} {currentWave}\n{segundos:00}s";
        }
        else
        {
            int segundos = Mathf.FloorToInt(tiempo);
            timerText.text = $"{fase} {currentWave}\n{segundos:00}s";
        }
    }

    public bool IsWaveActive() => isWaveActive;
    public int GetCurrentWave() => currentWave;
}
