using System;
using UnityEngine;
using UnityEngine.Events;

public class GameplayManager : SingletonMonoBehaviour<GameplayManager>
{
    public float restartDelay = 2f;
    public MonologueSO monologueSO;
    public Spawner spawner;
 
    
    public event Action OnGameplayStart;

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
    public void HandlePlayerDeath()
    {
        Debug.Log("Handling player death in GameplayManager.");
        Invoke(nameof(StartGameplay), restartDelay); // Delay sebelum restart level
    }
}
