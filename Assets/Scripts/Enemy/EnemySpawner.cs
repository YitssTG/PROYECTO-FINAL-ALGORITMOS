using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemigos por oleada")]
    public EnemyDataSO meleeEnemySO;
    public EnemyDataSO rangedEnemySO;
    public EnemyDataSO miniTankEnemySO;

    [Header("Cantidad base por grupo (se escalará con la oleada)")]
    public int meleeBase = 1;
    public int rangedBase = 1;
    public int miniTankBase = 0;

    [Header("Referencias")]
    public Transform player;
    public Transform endPoint; // Cada spawner su propio endPoint

    [Header("Configuración de spawn")]
    public float radioSpawn = 10f;
    public float intervaloEntreEnemigos = 0.5f;

    private bool puedeSpawnear = false;

    void Update()
    {
        // No hacemos nada, spawn controlado por WaveManager
    }

    public void SetActive(bool active)
    {
        puedeSpawnear = active;
    }

    public IEnumerator SpawnOleada(int waveNumber)
    {
        if (!puedeSpawnear) yield break;

        // La cantidad de grupos por oleada = número de oleada (1,2,3)
        int grupos = waveNumber;
        for (int g = 0; g < grupos; g++)
        {
            // Spawnear melee
            for (int i = 0; i < meleeBase; i++)
                yield return SpawnEnemy(meleeEnemySO, waveNumber);

            // Spawnear ranged
            for (int i = 0; i < rangedBase; i++)
                yield return SpawnEnemy(rangedEnemySO, waveNumber);

            // Spawnear miniTank
            for (int i = 0; i < miniTankBase; i++)
                yield return SpawnEnemy(miniTankEnemySO, waveNumber);

            // Espera entre grupos
            yield return new WaitForSeconds(1f);
        }
    }

    private IEnumerator SpawnEnemy(EnemyDataSO data, int waveNumber)
    {
        if (data == null || data.enemyPrefab == null) yield break;

        Vector3 spawnPos = transform.position + Random.insideUnitSphere * radioSpawn;
        spawnPos.y = 0;

        GameObject enemigoGO = Instantiate(data.enemyPrefab, spawnPos, Quaternion.identity);
        EnemyBase enemigoBase = enemigoGO.GetComponent<EnemyBase>();
        if (enemigoBase != null)
        {
            enemigoBase.Initialize(data, waveNumber); // Stats desde SO
        }

        EnemyMovement mov = enemigoGO.GetComponent<EnemyMovement>();
        if (mov != null)
        {
            mov.target = player;
            if (endPoint != null) mov.SetWalkDestination(endPoint.position);
            EnemyManager.Instance?.Registrar(mov);
        }

        yield return new WaitForSeconds(intervaloEntreEnemigos);
    }

    public void ActualizarDificultad(int waveNumber)
    {
        EnemyDataSO[] enemiesToSpawn = { meleeEnemySO, rangedEnemySO, miniTankEnemySO };
        for (int i = 0; i < enemiesToSpawn.Length; i++)
        {
            EnemyDataSO data = enemiesToSpawn[i];
            if (data == null) continue;

            EnemyStats stats = data.GetScaledStats(waveNumber);
            Debug.Log($"Spawner {name} - {data.enemyName} configurado: HP {stats.health}, DMG {stats.damage}");
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, radioSpawn);
    }
}
