using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class GameplayManager : SingletonMonoBehaviour<GameplayManager>
{
    public float restartDelay = 2f;
    public MonologueSO monologueSO;
    public Spawner spawner;

    public bool isRestartSceneWhenPlayerDie = true;

    [Header("Player")]
    public PlayerInputHandler2D playerInputHandler;
 
    
    public UnityEvent OnGameplayStart;
    public UnityEvent OnPlayerDeath;
    public UnityEvent OnGameplayRestart;

    private void Start()
    {
        StartGameplay();
    }
    public void StartGameplay()
    {
        OnGameplayStart?.Invoke();
        spawner.Spawn();    
        MonologueManager.Instance.PlayMonologue(monologueSO);
    }
    public void RestartGameplay()
    {
        OnGameplayRestart?.Invoke();
        spawner.Spawn();    
        MonologueManager.Instance.PlayMonologue(monologueSO);
        
    }
    public void HandlePlayerDeath()
    {
        OnPlayerDeath?.Invoke();
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
        // yield return new WaitForSecondsRealtime(delay/2);
        // Time.timeScale = 0f;
        // yield return new WaitForSecondsRealtime(delay/2);
        // Time.timeScale = 1f;
        yield return new WaitForSecondsRealtime(delay);
        RestartGameplay();
    }
}
