using UnityEngine;

public class GiveDamageHandler2D : MonoBehaviour
{
    public float damageAmount = 10f;

    // Menggunakan Collision2D untuk objek dengan Rigidbody2D dan Collider2D (non-trigger)
    public void OnCollisionEnter2D(Collision2D collision)
    {
        // Cari interface pada objek yang ditabrak
        IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
        
        if (damageable != null)
        {
            // Pada 2D, posisi kontak bisa diambil dari collision.otherCollider.transform.position 
            // atau titik kontak spesifik: collision.GetContact(0).point
            damageable.TakeDamage(damageAmount, collision.transform.position);
        }
    }

    // Menggunakan Collider2D untuk objek dengan Collider2D yang dicentang "Is Trigger"
    public void OnTriggerEnter2D(Collider2D other)
    {
        IDamageable damageable = other.gameObject.GetComponent<IDamageable>();
        
        if (damageable != null)
        {
            damageable.TakeDamage(damageAmount, other.transform.position);
        }
    }
}