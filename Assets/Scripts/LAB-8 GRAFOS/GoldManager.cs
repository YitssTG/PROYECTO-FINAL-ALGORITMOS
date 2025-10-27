using UnityEngine;
using UnityEngine.Events;

public class GoldManager : MonoBehaviour
{
    public static GoldManager Instance; // ✅ Singleton global

    [Header("Configuración Inicial")]
    public int startingGold = 500;

    [HideInInspector] public int currentGold;
    public UnityEvent<int> OnGoldChanged = new();

    private void Awake()
    {
        // 🔹 Asegura que solo exista una instancia
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        ResetGold(); // 🟢 Reinicia el oro al iniciar la escena
    }

    public void ResetGold()
    {
        currentGold = startingGold;
        OnGoldChanged.Invoke(currentGold);
    }

    public bool SpendGold(int amount)
    {
        if (currentGold < amount)
        {
            Debug.Log("❌ No hay suficiente oro.");
            return false;
        }

        currentGold -= amount;
        OnGoldChanged.Invoke(currentGold);
        return true;
    }

    public void AddGold(int amount)
    {
        currentGold += amount;
        OnGoldChanged.Invoke(currentGold);
    }
}
