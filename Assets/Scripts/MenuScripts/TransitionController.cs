using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.SceneManagement;

public class TransitionController : MonoBehaviour
{
    public RectTransform panelLeft;
    public RectTransform panelRight;
    public RectTransform logo;
    public Slider slider;
    public TextMeshProUGUI fadeText;

    public RectTransform shakeTarget; // 🔥 NUEVO: objeto que sacudirá (Canvas o un empty)

    public float panelSpeed = 1.2f;
    public float logoDropSpeed = 0.8f;
    public float fakeLoadingTime = 2f;
    public float fadeSpeed = 2f;

    public string nextSceneName = "NombreDeLaSiguienteEscena";

    private bool started = false;
    private bool textFadingOut = true;

    void Start()
    {
        slider.value = 0f;
        slider.gameObject.SetActive(false);
        logo.gameObject.SetActive(false);

        if (fadeText != null)
        {
            Color c = fadeText.color;
            c.a = 1f;
            fadeText.color = c;
        }
    }

    void Update()
    {
        FadeTextEffect();

        if (!started && Input.anyKeyDown)
        {
            started = true;
            ClosePanels();
        }
    }

    void FadeTextEffect()
    {
        if (fadeText == null) return;

        Color c = fadeText.color;

        if (textFadingOut)
        {
            c.a -= fadeSpeed * Time.deltaTime;
            if (c.a <= 0.2f)
            {
                c.a = 0.2f;
                textFadingOut = false;
            }
        }
        else
        {
            c.a += fadeSpeed * Time.deltaTime;
            if (c.a >= 1f)
            {
                c.a = 1f;
                textFadingOut = true;
            }
        }

        fadeText.color = c;
    }

    void ClosePanels()
    {
        fadeText.gameObject.SetActive(false);

        panelLeft.DOAnchorPosX(0, panelSpeed).SetEase(Ease.InOutQuad);
        panelRight.DOAnchorPosX(0, panelSpeed)
        .SetEase(Ease.InOutQuad)
        .OnComplete(DoImpactShake);  // 🔥 SHAKE al terminar de cerrar
    }

    void DoImpactShake()
    {
        // 🔥 Aplicamos shake suave pero con impacto
        if (shakeTarget != null)
        {
            shakeTarget.DOShakePosition(
                0.25f,     // duración
                25f,       // fuerza
                50,        // vibraciones
                90f,       // aleatoriedad
                false,
                true
            );
        }

        // Continua la animación original
        DropLogo();
    }

    void DropLogo()
    {
        logo.gameObject.SetActive(true);

        logo.DOAnchorPosY(0, logoDropSpeed)
        .SetEase(Ease.OutBounce)
        .OnComplete(StartFakeLoading);
    }

    void StartFakeLoading()
    {
        slider.gameObject.SetActive(true);

        DOTween.To(
            () => slider.value,
            x => slider.value = x,
            1f,
            fakeLoadingTime
        ).OnComplete(OpenPanelsAndFinish);
    }

    void OpenPanelsAndFinish()
    {
        float leftExit = -panelLeft.rect.width;
        float rightExit = panelRight.rect.width;

        panelLeft.DOAnchorPosX(leftExit, panelSpeed).SetEase(Ease.OutQuad);
        panelRight.DOAnchorPosX(rightExit, panelSpeed).SetEase(Ease.OutQuad);

        logo.DOScale(0f, 0.4f).SetEase(Ease.InBack);
        slider.transform.DOScale(0f, 0.3f);

        DOVirtual.DelayedCall(0.6f, LoadScene);
    }

    void LoadScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}
