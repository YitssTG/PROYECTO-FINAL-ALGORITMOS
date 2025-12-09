using UnityEngine;

public class PauseController : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;

    private bool isPaused;

    private void Start()
    {
        // Sincroniza la variable con el estado real del panel
        isPaused = pausePanel.activeSelf;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    private void TogglePause()
    {
        isPaused = !isPaused;

        pausePanel.SetActive(isPaused);

        Time.timeScale = isPaused ? 0f : 1f;

        //Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;
        //Cursor.visible = isPaused;
    }
}
