using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FallingObstacleController : MonoBehaviour, ISpawner
{
    [Header("Settings")]
    public float delayBetweenFalls = 0.2f; 
    
    private List<FallingObstacle> _childObstacles = new List<FallingObstacle>();
    private bool _hasTriggered = false;

    private void Start()
    {
        // Ambil semua anak dan simpan urutannya
        foreach (Transform child in transform)
        {
            FallingObstacle obs = child.GetComponent<FallingObstacle>();
            if (obs != null)
            {
                _childObstacles.Add(obs);
                obs.SetController(this);
            }
        }
    }

    // Fungsi sekarang menerima parameter 'starter' untuk tahu siapa yang diinjak duluan
    public void TriggerGroupFall(FallingObstacle starter)
    {
        if (_hasTriggered) return;
        _hasTriggered = true;

        // Cari index objek yang diinjak
        int startIndex = _childObstacles.IndexOf(starter);

        if (startIndex != -1)
        {
            // Mulai jatuhkan ke arah kanan dan kiri secara bersamaan
            StartCoroutine(FallDirectional(startIndex, 1));  // Ke Kanan
            StartCoroutine(FallDirectional(startIndex - 1, -1)); // Ke Kiri
        }
    }

    IEnumerator FallDirectional(int startIndex, int direction)
    {
        int currentIndex = startIndex;

        // Terus berjalan selama index masih dalam batas list
        while (currentIndex >= 0 && currentIndex < _childObstacles.Count)
        {
            _childObstacles[currentIndex].Fall();
            
            // Bergerak ke index berikutnya berdasarkan arah (1 atau -1)
            currentIndex += direction;
            
            yield return new WaitForSeconds(delayBetweenFalls);
        }
    }
    public void Spawn()
    {
        StopAllCoroutines(); // Hentikan proses jatuh yang mungkin masih berjalan
        _hasTriggered = false;

        foreach (var obs in _childObstacles)
        {
            obs.ResetObstacle();
        }
    }
}