using UnityEngine;
using Ami.BroAudio;

public class EndingAudioHandler : MonoBehaviour
{
    [Header("Audio")]
    public SoundID walkingAudio;
    public SoundID endingBGMAudio;
    public SoundID doorSqueakAudio;
    public SoundID doorHandleJiggleAudio;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PlayEndingMusic()
    {
        BroAudio.Play(endingBGMAudio);
    }

    public void PlayWalkingSound()
    {
        BroAudio.Play(walkingAudio);
    }

    public void StopWalkingSound()
    {
        BroAudio.Stop(walkingAudio);
    }

    public void PlayDoorSqueakSound()
    {
        BroAudio.Play(doorSqueakAudio);
    }
    public void PlayDoorHandleJiggleSound()
    {
        BroAudio.Play(doorHandleJiggleAudio);
    }
}
