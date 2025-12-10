using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class LoadingScreen : MonoBehaviour
{
    public Slider barraCarga;
    public Image fadeImage; 
    public string nombreEscena = "Gameplay";

    void Start()
    {
        fadeImage.color = new Color(0, 0, 0, 1); 
        fadeImage.DOFade(0, 1f).OnComplete(() => StartCoroutine(CargarEscenaConRetraso(nombreEscena)));
    }
    IEnumerator CargarEscenaConRetraso(string nombreEscena)
    {
        float tiempoCargaSimulado = Random.Range(7f, 10f);
        float tiempoTranscurrido = 0f;

        AsyncOperation operacion = SceneManager.LoadSceneAsync(nombreEscena);
        operacion.allowSceneActivation = false;

        while (tiempoTranscurrido < tiempoCargaSimulado)
        {
            tiempoTranscurrido += Time.deltaTime;

            float progresoVisual = Mathf.Clamp01(tiempoTranscurrido / tiempoCargaSimulado);
            if (barraCarga != null)
                barraCarga.value = progresoVisual;

            yield return null;
        }
        yield return fadeImage.DOFade(1, 1f).WaitForCompletion();

        operacion.allowSceneActivation = true;
    }
}