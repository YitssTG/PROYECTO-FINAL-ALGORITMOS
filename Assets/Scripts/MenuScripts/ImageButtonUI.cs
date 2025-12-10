using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class ImageButtonUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public enum ButtonType
    {
        Play,
        Options,
        Exit
    }

    [Header("Tipo de botón")]
    public ButtonType buttonType;

    [Header("Hover Scale")]
    public float hoverScale = 1.15f;
    public float scaleTime = 0.2f;

    [Header("Options Panel")]
    public GameObject optionsPanel;

    [Header("Play Scene")]
    public string gameSceneName;

    private Vector3 originalScale;
    private bool isTransitioning = false;
    private bool optionsOpen = false;

    void Start()
    {
        originalScale = transform.localScale;

        if (optionsPanel != null)
            optionsPanel.SetActive(false);
    }

    void Update()
    {
        // Cerrar panel con ESC
        if (optionsOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseOptions();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // NO matamos tweens globales, solo la escala
        DOTween.Kill("ScaleTween_" + gameObject.GetInstanceID());
        transform.DOScale(originalScale * hoverScale, scaleTime)
                 .SetId("ScaleTween_" + gameObject.GetInstanceID());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        DOTween.Kill("ScaleTween_" + gameObject.GetInstanceID());
        transform.DOScale(originalScale, scaleTime)
                 .SetId("ScaleTween_" + gameObject.GetInstanceID());
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isTransitioning) return;

        switch (buttonType)
        {
            case ButtonType.Play:
                PlayGame();
                break;

            case ButtonType.Options:
                OpenOptions();
                break;

            case ButtonType.Exit:
                ExitGame();
                break;
        }
    }

    void PlayGame()
    {
        isTransitioning = true;

        if (Transicion.Instance != null)
        {
            Transicion.Instance.TransicionCerrarPuertas();
            Transicion.OnFinishCloseDoors += LoadScene;
        }
        else
        {
            LoadScene();
        }
    }

    void LoadScene()
    {
        Transicion.OnFinishCloseDoors -= LoadScene;
        SceneManager.LoadScene(gameSceneName);
    }

    void OpenOptions()
    {
        if (optionsPanel == null) return;

        optionsPanel.SetActive(true);

        optionsPanel.transform.DOKill();
        optionsPanel.transform.localScale = Vector3.zero;
        optionsPanel.transform.DOScale(Vector3.one, 0.25f);

        optionsOpen = true;
    }

    void CloseOptions()
    {
        if (optionsPanel == null) return;

        optionsPanel.transform.DOKill();
        optionsPanel.transform.DOScale(Vector3.zero, 0.25f).OnComplete(() =>
        {
            optionsPanel.SetActive(false);
            optionsOpen = false;
        });
    }

    void ExitGame()
    {
        Application.Quit();
        Debug.Log("Salir del juego");
    }
}
