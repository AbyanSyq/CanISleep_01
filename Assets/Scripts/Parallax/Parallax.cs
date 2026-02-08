using UnityEngine;

public class Parallax : MonoBehaviour
{
    [Tooltip("Semakin kecil nilainya, semakin lambat gerakannya (efek jauh). 0 = diam, 1 = ikut kamera sepenuhnya.")]
    [Range(0f, 1f)]
    public float speed = 0.1f;

    // Kita tidak butuh Update() di sini karena akan diatur oleh Controller
}