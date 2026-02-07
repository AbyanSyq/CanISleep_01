using UnityEngine;

public class CorpseGenerator : SingletonMonoBehaviour<CorpseGenerator>
{
    [SerializeField] private GameObject corpsePrefab;

    public void GenerateCorpse(Vector3 position)
    {
        Instantiate(corpsePrefab, position, corpsePrefab.transform.rotation);
        Debug.Log("Corpse generated at position: " + position);
    }
}
