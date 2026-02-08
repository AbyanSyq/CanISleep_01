using UnityEngine;

public class PlayerHealth : Health
{
    [Header("Audio Settings")]
    [SerializeField] private PlayerAudioHandler playerAudioHandler;

    void Awake()
    {
        playerAudioHandler = GetComponent<PlayerAudioHandler>();
    }
    protected override void Die()
    {
        base.Die();
        playerAudioHandler.PlayDieSound();
        // Tambahkan logika khusus untuk kematian pemain di sini, seperti memicu animasi kematian atau menampilkan layar game over.
        Debug.Log("Player has died.");
        GameplayManager.Instance.HandlePlayerDeath();
    }
}
