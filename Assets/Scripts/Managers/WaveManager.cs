using UnityEngine;
using System.Collections;
using TMPro;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance;

    [Header("Tiempos de fases")]
    public float preparationTime = 15f;
    public float waveDuration = 60f;

    [Header("Generadores de enemigos")]
    public EnemySpawner[] spawners;

    [Header("UI del tiempo")]
    public TextMeshProUGUI timerText;

    [Header("Dificultad")]
    public int incrementoPorOleada = 2;

    [Header("Mini Jefe")]
    public GameObject miniJefePrefab;          
    public Transform puntoMiniJefe;           
    public int oleadaMiniJefe = 3;             
    public int frecuenciaMiniJefe = 3;        

    private bool isWaveActive = false;
    private float timer;
    private int currentWave = 0;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        StartCoroutine(WaveRoutine());
    }

    IEnumerator WaveRoutine()
    {
        while (true)
        {
            isWaveActive = false;
            currentWave++;
            Debug.Log($"Preparación antes de la oleada {currentWave}");
            SetSpawnersActive(false);

            timer = preparationTime;
            while (timer > 0)
            {
                UpdateTimerUI("PREPARACIÓN", timer);
                timer -= Time.deltaTime;
                yield return null;
            }

            isWaveActive = true;
            Debug.Log($"Comienza la oleada {currentWave}");

            for (int i = 0; i < spawners.Length; i++)
            {
                if (spawners[i] != null)
                    spawners[i].ActualizarDificultad(currentWave, incrementoPorOleada);
            }

            SetSpawnersActive(true);

            //if (miniJefePrefab != null && DebeAparecerMiniJefe(currentWave))
            //{
            //    SpawnMiniJefe();
            //}

            timer = waveDuration;
            while (timer > 0)
            {
                UpdateTimerUI("OLEADA", timer);
                timer -= Time.deltaTime;
                yield return null;
            }
            Debug.Log($"Oleada {currentWave} finalizada");
            SetSpawnersActive(false);
        }
    }

    private bool DebeAparecerMiniJefe(int wave)
    {
        if (wave < oleadaMiniJefe) return false;
        return (wave - oleadaMiniJefe) % frecuenciaMiniJefe == 0;
    }

    //private void SpawnMiniJefe()
    //{
    //    Vector3 spawnPos;

    //    if (puntoMiniJefe != null)
    //        spawnPos = puntoMiniJefe.position;
    //    else if (spawners != null && spawners.Length > 0)
    //        spawnPos = spawners[0].transform.position + new Vector3(0, 0, 2);
    //    else
    //        spawnPos = Vector3.zero;

    //    GameObject jefe = Instantiate(miniJefePrefab, spawnPos, Quaternion.identity);
    //    Debug.Log($"Mini Jefe generado en oleada {currentWave}");

    //    EnemyMovement mov = jefe.GetComponent<EnemyMovement>();
    //    if (mov != null && spawners.Length > 0)
    //        mov.target = spawners[0].player; 
    //}

    private void SetSpawnersActive(bool active)
    {
        if (spawners == null) return;

        for (int i = 0; i < spawners.Length; i++)
        {
            if (spawners[i] != null)
                spawners[i].SetActive(active);
        }
    }

    private void UpdateTimerUI(string fase, float tiempo)
    {
        if (timerText != null)
        {
            int minutos = Mathf.FloorToInt(tiempo / 60);
            int segundos = Mathf.FloorToInt(tiempo % 60);
            timerText.text = $"{fase} {currentWave}\n{minutos:00}:{segundos:00}";
        }
    }

    public bool IsWaveActive() => isWaveActive;
    public int GetCurrentWave() => currentWave;
}