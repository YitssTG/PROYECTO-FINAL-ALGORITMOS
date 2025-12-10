using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseController : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;

    private bool isPaused;
    private bool isSceneValid = true;

    private void Start()
    {
        if (pausePanel != null)
            isPaused = pausePanel.activeSelf;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Desactivar pausa en la escena Menu
        isSceneValid = scene.name != "Menu";

        if (!isSceneValid)
        {
            if (pausePanel != null)
                pausePanel.SetActive(false);

            Time.timeScale = 1f;
        }
    }

    private void Update()
    {
        if (!isSceneValid) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    private void TogglePause()
    {
        if (pausePanel == null)
        {
            Debug.LogWarning("Pause panel destruido o no asignado.");
            return;
        }

        isPaused = !isPaused;

        pausePanel.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;
    }
}
