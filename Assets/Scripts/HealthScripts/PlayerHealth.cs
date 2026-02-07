using UnityEngine;

public class PlayerHealth : Health
{
    protected override void Die()
    {
        base.Die();
        // Tambahkan logika khusus untuk kematian pemain di sini, seperti memicu animasi kematian atau menampilkan layar game over.
        Debug.Log("Player has died.");
    }
}
