using UnityEngine;
using DG.Tweening;
using Ami.BroAudio;
using Unity.VisualScripting;
using UnityEngine.Events;

public class TakeDamageHandler : MonoBehaviour, IDamageable
{
    [Header("Health Settings")]
    public Health healthComponent;
    public float damageMultiplier = 1f; // Typo fixed from Multiplaier
    public float damageReduction = 15f;

    [Header("Visual Settings")]
    public SpriteRenderer spriteRenderer; // Menggunakan SpriteRenderer untuk 2D
    public Color flashColor = Color.red;
    private Color originalColor;

    [Header("Audio Settings")]
    [SerializeField] private SoundID Damage_Sound;

    [Header("Event")]
    public UnityEvent OnTakeDamageEvent;

    private void Awake()
    {
        // Cache warna asli jika spriteRenderer tidak null
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        
        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;
    }

    public virtual void TakeDamage(float amount, Vector3 dmgImpactPos)
    {
        if (healthComponent != null)
        {
            // Menghitung damage akhir
            float finalDamage = amount * damageMultiplier;
            
            // Catatan: Di script asli Anda menggunakan 'damageReduction' sebagai nilai tetap
            // Jika ingin mengurangi health berdasarkan damage yang masuk, gunakan 'finalDamage'.
            healthComponent.ReduceHealth(damageReduction); 

            Flash();
            PlayDamageSound();
            OnTakeDamageEvent?.Invoke();
            
            Debug.Log($"{gameObject.name} took {finalDamage} damage.");
        }
    }

    public void Flash()
    {
        if (spriteRenderer == null) return;

        // Menghentikan tween warna sebelumnya agar tidak bertumpuk
        spriteRenderer.DOKill();
        
        // Memastikan warna kembali ke awal sebelum mulai (opsional tapi aman)
        spriteRenderer.color = originalColor;

        // Flash Effect menggunakan DOTween
        spriteRenderer.DOColor(flashColor, 0.05f)
            .SetLoops(2, LoopType.Yoyo)
            .SetEase(Ease.InOutQuad);
    }

    private void PlayDamageSound()
    {
        if (Damage_Sound.IsValid())
            BroAudio.Play(Damage_Sound);
    }
}