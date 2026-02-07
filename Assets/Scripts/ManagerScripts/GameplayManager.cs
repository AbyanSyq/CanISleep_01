using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class GameplayManager : SingletonMonoBehaviour<GameplayManager>
{
    [Header("Game Settings")]
    public float restartDelay = 2f;
    public bool isRestartSceneWhenPlayerDie = true;

    [Header("Cinematics & Cameras")]
    public GameObject explanationCamera; // Kamera A (NPC/Intro)
    public GameObject playerCamera;      // Kamera B (Player)
    public MonologueSO openingMonologue; // Data percakapan intro

    [Header("References")]
    public Spawner spawner;
    public PlayerInputHandler2D playerInputHandler;

    [Header("Events")]
    public UnityEvent OnGameplayStart;
    public UnityEvent OnPlayerDeath;
    public UnityEvent OnGameplayRestart;

    private void Start()
    {
        // Jangan langsung StartGameplay, tapi mulai sequence intro dulu
        StartOpeningSequence();
    }

    // ---------------------------------------------------------
    // PHASE 1: OPENING SEQUENCE (NPC Explanation)
    // ---------------------------------------------------------
    private void StartOpeningSequence()
    {
        // 1. Setup Kamera: Aktifkan kamera penjelasan (Cam A)
        SwitchCamera(explanationCamera);

        // 2. Disable Input: Pemain tidak boleh bergerak
        TogglePlayerInput(false);

        // 3. Mulai Monolog
        // NOTE: Kita butuh callback (Action) agar tahu kapan monolog selesai.
        // Pastikan script MonologueManager kamu support parameter kedua: PlayMonologue(SO, Action onComplete)
        if(openingMonologue != null)
            MonologueManager.Instance.PlayMonologue(openingMonologue, OnMonologueFinished);
        else
            OnMonologueFinished();
    }

    private void OnMonologueFinished()
    {
        // Setelah NPC selesai bicara, baru mulai gameplay sesungguhnya
        StartGameplay();
    }

    // ---------------------------------------------------------
    // PHASE 2: ACTUAL GAMEPLAY (Player Control)
    // ---------------------------------------------------------
    public void StartGameplay()
    {
        // 1. Setup Kamera: Pindah ke kamera player (Cam B)
        SwitchCamera(playerCamera);

        // 2. Enable Input: Pemain boleh bergerak sekarang
        TogglePlayerInput(true);

        // 3. Jalankan Event & Spawn
        OnGameplayStart?.Invoke();
        spawner.Spawn();    
    }

    // ---------------------------------------------------------
    // PHASE 3: RESTART (No Camera Switch)
    // ---------------------------------------------------------
    public void RestartGameplay()
    {
        // Logika restart tanpa mengganti kamera (tetap di Player Camera)
        // dan tanpa memutar ulang monolog intro NPC.
        
        OnGameplayRestart?.Invoke();
        spawner.Spawn();
        
        // Pastikan input nyala kembali jika sebelumnya dimatikan saat mati
        TogglePlayerInput(true); 
    }

    // ---------------------------------------------------------
    // PLAYER DEATH HANDLING
    // ---------------------------------------------------------
    public void HandlePlayerDeath()
    {
        OnPlayerDeath?.Invoke();
        
        // Opsional: Matikan input saat mati agar mayat tidak bisa digerakkan
        TogglePlayerInput(false);

        if (isRestartSceneWhenPlayerDie)
        {
            TransitionManager.Instance.RestartScene(restartDelay);
            return;
        }

        Debug.Log("Handling player death in GameplayManager.");
        StartCoroutine(RestartGameplayAfterDelay(restartDelay));
    }

    public IEnumerator RestartGameplayAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        RestartGameplay();
    }

    // ---------------------------------------------------------
    // HELPER METHODS (Supaya Rapi)
    // ---------------------------------------------------------
    
    private void TogglePlayerInput(bool isActive)
    {
        if (playerInputHandler != null)
        {
            playerInputHandler.enabled = isActive;
            
            // Stop pergerakan fisik jika input dimatikan (supaya tidak meluncur)
            if (!isActive && playerInputHandler.TryGetComponent<Rigidbody2D>(out var rb))
            {
                rb.linearVelocity = Vector2.zero; 
                rb.angularVelocity = 0f;
            }
        }
    }

    private void SwitchCamera(GameObject targetCamera)
    {
        if (explanationCamera != null) explanationCamera.SetActive(false);
        if (playerCamera != null) playerCamera.SetActive(false);

        if (targetCamera != null) targetCamera.SetActive(true);
    }
}