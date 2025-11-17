using UnityEngine;

public class DayNightCyclePRO : MonoBehaviour
{
    [Header("☀ Referencias")]
    public Light sun;
    public Material skyboxMaterial;
    public GameObject stars; // objeto con estrellas (particles o mesh)

    [Header("⏱ Tiempo del Ciclo")]
    public float dayDuration = 60f; // tiempo total en segundos (día + noche)
    [Range(0f, 1f)]
    public float timeOfDay = 0f; // 0 = medianoche, 0.25 = amanecer, 0.5 = mediodía

    float cycleSpeed;

    [Header("🎨 Colores")]
    public Color sunriseColor = new Color(1f, 0.5f, 0.2f);
    public Color noonColor = Color.white;
    public Color sunsetColor = new Color(1f, 0.3f, 0.1f);
    public Color nightColor = new Color(0.1f, 0.1f, 0.35f);

    [Header("💡 Intensidad")]
    public float dayIntensity = 1f;
    public float nightIntensity = 0f;

    [Header("🌁 Niebla Dinámica")]
    public Color fogDayColor = new Color(0.8f, 0.9f, 1f);
    public Color fogNightColor = new Color(0.02f, 0.02f, 0.05f);
    public float fogDayDensity = 0.003f;
    public float fogNightDensity = 0.01f;

    [Header("🌌 Skybox")]
    public Color skyDayColor = new Color(0.4f, 0.6f, 1f);
    public Color skyNightColor = new Color(0.01f, 0.01f, 0.1f);

    void Start()
    {
        cycleSpeed = 1f / dayDuration;
        RenderSettings.skybox = skyboxMaterial;
    }

    void Update()
    {
        // PROGRESO DEL TIEMPO
        timeOfDay += Time.deltaTime * cycleSpeed;
        if (timeOfDay > 1f)
            timeOfDay = 0f;

        // ROTAR EL SOL
        float sunAngle = timeOfDay * 360f;
        sun.transform.rotation = Quaternion.Euler(sunAngle - 90f, 170f, 0f);

        // COLOR DEL SOL SEGÚN LA HORA
        if (timeOfDay < 0.25f) // noche → amanecer
            sun.color = Color.Lerp(nightColor, sunriseColor, timeOfDay * 4f);
        else if (timeOfDay < 0.5f) // amanecer → mediodía
            sun.color = Color.Lerp(sunriseColor, noonColor, (timeOfDay - 0.25f) * 4f);
        else if (timeOfDay < 0.75f) // mediodía → atardecer
            sun.color = Color.Lerp(noonColor, sunsetColor, (timeOfDay - 0.5f) * 4f);
        else // atardecer → noche
            sun.color = Color.Lerp(sunsetColor, nightColor, (timeOfDay - 0.75f) * 4f);

        // INTENSIDAD DEL SOL
        float intensity = 1f - Mathf.Abs(timeOfDay - 0.5f) * 2f; // pico al mediodía
        intensity = Mathf.Lerp(nightIntensity, dayIntensity, intensity);
        sun.intensity = intensity;

        // SKYBOX
        Color skyColor = Color.Lerp(skyNightColor, skyDayColor, intensity);
        skyboxMaterial.SetColor("_Tint", skyColor);

        // ESTRELLAS
        if (stars != null)
            stars.SetActive(intensity < 0.2f); // aparecen cuando es de noche

        // NIEBLA
        RenderSettings.fogColor = Color.Lerp(fogNightColor, fogDayColor, intensity);
        RenderSettings.fogDensity = Mathf.Lerp(fogNightDensity, fogDayDensity, intensity);
    }
}
