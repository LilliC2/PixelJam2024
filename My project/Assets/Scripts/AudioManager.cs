using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : Singleton<AudioManager>
{
    public AudioMixer audioMixerVolumeController;
    public AudioSource buttonClick;
    public AudioSource winRound;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void ButtonClickAudio()
    {
        buttonClick.Play();
    }

  public void WinRoundAudio()
    {
        winRound.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
