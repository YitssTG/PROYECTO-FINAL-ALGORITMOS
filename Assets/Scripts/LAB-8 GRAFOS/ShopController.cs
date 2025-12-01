using UnityEngine;
using UnityEngine.InputSystem;

public class ShopController : MonoBehaviour
{
    private void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            ToggleShop();
        }
    }

    private void ToggleShop()
    {
        GameManager.Instance?.shopManager?.ToggleShop();
    }
}