using UnityEngine;
using System.Collections.Generic;

public class CorpseGenerator : SingletonMonoBehaviour<CorpseGenerator>
{
    [SerializeField] private GameObject corpsePrefab;
    [SerializeField] private int poolSize = 5;
    private Queue<GameObject> corpsePool = new Queue<GameObject>();

    protected override void Awake()
    {
        base.Awake();
        InitializePool();
    }

    void OnEnable()
    {
        GameplayManager.Instance.OnGameplayRestart.AddListener(GeneratePlayerCorpse);
    }
    void OnDisable()
    {
        GameplayManager.Instance.OnGameplayRestart.RemoveListener(GeneratePlayerCorpse);
    }

    private void InitializePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(corpsePrefab);
            obj.SetActive(false);
            corpsePool.Enqueue(obj);
        }
    }
    public void GeneratePlayerCorpse()
    {
        GenerateCorpse(GameplayManager.Instance.playerInputHandler.transform.position); 
    }
    public void GenerateCorpse(Vector3 position)
    {
        if (corpsePool.Count == 0) return;

        GameObject oldestCorpse = corpsePool.Dequeue();

        oldestCorpse.transform.position = position;
        oldestCorpse.SetActive(true);

        corpsePool.Enqueue(oldestCorpse);
    }
}