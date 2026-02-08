using UnityEngine;
using Ami.BroAudio;

public class MainMenuAudioHandler : MonoBehaviour
{
    [Header("Audio")]
    public SoundID whooshAudio;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PlayWhooshSound()
    {
        BroAudio.Play(whooshAudio);
    }
}
