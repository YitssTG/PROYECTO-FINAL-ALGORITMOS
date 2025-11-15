using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    public Transform player;
    public Transform endPoint;

    public GameObject enemyMeleePrefab;
    public int meleePorGrupo = 0;

    public GameObject enemyRangedPrefab;
    public int rangedPorGrupo = 1;

    public GameObject enemyMiniTankPrefab;
    public int miniTankPorGrupo = 0;

    public float radioSpawn = 10f;
    public float intervaloEntreGrupos = 20f;
    public float intervaloEntreEnemigos = 0.5f;

    private List<GameObject> enemigosVivos = new List<GameObject>();
    private bool puedeSpawnear = false;
    private float tiempoSiguienteGrupo = 0f;

    void Update()
    {
        if (!puedeSpawnear) return;

        tiempoSiguienteGrupo -= Time.deltaTime;
        if (tiempoSiguienteGrupo <= 0)
        {
            StartCoroutine(SpawnGrupoConDelay());
            tiempoSiguienteGrupo = intervaloEntreGrupos;
        }
    }

    private IEnumerator SpawnGrupoConDelay()
    {
        for (int i = 0; i < meleePorGrupo; i++)
            yield return SpawnEnemy(enemyMeleePrefab);

        for (int i = 0; i < rangedPorGrupo; i++)
            yield return SpawnEnemy(enemyRangedPrefab);

        for (int i = 0; i < miniTankPorGrupo; i++)
            yield return SpawnEnemy(enemyMiniTankPrefab);
    }

    private IEnumerator SpawnEnemy(GameObject prefab)
    {
        if (prefab == null) yield break;

        GameObject enemigo = Instantiate(prefab, GetRandomPosition(), Quaternion.identity);

        EnemyMovement mov = enemigo.GetComponent<EnemyMovement>();
        if (mov != null)
        {
            mov.target = player;

            if (endPoint != null)
                mov.SetWalkDestination(endPoint.position);
        }

        enemigosVivos.Add(enemigo);
        yield return new WaitForSeconds(intervaloEntreEnemigos);
    }

    private Vector3 GetRandomPosition()
    {
        Vector3 pos = transform.position + Random.insideUnitSphere * radioSpawn;
        pos.y = 0;
        return pos;
    }

    public void SetActive(bool active)
    {
        puedeSpawnear = active;
        tiempoSiguienteGrupo = 0f;
    }

    // ⭐⭐⭐ Este método DEBE existir para evitar tu error
    public void ActualizarDificultad(int numeroOleada, int incremento)
    {
        meleePorGrupo += incremento;
        rangedPorGrupo += incremento / 2;
        miniTankPorGrupo += incremento / 2;

        intervaloEntreGrupos = Mathf.Max(5f, intervaloEntreGrupos - 1f);

        Debug.Log($"Spawner {name}: dificultad actualizada en oleada {numeroOleada}");
    }
}
