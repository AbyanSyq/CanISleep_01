using UnityEngine;

public class MonologueTriggerHandler : MonoBehaviour
{
    [SerializeField] private MonologueSO monologueSO;

    public void TriggerMonologue()
    {
        MonologueManager.Instance.PlayMonologue(monologueSO);
    }
}
