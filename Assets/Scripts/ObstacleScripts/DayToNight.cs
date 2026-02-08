using UnityEngine;
using UnityEngine.UI;

public class DayToNight : MonoBehaviour
{
    [Header("Sun & Moon Settings")]
    [SerializeField] private Transform sunAndMoonTransform;
    [SerializeField, Range(0f, 1f)] private float rotationStart = 0.3f; // Mulai gerak di 0.3
    [SerializeField, Range(0f, 1f)] private float rotationEnd = 0.7f;   // Selesai gerak di 0.7
    [SerializeField] private Vector3 dayRotation; 
    [SerializeField] private Vector3 nightRotation; 
    
    
    [Header("Sky Settings")]
    [SerializeField] SpriteRenderer skyImage;
    [SerializeField, Range(0f, 1f)] private float skyColorStart = 0f;
    [SerializeField, Range(0f, 1f)] private float skyColorEnd = 1f;
    [SerializeField] Color dayColor;
    [SerializeField] Color nightColor;

    [Header("Platform Settings")]
    [SerializeField] SpriteRenderer[] platformImages;
    [SerializeField, Range(0f, 1f)] private float platformColorStart = 0.2f;
    [SerializeField, Range(0f, 1f)] private float platformColorEnd = 0.8f;
    [SerializeField] Color dayPlatformColor;
    [SerializeField] Color nightPlatformColor;

    [Header("Obstacle Settings")]
    [SerializeField] SpriteRenderer[] obstacleImage;
    [SerializeField, Range(0f, 1f)] private float obstacleColorStart = 0.4f;
    [SerializeField, Range(0f, 1f)] private float obstacleColorEnd = 0.6f;
    [SerializeField] Color dayObstacleColor;
    [SerializeField] Color nightObstacleColor;
    [Space]
    [SerializeField, Range(0f, 1f)] private float obstacleColliderDisableAt = 0.7f;
    [SerializeField] Collider2D[] obstacleCollider;

    [Header("References")]
    [SerializeField] private RotationPuzzleHandler rotationPuzzleHandler;

    private void Update()
    {
        if (rotationPuzzleHandler != null)
        {
            float progress = rotationPuzzleHandler.rotationProgress;
            UpdateEnvironment(progress);
        }
    }

    private void UpdateEnvironment(float t)
    {
        float rotationT = Mathf.InverseLerp(rotationStart, rotationEnd, t);

        sunAndMoonTransform.localRotation = Quaternion.Lerp(

            Quaternion.Euler(dayRotation),

            Quaternion.Euler(nightRotation),

            rotationT

        );

        // --- 2. Warna Langit ---
        float skyT = Mathf.InverseLerp(skyColorStart, skyColorEnd, t);
        skyImage.color = Color.Lerp(dayColor, nightColor, skyT);

        // --- 3. Warna Platform ---
        float platformT = Mathf.InverseLerp(platformColorStart, platformColorEnd, t);
        foreach (SpriteRenderer img in platformImages)
        {
            if (img != null)
                img.color = Color.Lerp(dayPlatformColor, nightPlatformColor, platformT);
        }

        // --- 4. Warna Obstacle ---
        float obstacleT = Mathf.InverseLerp(obstacleColorStart, obstacleColorEnd, t);
        if (obstacleImage != null)
        {
            foreach (SpriteRenderer img in obstacleImage)
                if (img != null)
                    img.color = Color.Lerp(dayObstacleColor, nightObstacleColor, obstacleT);
        }

        // --- 5. Collider Toggle ---
        // Collider mati jika progress sudah melewati batas tertentu
        if (obstacleCollider != null)
        {
            foreach (Collider2D col in obstacleCollider)
                if (col != null)
                    col.enabled = (t < obstacleColliderDisableAt);
        }
    }
}