using UnityEngine;

public class ParallaxController : MonoBehaviour
{
    private Transform cam;
    private float lastCamX;

    private GameObject[] backgrounds;
    private Material[] materials;
    private Parallax[] parallaxScripts;

    void Start()
    {
        if (Camera.main == null) return;
        
        cam = Camera.main.transform;
        lastCamX = cam.position.x;

        int backCount = transform.childCount;
        backgrounds = new GameObject[backCount];
        materials = new Material[backCount];
        parallaxScripts = new Parallax[backCount];

        for (int i = 0; i < backCount; i++)
        {
            backgrounds[i] = transform.GetChild(i).gameObject;
            
            Renderer rend = backgrounds[i].GetComponent<Renderer>();
            if (rend != null)
            {
                materials[i] = rend.material;
            }
            
            parallaxScripts[i] = backgrounds[i].GetComponent<Parallax>();
        }
    }

    // Menggunakan LateUpdate adalah kunci agar berjalan SETELAH kamera bergerak
    private void LateUpdate()
    {
        if (cam == null) return;

        // Gunakan posisi kamera saat ini secara langsung
        float currentCamX = cam.position.x;
        float deltaX = currentCamX - lastCamX;

        // JANGAN gerakkan transform.position secara kasar jika tidak perlu.
        // Cukup kunci posisi parent ke kamera, tapi pastikan nilai Z tetap.
        Vector3 newPos = transform.position;
        newPos.x = currentCamX;
        transform.position = newPos;

        for (int i = 0; i < backgrounds.Length; i++)
        {
            if (parallaxScripts[i] == null || materials[i] == null) continue;

            float currentSpeed = parallaxScripts[i].speed;

            // Menggunakan properti khusus "_MainTex" lebih aman untuk shader standar
            // Atau gunakan mainTextureOffset
            Vector2 offset = materials[i].mainTextureOffset;
            offset.x += deltaX * currentSpeed;
            materials[i].mainTextureOffset = offset;
        }

        lastCamX = currentCamX;
    }
}