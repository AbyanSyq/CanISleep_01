using UnityEngine;
using Ami.BroAudio;

public class GameplayAudioHandler : MonoBehaviour
{
    [Header("Audio")]
    public SoundID gameplayBGM;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        BroAudio.Play(gameplayBGM).AsBGM();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
