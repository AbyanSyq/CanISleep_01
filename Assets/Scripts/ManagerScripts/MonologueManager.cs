using System;
using System.Collections;
using UnityEngine;

public class MonologueManager : SingletonMonoBehaviour<MonologueManager>
{
    [Header("Settings")]
    [SerializeField] private UIMonologue uiMonologue;
    
    [Header("Current State")]
    [SerializeField] private Coroutine currentMonologueCoroutine;
    public bool isPlaying = false;
    
    // Flag untuk menandakan player meminta skip saat sedang masa "tunggu baca"
    [SerializeField] private bool skipRequested = false; 

    private PlayerInputAction inputActions;

    protected override void Awake()
    {
        base.Awake();
        Reinitialize();
        inputActions = new PlayerInputAction();
    }

    void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.UI.Enable();
        inputActions.UI.Skip.performed += ctx => HandleSkipInput();
    }

    void OnDisable()
    {
        inputActions.Player.Disable();
        inputActions.UI.Disable();
        inputActions.UI.Skip.performed -= ctx => HandleSkipInput();
    }

    private void Reinitialize()
    {
        isPlaying = false;
        skipRequested = false;
        if (currentMonologueCoroutine != null)
        {
            StopCoroutine(currentMonologueCoroutine);
            currentMonologueCoroutine = null;
        }
    }

    public void PlayMonologue(MonologueSO so, Action onComplete = null)
    {
        if (so == null || so.monologueDataList.Count == 0) return;
        
        if (currentMonologueCoroutine != null) StopCoroutine(currentMonologueCoroutine);
        currentMonologueCoroutine = StartCoroutine(MonologueSequenceRoutine(so, onComplete));
    }

    // Fungsi logic Skip
    private void HandleSkipInput()
    {
        // Jika sedang mengetik -> Skip typing (muncul semua)
        if (uiMonologue.isTyping)
        {
            // Kita butuh akses teks saat ini, tapi karena text ada di dalam loop coroutine,
            // kita biarkan MonologueSequenceRoutine yang mengurusnya atau 
            // kita set flag skipRequested = true agar loop di bawah merespon.
            skipRequested = true; 
        }
        else
        {
            // Jika sudah selesai ngetik (sedang baca) -> Lanjut ke dialog berikutnya
            skipRequested = true;
        }
    }

    private IEnumerator MonologueSequenceRoutine(MonologueSO so, Action onComplete)
    {                                                                                   
        isPlaying = true;
        uiMonologue.Show();

        float waitShow = uiMonologue.AnimationDuration > 0 ? uiMonologue.AnimationDuration : 0.1f;
        yield return new WaitForSeconds(waitShow);

        foreach (var data in so.monologueDataList)
        {
            // Reset flag skip sebelum memulai baris baru
            skipRequested = false; 

            // 1. Mulai Mengetik
            uiMonologue.OnMonologueLoad(data);
            
            // 2. Tunggu sampai mengetik selesai ATAU User minta skip
            while (uiMonologue.isTyping)
            {
                if (skipRequested)
                {
                    uiMonologue.SkipTyping(data.text); // Paksa selesai
                    skipRequested = false; // Reset flag setelah dipakai
                    break; 
                }
                yield return null;
            }

            // 3. Tunggu durasi baca (Display Duration)
            // Kita ganti WaitForSeconds dengan loop timer agar bisa di-break (skip)
            float timer = 0f;
            while (timer < data.displayDuration)
            {
                if (skipRequested)
                {
                    skipRequested = false; // Reset flag
                    break; // Keluar dari loop tunggu -> Lanjut ke teks berikutnya
                }
                timer += Time.unscaledDeltaTime; // Gunakan unscaled agar tidak terpengaruh TimeScale 0
                yield return null;
            }

            // 4. Jeda antar teks (Interval)
            if (so.intervalBetweenTexts > 0)
            {
                uiMonologue.HideUIText(); 
                yield return new WaitForSeconds(so.intervalBetweenTexts);
            }
        }

        uiMonologue.Hide();
        
        float waitHide = uiMonologue.AnimationDuration > 0 ? uiMonologue.AnimationDuration : 0.1f;
        yield return new WaitForSeconds(waitHide);

        isPlaying = false;
        currentMonologueCoroutine = null;
        
        onComplete?.Invoke();
    }
}