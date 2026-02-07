using UnityEngine;
using UnityEngine.Rendering;

public class Spawner : MonoBehaviour
{
    [Header("Player Spawn")]
    [SerializeField] private Transform playerSpawnPoint;
    [SerializeField] private Health playerHealth;

    [Header("Enemy Spawn")]
    [SerializeField] private Transform enemySpawnPoint;
    [SerializeField] private Health enemyHealth;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        GameplayManager.Instance.OnGameplayStart += Spawn;
    }

    void OnDisable()
    {
        GameplayManager.Instance.OnGameplayStart -= Spawn;
    }

    public void Spawn()
    {
        if(playerHealth == null)
        {
            playerHealth.transform.position = playerSpawnPoint.position;
            playerHealth.IncreaseHealth(playerHealth.MaxHealth);
            Debug.LogWarning("PlayerHealth or EnemyHealth is not assigned in Spawner.");
            return;
        }
        if(enemyHealth == null)
        {
            enemyHealth.transform.position = enemySpawnPoint.position;
            enemyHealth.IncreaseHealth(enemyHealth.MaxHealth);
            Debug.LogWarning("PlayerHealth or EnemyHealth is not assigned in Spawner.");
            return;
        }
    }


}
