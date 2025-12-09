using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;

    private List<EnemyMovement> enemigosVivos = new List<EnemyMovement>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Debug.Log("EnemyManager Instance creada");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Registrar(EnemyMovement enemigo)
    {
        if (!enemigosVivos.Contains(enemigo))
            enemigosVivos.Add(enemigo);
    }

    public void Desregistrar(EnemyMovement enemigo)
    {
        if (enemigosVivos.Contains(enemigo))
            enemigosVivos.Remove(enemigo);
    }

    public int GetEnemigosVivosCount()
    {
        return enemigosVivos.Count;
    }

    public List<EnemyMovement> GetEnemigosVivos()
    {
        return new List<EnemyMovement>(enemigosVivos);
    }

    public void LimpiarTodosEnemigos()
    {
        foreach (var enemigo in enemigosVivos)
        {
            if (enemigo != null)
                Destroy(enemigo.gameObject);
        }
        enemigosVivos.Clear();
    }
}
