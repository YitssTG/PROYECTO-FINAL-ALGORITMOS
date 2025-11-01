using UnityEngine;
using UnityEngine.Events;

public class GoldManager : MonoBehaviour
{
    public static GoldManager Instance;

    [Header("Configuración Inicial")]
    public int startingGold = 500;

    [HideInInspector] public int currentGold;
    public UnityEvent<int> OnGoldChanged = new();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        ResetGold();  // Inicializar oro al inicio
    }

    public void ResetGold()
    {
        currentGold = startingGold;
        OnGoldChanged.Invoke(currentGold);  // Notifica a la UI del oro inicial
        EventManager.CoinsCollected(currentGold);  // Notificar que el oro ha cambiado
    }

    public bool SpendGold(int amount)
    {
        if (currentGold < amount)
        {
            Debug.Log("❌ No hay suficiente oro.");
            return false;
        }

        currentGold -= amount;
        OnGoldChanged.Invoke(currentGold);  // Actualiza la UI con el nuevo oro
        EventManager.CoinsCollected(currentGold);  // Notificar que el oro ha cambiado
        return true;
    }

    public void AddGold(int amount)
    {
        currentGold += amount;
        OnGoldChanged.Invoke(currentGold);  // Actualiza la UI con el nuevo oro
        EventManager.CoinsCollected(currentGold);  // Notificar que el oro ha cambiado
    }
}
