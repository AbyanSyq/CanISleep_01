using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI; // Tambahkan untuk Image
using System.Collections;
using Ami.BroAudio;

public class UIMonologue : UIBase
{
    [SerializeField] private UIBase uiText;
    [SerializeField] private Image speakerIcon; // Untuk menampung icon dari textData
    [SerializeField] private TextMeshProUGUI monologueText;

    [Header("Audio")]
    public SoundID dialogBlipID;
    public float pitchMin = 0.9f;
    public float pitchMax = 1.1f;
    float nextSoundTime = 0f;
    float interval = 0.02f;

    [Header("Typing Settings")]
    public float typingSpeed = 0.03f;

    private Coroutine typingRoutine;
    public bool isTyping { get; private set; }

    public void OnMonologueLoad(MonologueTextData data)
    {
        if (typingRoutine != null)
            StopCoroutine(typingRoutine);

        // Update UI Elements
        if (speakerIcon != null) speakerIcon.sprite = data.icon;
        if (uiText != null) uiText.Show();
        typingRoutine = StartCoroutine(TypeMonologue(data.text));

        Debug.Log($"Displaying Monologue: {data.type}");
    }

    private IEnumerator TypeMonologue(string fullText)
    {
        isTyping = true;
        monologueText.text = "";

        foreach (char c in fullText)
        {
            monologueText.text += c;

            // PLAY SOUND hanya huruf & angka (biar natural)
            if (char.IsLetterOrDigit(c) && Time.unscaledTime >= nextSoundTime)
            {
                BroAudio.Play(dialogBlipID)
                        .SetPitch(Random.Range(pitchMin, pitchMax));

                nextSoundTime = Time.unscaledTime + interval;
            }

            yield return new WaitForSecondsRealtime(typingSpeed);
        }

        isTyping = false;
    }

    public void HideUIText()
    {
        if (uiText != null)
            uiText.Hide();
    }

    // Optional: Jika ingin skip animasi ketik
    public void SkipTyping(string fullText)
    {
        if (!isTyping) return;

        StopCoroutine(typingRoutine);
        monologueText.text = fullText;
        isTyping = false;
    }
}