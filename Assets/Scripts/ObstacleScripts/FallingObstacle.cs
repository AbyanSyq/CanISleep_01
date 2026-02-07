using UnityEngine;
using DG.Tweening;
using SmoothShakeFree;

public class FallingObstacle : MonoBehaviour
{
    [SerializeField] private float rotationDuration = 0.3f;
    [SerializeField] private float fallDuration = 0.5f;
    [SerializeField] private SmoothShake _shake;
    [SerializeField] private bool _isFalling = false;
    [SerializeField] private FallingObstacleController _controller;

    // Gunakan Vector3 untuk menyimpan posisi lokal asli
    private Vector3 _initialLocalPos;

    private void Awake()
    {
        _shake = GetComponent<SmoothShake>();
    }

    private void Start()
    {
        // Simpan posisi LOKAL (relatif terhadap parent)
        _initialLocalPos = transform.localPosition;
    }

    public void SetController(FallingObstacleController controller)
    {
        _controller = controller;
    }

    public void Shake()
    {
        if (_isFalling) return;
        _shake?.StartShake();
    }

    public void Fall()
    {
        if (_isFalling) return;
        _isFalling = true;

        // Berhenti shake dan reset posisi ke titik awal sebelum animasi jatuh
        _shake?.ForceStop();
        
        // Penting: Reset posisi ke posisi lokal awal agar tidak ada offset dari Shake
        transform.localPosition = _initialLocalPos;

        // Tentukan target jatuh di sumbu Y lokal
        float targetLocalY = _initialLocalPos.y - 1f;

        
        // Gunakan DOLocalMove agar tetap konsisten di dalam Parent
        transform.DOLocalRotate(new Vector3(0, 0, Random.Range(-20f, 20f)), 0.3f).OnComplete(() => gameObject.GetComponent<Collider2D>().enabled = false);
        transform.DOLocalMoveY(targetLocalY, 0.5f)
            .SetEase(Ease.InBack)
            .OnComplete(() => gameObject.SetActive(false));

        // Animasi rotasi (Local Rotation juga lebih aman)
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (_isFalling) return;

        PlayerInputHandler2D player = collision.gameObject.GetComponent<PlayerInputHandler2D>();

        if (player != null)
        {
            if (player.move.magnitude > 0.8f) 
            {
                _controller.TriggerGroupFall(this);
            }
            else if (player.move.magnitude > 0.3f)
            {
                Shake();
            }
        }
    }
    public void ResetObstacle()
    {
        // Hentikan semua animasi DOTween yang sedang berjalan pada objek ini
        transform.DOKill(); 
        
        _isFalling = false;
        
        // Aktifkan kembali objek jika mati
        gameObject.SetActive(true);
        gameObject.GetComponent<Collider2D>().enabled = true;
        
        // Kembalikan ke posisi dan rotasi awal dengan animasi halus (atau instan)
        transform.localPosition = _initialLocalPos;
        transform.localRotation = Quaternion.identity;
    }
}