using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class GoldManager : MonoBehaviour
{
    public static GoldManager Instance { get; private set; }

    [Header("Configuración de Oro")]
    public int currentGold = 1000; // Oro inicial
    public int maxGold = 99999;

    [Header("UI References")]
    public TMP_Text goldText;

    [Header("Eventos")]
    public UnityEvent<int> OnGoldChanged = new UnityEvent<int>();
    public UnityEvent<int> OnGoldAdded = new UnityEvent<int>();
    public UnityEvent<int> OnGoldSpent = new UnityEvent<int>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        UpdateGoldUI();
        // Disparar evento inicial
        OnGoldChanged?.Invoke(currentGold);
        Debug.Log($"💰 GoldManager iniciado con {currentGold} oro");
    }

    public void AddGold(int amount)
    {
        if (amount <= 0)
        {
            Debug.LogWarning("⚠️ Intentando agregar cantidad negativa o cero de oro");
            return;
        }

        int previousGold = currentGold;
        currentGold += amount;

        // Limitar oro máximo
        if (currentGold > maxGold)
        {
            currentGold = maxGold;
        }

        UpdateGoldUI();
        OnGoldChanged?.Invoke(currentGold);
        OnGoldAdded?.Invoke(amount);

        Debug.Log($"💰 +{amount} oro. Antes: {previousGold}, Ahora: {currentGold}");
    }

    public bool SpendGold(int amount)
    {
        if (amount <= 0)
        {
            Debug.LogWarning("⚠️ Intentando gastar cantidad negativa o cero de oro");
            return false;
        }

        if (currentGold >= amount)
        {
            int previousGold = currentGold;
            currentGold -= amount;

            UpdateGoldUI();
            OnGoldChanged?.Invoke(currentGold);
            OnGoldSpent?.Invoke(amount);

            Debug.Log($"💰 -{amount} oro. Antes: {previousGold}, Ahora: {currentGold}");
            return true;
        }
        else
        {
            Debug.LogWarning($"❌ Oro insuficiente: {currentGold}/{amount}");
            return false;
        }
    }

    private void UpdateGoldUI()
    {
        if (goldText != null)
        {
            goldText.text = $"💰 {currentGold}G";
        }
    }

    public void ResetGold()
    {
        currentGold = 1000;
        UpdateGoldUI();
        OnGoldChanged?.Invoke(currentGold);
        Debug.Log("🔄 Oro reiniciado a 1000");
    }

    // Para debugging
    public void DebugGoldStatus()
    {
        Debug.Log($"💰 Estado del oro: {currentGold}G");
    }
}