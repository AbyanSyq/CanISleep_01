using System;
using UnityEngine;
using UnityEngine.Events;

public class GameplayManager : SingletonMonoBehaviour<GameplayManager>
{
    public MonologueSO monologueSO;

    
    public event Action OnGameplayStart;

    private void Start()
    {
        StartLevel();
    }
    public void StartLevel()
    {
        OnGameplayStart?.Invoke();
        MonologueManager.Instance.PlayMonologue(monologueSO);
    }
}
