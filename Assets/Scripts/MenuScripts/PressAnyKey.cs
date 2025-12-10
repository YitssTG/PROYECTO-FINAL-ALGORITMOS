using UnityEngine;
using TMPro;
using DG.Tweening;

public class PressAnyKeyUI : MonoBehaviour
{
    [Header("Texto")]
    public TextMeshProUGUI pressKeyText;
    public float fadeSpeed = 2f;

    [Header("Objetos a animar")]
    public RectTransform[] currentObjects; // elementos actuales que se cierran
    public RectTransform[] nextObjects;    // elementos que aparecen después
    public float moveDistance = 500f;
    public float moveDuration = 1f;
    public Ease moveEase = Ease.OutQuad;

    private bool fadingOut = true;
    private Color originalColor;
    private bool transitionStarted = false;

    void Start()
    {
        if (pressKeyText != null)
            originalColor = pressKeyText.color;

        // Al inicio, los siguientes objetos los escondemos
        foreach (var obj in nextObjects)
        {
            obj.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        // Detecta cualquier tecla y empieza la transición
        if (!transitionStarted && Input.anyKeyDown)
        {
            transitionStarted = true;
            StartTransition();
        }

        // Fade / parpadeo del texto
        if (pressKeyText != null)
        {
            Color c = pressKeyText.color;

            if (fadingOut)
            {
                c.a -= fadeSpeed * Time.deltaTime;
                if (c.a <= 0.1f)
                {
                    c.a = 0.1f;
                    fadingOut = false;
                }
            }
            else
            {
                c.a += fadeSpeed * Time.deltaTime;
                if (c.a >= originalColor.a)
                {
                    c.a = originalColor.a;
                    fadingOut = true;
                }
            }

            pressKeyText.color = c;
        }
    }

    void StartTransition()
    {
        // Animamos los objetos actuales hacia fuera
        foreach (var obj in currentObjects)
        {
            obj.DOAnchorPosX(obj.anchoredPosition.x - moveDistance, moveDuration).SetEase(moveEase);
        }

        // Después de la animación, activamos los siguientes objetos
        DOVirtual.DelayedCall(moveDuration, () =>
        {
            foreach (var obj in nextObjects)
            {
                obj.gameObject.SetActive(true);
                // Aparecen con animación desde fuera
                obj.anchoredPosition += new Vector2(moveDistance, 0);
                obj.DOAnchorPosX(obj.anchoredPosition.x - moveDistance, moveDuration).SetEase(moveEase);
            }
        });
    }
}
