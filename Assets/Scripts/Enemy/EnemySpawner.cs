using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [Header("Configuración del Spawner")]
    public Transform player;
    public Transform endPoint; // Destino del carril (top, mid, bot)

    [Header("Tipos de enemigos y cantidad por grupo")]
    public GameObject enemyMeleePrefab;
    public int meleePorGrupo = 0;

    public GameObject enemyRangedPrefab;
    public int rangedPorGrupo = 1;

    public GameObject enemyMiniTankPrefab;
    public int miniTankPorGrupo = 0;

    public float radioSpawn = 10f;
    public float intervaloEntreGrupos = 20f;
    public float intervaloEntreEnemigos = 0.5f;

    private CustomLinkedList<GameObject> enemigosVivos = new CustomLinkedList<GameObject>();
    private bool puedeSpawnear = false;
    private float tiempoSiguienteGrupo = 0f;

    void OnEnable()
    {
        EventManager.OnEnemyDefeated += ContarMuerte;
    }

    void OnDisable()
    {
        EventManager.OnEnemyDefeated -= ContarMuerte;
    }

    void Update()
    {
        if (!puedeSpawnear) return;

        tiempoSiguienteGrupo -= Time.deltaTime;
        if (tiempoSiguienteGrupo <= 0)
        {
            StartCoroutine(SpawnGrupoConDelay());
            tiempoSiguienteGrupo = intervaloEntreGrupos;
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            EliminarTodos();
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

        Debug.Log($"Spawner {name} generó {meleePorGrupo} melee, {rangedPorGrupo} ranged, {miniTankPorGrupo} miniTank. Total vivos: {enemigosVivos.Count}");
    }

    private IEnumerator SpawnEnemy(GameObject prefab)
    {
        if (prefab == null) yield break;

        GameObject enemigo = Instantiate(prefab, GetRandomPosition(), Quaternion.identity);
        EnemyMovement mov = enemigo.GetComponent<EnemyMovement>();

        if (mov != null)
        {
            mov.target = player;
            mov.OnDeath += OnEnemyDeath;

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

    private void OnEnemyDeath(GameObject enemigo)
    {
        enemigosVivos.Remove(enemigo);
        Debug.Log($"Enemigo eliminado. Quedan: {enemigosVivos.Count}");
    }

    private void ContarMuerte()
    {
        Debug.Log($"Spawner {name} detectó una muerte mediante evento global.");
    }

    public void SetActive(bool active)
    {
        puedeSpawnear = active;
        tiempoSiguienteGrupo = 0f;

        if (active)
            Debug.Log($"Spawner {name} ACTIVADO");
        else
            Debug.Log($"Spawner {name} DESACTIVADO");
    }

    public void ActualizarDificultad(int numeroOleada, int incremento)
    {
        meleePorGrupo += incremento;
        rangedPorGrupo += incremento / 2;
        miniTankPorGrupo += incremento / 2;
        intervaloEntreGrupos = Mathf.Max(5f, intervaloEntreGrupos - 1f);

        Debug.Log($"Spawner {name}: dificultad actualizada. Oleada {numeroOleada}, Melee: {meleePorGrupo}, Ranged: {rangedPorGrupo}, MiniTank: {miniTankPorGrupo}, Intervalo: {intervaloEntreGrupos}s");
    }

    private void EliminarTodos()
    {
        foreach (var enemigo in enemigosVivos.GetAll())
        {
            if (enemigo != null)
                Destroy(enemigo);
        }

        enemigosVivos.Clear();
        Debug.Log("Todos los enemigos fueron eliminados manualmente (tecla J).");
    }
}
