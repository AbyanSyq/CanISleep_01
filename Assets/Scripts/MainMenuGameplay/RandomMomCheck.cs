using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; // Pastikan sudah menginstal Input System Package

public class RamdomMomCheck : MonoBehaviour
{
    [Header("Settings")]
    public GameObject[] targetObjectActive; // Masukkan 2 atau lebih object di sini
    public GameObject[] targetObjectDeactive;
    public float minWaitTime = 2f;
    public float maxWaitTime = 4f;
    public float hintDuration = 1f; // Durasi getaran sebelum muncul
    public float minActiveDuration = 1f; // Berapa lama object aktif
    public float maxActiveDuration = 2f;

    public bool isMomCheckActive = false;

    private void Start()
    {
        foreach (var obj in targetObjectActive) obj.SetActive(false);
        foreach (var obj in targetObjectDeactive) obj.SetActive(true);
        
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            // 1. Tunggu waktu acak sebelum siklus dimulai
            yield return new WaitForSeconds(Random.Range(minWaitTime, maxWaitTime));

            // 2. Berikan Hint (Getaran Non-Linear)
            yield return StartCoroutine(VibrateController(hintDuration));

            // 3. Pilih 2 object secara acak untuk dinyalakan
            ActivateAll();

            // 4. Tunggu selama object aktif, lalu matikan semua
            yield return new WaitForSeconds(Random.Range(minActiveDuration, maxActiveDuration));
            DeactivateAll();
        }
    }

    IEnumerator VibrateController(float duration)
    {
        Gamepad gamepad = Gamepad.current;
        if (gamepad == null) yield break;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // Efek Non-Linear: Menggunakan t^3 (Cubic Ease-In)
            // Getaran akan terasa sangat pelan di awal, lalu melonjak kencang di akhir
            float intensity = Mathf.Pow(t, 3); 

            gamepad.SetMotorSpeeds(intensity * 0.2f, intensity); // Low & High frequency motors
            yield return null;
        }

        // Hentikan getaran saat object muncul
        gamepad.ResetHaptics();
    }

    void ActivateAll()
    {
        isMomCheckActive = true;
        foreach (var obj in targetObjectActive) obj.SetActive(true);
        foreach (var obj in targetObjectDeactive) obj.SetActive(false);
    }

    void DeactivateAll()
    {
        isMomCheckActive = false;
        foreach (var obj in targetObjectActive) obj.SetActive(false);
        foreach (var obj in targetObjectDeactive) obj.SetActive(true);
    }
    
    private void OnDisable() 
    {
        // Safety: Hentikan getaran jika script/object dimatikan
        Gamepad.current?.ResetHaptics();
    }
}