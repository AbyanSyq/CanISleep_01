using UnityEngine;

public class MaterialOffsetLooper : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Renderer targetRenderer;
    [SerializeField] private Vector2 offsetSpeed = new Vector2(0.001f, 0f); // Bisa mengatur kecepatan X dan Y
    
    private Material targetMaterial;
    private Vector2 currentOffset = Vector2.zero;

    private void Start()
    {
        // Jika renderer tidak diisi di inspector, coba ambil dari object ini
        if (targetRenderer == null)
            targetRenderer = GetComponent<Renderer>();

        if (targetRenderer != null)
        {
            // Menggunakan .material akan membuat instance unik agar tidak merubah semua object lain
            targetMaterial = targetRenderer.material;
        }
        else
        {
            Debug.LogError("Renderer tidak ditemukan pada " + gameObject.name);
            enabled = false;
        }
    }

    private void Update()
    {
        // Menambah offset berdasarkan waktu dan kecepatan
        currentOffset += offsetSpeed * Time.deltaTime;

        // Looping nilai dari 0 ke 1 secara otomatis
        // Mathf.Repeat memastikan nilai kembali ke 0 setelah mencapai 1
        float offsetX = Mathf.Repeat(currentOffset.x, 1f);
        float offsetY = Mathf.Repeat(currentOffset.y, 1f);

        // Update offset ke material
        // "_MainTex" adalah nama default untuk Albedo/Main Texture di shader Unity
        targetMaterial.SetTextureOffset("_MainTex", new Vector2(offsetX, offsetY));
    }

    private void OnDestroy()
    {
        // Membersihkan material instance untuk mencegah memory leak
        if (targetMaterial != null)
        {
            Destroy(targetMaterial);
        }
    }
}