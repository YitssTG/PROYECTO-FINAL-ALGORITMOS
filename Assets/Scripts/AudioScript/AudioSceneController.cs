using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioSceneController : MonoBehaviour
{
    [Header("Musica por escena")]
    public AudioData menuMusic;
    public AudioData level1Music;
    public AudioData level2Music;
    public AudioData bossMusic;

    [Header("Mixer donde va la música")]
    public AudioSettings musicSettings;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += CambiarMusica;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= CambiarMusica;
    }

    private void CambiarMusica(Scene scene, LoadSceneMode mode)
    {
        AudioClip clip = null;

        switch (scene.name)
        {
            case "Menu":
                clip = menuMusic.AudioClip;
                break;

            case "Gameplay":
                clip = level1Music.AudioClip;
                break;
        }

        if (clip != null)
        {
            AudioManager.ChangeMusic(musicSettings.AudioMixerGroup, clip);
        }
    }
}
