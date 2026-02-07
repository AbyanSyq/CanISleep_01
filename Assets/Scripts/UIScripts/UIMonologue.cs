using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI; 
using System.Collections;
using Ami.BroAudio;

public class UIMonologue : UIBase
{
    [SerializeField] private UIBase uiText;
    [SerializeField] private Image speakerIcon; 
    [SerializeField] private TextMeshProUGUI monologueText;

    [Header("Audio")]
    public SoundID dialogBlipID;
    public float pitchMin = 0.9f;
    public float pitchMax = 1.1f;
    float nextSoundTime = 0f;
    float interval = 0.05f; // Sedikit diperlambat agar tidak spamming

    [Header("Typing Settings")]
    public float typingSpeed = 0.03f;

    private Coroutine typingRoutine;
    public bool isTyping { get; private set; }

    public void OnMonologueLoad(MonologueTextData data)
    {
        if (typingRoutine != null) StopCoroutine(typingRoutine);

        // Update UI Elements
        if (speakerIcon != null) speakerIcon.sprite = data.icon;
        if (uiText != null) uiText.Show();
        
        typingRoutine = StartCoroutine(TypeMonologue(data.text));

        // Debug.Log($"Displaying Monologue: {data.type}");
    }

    private IEnumerator TypeMonologue(string fullText)
    {
        isTyping = true;
        monologueText.text = "";

        // Loop per karakter
        for (int i = 0; i < fullText.Length; i++)
        {
            monologueText.text += fullText[i];

            // Play Sound Logic
            if (char.IsLetterOrDigit(fullText[i]) && Time.unscaledTime >= nextSoundTime)
            {
                BroAudio.Play(dialogBlipID).SetPitch(UnityEngine.Random.Range(pitchMin, pitchMax));
                nextSoundTime = Time.unscaledTime + interval;
            }

            yield return new WaitForSecondsRealtime(typingSpeed);
        }

        isTyping = false;
    }

    public void HideUIText()
    {
        if (uiText != null) uiText.Hide();
    }

    // Fungsi untuk menampilkan teks secara instan
    public void SkipTyping(string fullText)
    {
        // Hentikan proses mengetik
        if (typingRoutine != null) StopCoroutine(typingRoutine);
        
        // Tampilkan full text langsung
        monologueText.text = fullText;
        
        // Update status
        isTyping = false;
    }
}