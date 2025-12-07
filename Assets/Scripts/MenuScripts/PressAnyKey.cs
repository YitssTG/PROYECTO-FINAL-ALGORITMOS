using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PressAnyKey : MonoBehaviour
{
    public TextMeshProUGUI pressKeyText;
    public float fadeSpeed = 2f;
    public string nextSceneName;

    private bool fadingOut = true;
    private Color originalColor;

    void Start()
    {
        if (pressKeyText != null)
        {
            originalColor = pressKeyText.color;
        }
    }

    void Update()
    {
        // Cargar escena por nombre
        if (Input.anyKeyDown)
        {
            SceneManager.LoadScene(nextSceneName);
        }

        // Fade / parpadeo
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
}
