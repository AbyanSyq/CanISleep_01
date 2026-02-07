using UnityEngine;

public class CorpseSpawnHandler : MonoBehaviour
{
    [SerializeField] Transform playerTransform;
    public void SpawnCorpseAtPosition()
    {
       CorpseGenerator.Instance.GenerateCorpse(playerTransform.position);
    }
}
