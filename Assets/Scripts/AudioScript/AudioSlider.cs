using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class AudioSlider : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private TMP_Text volumeText;
    [SerializeField] private AudioSettings audioSettingsData;
    [SerializeField] private Image[] colores; //arreglo

    private void OnEnable()
    {
        audioSettingsData.OnUpdateVolume += UpdateText;

        audioSettingsData.UpdateVolume(audioSettingsData.VolumeScaled);

        slider.value = audioSettingsData.VolumeScaled;

        slider.onValueChanged.AddListener(audioSettingsData.UpdateVolume);
    }

    private void OnDisable()
    {
        audioSettingsData.OnUpdateVolume -= UpdateText;

        slider.onValueChanged.RemoveListener(audioSettingsData.UpdateVolume);
    }

    private void UpdateText(float value)// asigna color dependiendo del index 
    {
        volumeText.text = (value * 100).ToString("000") + "%";

        volumeText.text = (value * 100).ToString("000");
        Debug.Log(value);
        int index = 0;

        if (value < 0.142f) index = 0;
        else if (value < 0.285f) index = 1;
        else if (value < 0.428f) index = 2;
        else if (value < 0.571f) index = 3;
        else if (value < 0.714f) index = 4;
        else if (value < 0.857f) index = 5;
        else index = 6;

        for (int i = 0; i < colores.Length; i++)
        {
            Color original = colores[i].color;

            if (i <= index)
            {
                colores[i].color = new Color(1, 1, 1, 1f);
            }
            else
            {
                colores[i].color = new Color(original.r * 0.5f, original.g * 0.5f, original.b * 0.5f, 1f);

            }
        }
    }
}
