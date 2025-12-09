using UnityEngine;
using UnityEngine.Audio;

public class ChannelPlayer : MonoBehaviour
{
    //relaciona la configuracion guardada
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSettings audioSettings;
    public AudioMixerGroup PlayerChannel => audioSource.outputAudioMixerGroup;
    private void Awake()
    {
        audioSource.outputAudioMixerGroup = audioSettings.AudioMixerGroup;
        audioSource.loop = true;
    }
    public void PlayClip(AudioClip clipToPlay, bool loop = true)
    {
        Debug.Log("clip: " + clipToPlay.name);
        if (loop)
        {
            audioSource.clip = clipToPlay;
            audioSource.Play();
        }
        else
        {
            CreateTempAudioSource(clipToPlay);
        }
    }
    public void StopClip()
    {
        audioSource.Stop();
    }
    public void ExitClip(AudioClip exitClip)
    {
        audioSource.loop = false;
        audioSource.clip = exitClip;
        audioSource.Play();
    }
    private void CreateTempAudioSource(AudioClip clipToPlay)
    {
        GameObject tempAudio = new GameObject("TempAudioSource");
        tempAudio.transform.position = transform.position;

        AudioSource tempSource = tempAudio.AddComponent<AudioSource>();
        tempSource.outputAudioMixerGroup = audioSettings.AudioMixerGroup;
        tempSource.clip = clipToPlay;
        tempSource.Play();

        Destroy(tempAudio, clipToPlay.length);
    }
}
