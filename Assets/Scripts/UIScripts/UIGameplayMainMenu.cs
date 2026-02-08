using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIGameplayMainMenu : UIBase
{
    [SerializeField] private Image progressBar;
    [SerializeField] private UIBase progressBarUI;
    [Header("Hint UI Settings")]
    [SerializeField] private UIBase hintUI; // UI Petunjuk baru
    [SerializeField] private float idleThreshold = 3f; // Munculkan petunjuk jika diam selama X detik

    private Coroutine hintTimerCoroutine;
    private bool isGameStarted = false;

    void Start()
    {
        // Muncul di awal game
        ShowHint();
        isGameStarted = true;
    }

    void OnEnable()
    {
        ShakeAnalogProgressEvents.OnShakeProgressUpdate += UpdateProgressBar;
    }

    void OnDisable() // Typo sebelumnya ODisable sudah diperbaiki
    {
        ShakeAnalogProgressEvents.OnShakeProgressUpdate -= UpdateProgressBar;
    }

    public void UpdateProgressBar(float progress)
    {
        if (progressBar != null)
        {
            progressBar.fillAmount = progress;

            // Logika Progress Bar UI
            if (progress > 0f && progress < 1f)
            {
                StartCoroutine(SetProgressBarUI(true));
                
                // Jika ada pergerakan, sembunyikan petunjuk dan reset timer
                HideHint();
                ResetHintTimer();
            }
            else if (progress <= 0f)
            {
                StartCoroutine(SetProgressBarUI(false, 0.5f));
            }

            if (progress >= 0.99f)
            {
                StartCoroutine(SetProgressBarUI(false));
                HideHint(); // Pastikan petunjuk hilang saat selesai
                StopHintTimer();
            }
        }
    }

    // --- Logika UI Petunjuk ---

    private void ShowHint()
    {
        if (hintUI != null && !hintUI.isActive)
        {
            hintUI.Show();
        }
    }

    private void HideHint()
    {
        if (hintUI != null && hintUI.isActive)
        {
            hintUI.Hide();
        }
    }

    private void ResetHintTimer()
    {
        StopHintTimer();
        // Mulai menghitung: jika pemain diam selama 'idleThreshold', tunjukkan petunjuk
        hintTimerCoroutine = StartCoroutine(HintTimerRoutine());
    }

    private void StopHintTimer()
    {
        if (hintTimerCoroutine != null)
        {
            StopCoroutine(hintTimerCoroutine);
            hintTimerCoroutine = null;
        }
    }

    private IEnumerator HintTimerRoutine()
    {
        yield return new WaitForSeconds(idleThreshold);
        // Jika coroutine ini berhasil selesai (tidak di-stop oleh progress), munculkan petunjuk
        ShowHint();
    }

    // --- Logika Progress Bar (Existing) ---

    public IEnumerator SetProgressBarUI(bool state, float delay = 0f)
    {
        yield return new WaitForSeconds(delay);

        if (state)
        {
            if (progressBarUI.isActive) yield break;
            progressBarUI.Show();
        }
        else
        {
            if (!progressBarUI.isActive) yield break;
            progressBarUI.Hide();
        }
    }
}