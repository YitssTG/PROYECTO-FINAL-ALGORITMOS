using UnityEngine;
using UnityEngine.Audio;

public class AudioPlayer : MonoBehaviour
{
    [SerializeField] private ChannelPlayer musicPlayer;
    [SerializeField] private ChannelPlayer sfxPlayer;

    private void OnEnable()
    {
        AudioManager.OnCollisionMusic += PlayPlayer;
        AudioManager.OnCollisionStopMusic += StopPlayer;
        AudioManager.OnExitCollision += PlayExit;
        AudioManager.OnFootstep += ReproducirPaso;
        AudioManager.OnCoinCollectedSound += PlayCoinSound;
    }
    private void OnDisable()
    {
        AudioManager.OnCollisionMusic -= PlayPlayer;
        AudioManager.OnCollisionStopMusic -= StopPlayer;
        AudioManager.OnExitCollision -= PlayExit;
        AudioManager.OnFootstep -= ReproducirPaso;
        AudioManager.OnCoinCollectedSound -= PlayCoinSound;

    }
    private void ReproducirPaso(AudioClip pasoClip)
    {
        if (pasoClip != null && sfxPlayer != null)
        {
            sfxPlayer.PlayClip(pasoClip, false); // Usa CreateTempAudioSource
        }
    }
    private void PlayCoinSound(AudioClip clip)
    {
        sfxPlayer.PlayClip(clip, false);
    }
    private void PlayPlayer(AudioMixerGroup currentGroup, AudioClip currentAudioClip)
    {
        Debug.Log("CurrentClip: " + currentAudioClip.name);
        if (currentGroup == musicPlayer.PlayerChannel)
        {
            musicPlayer.PlayClip(currentAudioClip, true);
        }
        else
        {
            sfxPlayer.PlayClip(currentAudioClip, false);
        }
    }
    private void StopPlayer(AudioMixerGroup currentGroup)
    {
        if (currentGroup == musicPlayer.PlayerChannel)
        {
            musicPlayer.StopClip();
        }
        else
        {
            sfxPlayer.StopClip();
        }
    }
    private void PlayExit(AudioMixerGroup currentGroup, AudioClip exitClip)
    {
        if (currentGroup == sfxPlayer.PlayerChannel)
        {
            sfxPlayer.ExitClip(exitClip);
        }
    }
}
