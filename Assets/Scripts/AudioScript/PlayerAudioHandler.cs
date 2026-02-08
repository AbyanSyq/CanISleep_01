using UnityEngine;
using Ami.BroAudio;

public class PlayerAudioHandler : MonoBehaviour
{
    [Header("Audio")]
    public SoundID jumpingAudio;
    public SoundID dieAudio;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PlayJumpingSound()
    {
        BroAudio.Play(jumpingAudio);
    }   

    public void PlayDieSound()
    {
        BroAudio.Play(dieAudio);
    }
}
