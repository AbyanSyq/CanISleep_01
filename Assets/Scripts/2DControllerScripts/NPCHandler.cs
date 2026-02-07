using UnityEngine;

public class NPCHandler : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameplayManager.Instance.OnGameplayStart.AddListener(HandleNPCBehavior);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void HandleNPCBehavior()
    {
        gameObject.SetActive(false);
    }
}
