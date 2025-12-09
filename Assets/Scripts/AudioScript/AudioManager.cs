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
        OnCollisionMusic?.Invoke(audioSettings.AudioMixerGroup, audioData.AudioClip);
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
