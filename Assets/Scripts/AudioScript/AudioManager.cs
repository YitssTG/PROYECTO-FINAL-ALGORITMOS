using System;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioData audioData;
    [SerializeField] private AudioSettings audioSettings;

    public static event Action<AudioMixerGroup, AudioClip> OnCollisionMusic;
    public static event Action<AudioMixerGroup> OnCollisionStopMusic;
    public static event Action<AudioMixerGroup, AudioClip> OnExitCollision;

    public static event Action<AudioClip> OnFootstep;
    public static event Action<AudioClip> OnCoinCollectedSound;

    private void Start()
    {
        // Música inicial al cargar la escena
        ChangeMusic(audioSettings.AudioMixerGroup, audioData.AudioClip);
    }

    // ============================
    // MÉTODOS PARA OTROS SCRIPTS
    // ============================

    public static void ChangeMusic(AudioMixerGroup group, AudioClip clip)
    {
        OnCollisionMusic?.Invoke(group, clip);
    }

    public static void StopMusic(AudioMixerGroup group)
    {
        OnCollisionStopMusic?.Invoke(group);
    }

    public static void PlayExitMusic(AudioMixerGroup group, AudioClip clip)
    {
        OnExitCollision?.Invoke(group, clip);
    }

    public static void TriggerFootstep(AudioClip clip)
    {
        OnFootstep?.Invoke(clip);
    }

    public static void TriggerCoinSound(AudioClip clip)
    {
        OnCoinCollectedSound?.Invoke(clip);
    }
}
