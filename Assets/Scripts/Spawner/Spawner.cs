using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
public interface ISpawner
{
    void Spawn();
}
public class Spawner : MonoBehaviour
{
    [Header("Player Spawn")]
    [SerializeField] private Transform playerSpawnPoint;
    [SerializeField] private Health playerHealth;

    [Header("Enemy Spawn")]
    [SerializeField] private Transform enemySpawnPoint;
    [SerializeField] private Health enemyHealth;

    public List<GameObject> spawners = new();
    public void Spawn()
    {
        foreach (var spawner in spawners)
        {
            spawner.GetComponent<ISpawner>()?.Spawn();
        }
        if(playerHealth != null)
        {
            playerHealth.transform.position = playerSpawnPoint.position;
            playerHealth.Revive();
        }
        if(enemyHealth != null)
        {
            enemyHealth.transform.position = enemySpawnPoint.position;
            enemyHealth.IncreaseHealth(enemyHealth.MaxHealth);
        }
    }


}
