using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Ami.BroAudio;
using UnityEngine.Rendering;
public static class ShakeAnalogProgressEvents
{
    public static Action OnShakeProgressComplete;
    public static Action<float> OnShakeProgressUpdate;
}
public class ShakeAnalogProgress : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private string animationParameter = "LightParam";
    [SerializeField] private string animationParameterIsProgressing = "IsProgressing";
    [SerializeField] private string animationParameterIsCompleteTrigger = "IsComplete";
    [SerializeField] private PlayerInputHandler2D inputHandler;

    [Header("Audio")]
    private IAudioPlayer currentAudioPlayer;
    public SoundID progressingAudio;
    public SoundID idleAudio;

    private bool _lastProgressState;

    [Header("Settings")]
    [Range(0, 1)] public float progress = 0f;
    public bool isProgressing = false;
    [Tooltip("Sensitivitas gerakan. Semakin besar, semakin cepat penuh.")]
    public float sensitivity = 0.5f;
    public float decayRate = 0.1f;

    [Tooltip("Batas minimal analog dianggap bergerak (untuk menghindari drift)")]
    public float deadzone = 0.1f;

    [Header("Haptics Settings")]
    public bool useVibration = true;
    [Range(0, 1)] public float vibrationIntensity = 0.2f;

    private Vector2 _lastMoveInput;
    private Vector2 _lastLookInput;

    public UnityEvent OnProgressComplete;
    public UnityEvent OnFoundedByMom;

    private void Update()
    {
        if (inputHandler == null) return;

        Vector2 currentMove = inputHandler.move; // Analog Kiri
        Vector2 currentLook = inputHandler.look; // Analog Kanan

        // Hitung seberapa jauh analog bergerak dari posisi frame sebelumnya
        float moveDelta = Vector2.Distance(currentMove, _lastMoveInput);
        float lookDelta = Vector2.Distance(currentLook, _lastLookInput);

        // Jika salah satu analog bergerak secara aktif
        if ((currentMove.magnitude > deadzone || currentLook.magnitude > deadzone) && progress < 1f)
        {
            isProgressing = true;
            ProcessShake(moveDelta, lookDelta);
            TriggerVibration(vibrationIntensity, vibrationIntensity);
        }
        else
        {
            isProgressing = false;
            ApplyDecay();
            StopVibration();
        }

        HandleProgressAudio();


        ShakeAnalogProgressEvents.OnShakeProgressUpdate?.Invoke(progress);

        // Simpan posisi input untuk dibandingkan di frame berikutnya
        _lastMoveInput = currentMove;
        _lastLookInput = currentLook;
    }

    private void LateUpdate()
    {
        if (animator != null)
        {
            animator.SetBool(animationParameterIsProgressing, isProgressing);
            animator.SetFloat(animationParameter, progress);
        }
    }

    private void OnDisable() => StopVibration();

    private void ProcessShake(float moveDelta, float lookDelta)
    {
        if (FindFirstObjectByType<RamdomMomCheck>().isMomCheckActive)
        {
            progress = 0f;
            OnFoundedByMom?.Invoke();
            return;
        }
        // Gabungkan pergerakan kedua analog
        // Pemain bisa menggerakkan satu atau keduanya, keduanya akan menambah progres
        float totalMovement = (moveDelta + lookDelta) * sensitivity * Time.deltaTime;

        progress += totalMovement;
        progress = Mathf.Clamp01(progress);



        if (progress >= 1f)
        {
            OnPuzzleComplete();
        }
    }

    private void ApplyDecay()
    {
        if (progress > 0 && progress < 1f)
        {
            progress -= decayRate * Time.deltaTime;
            progress = Mathf.Max(0, progress);
        }
    }

    private void OnPuzzleComplete()
    {
        Debug.Log("Puzzle Shake Berhasil!");
        StopVibration();
        OnProgressComplete?.Invoke();
        ShakeAnalogProgressEvents.OnShakeProgressComplete?.Invoke();
        if (animator != null)
        {
            animator.SetTrigger(animationParameterIsCompleteTrigger);
        }
    }

    private void TriggerVibration(float leftMotor, float rightMotor)
    {
        if (!useVibration) return;
        var gamepad = Gamepad.current;
        if (gamepad != null) gamepad.SetMotorSpeeds(leftMotor, rightMotor);
    }

    private void StopVibration()
    {
        var gamepad = Gamepad.current;
        if (gamepad != null) gamepad.SetMotorSpeeds(0f, 0f);
    }

    private void HandleProgressAudio()
    {
        // Kalau state berubah baru play sound
        if (_lastProgressState != isProgressing)
        {
            if (isProgressing)
            {
                // Audio A
                currentAudioPlayer?.Stop();
                currentAudioPlayer = BroAudio.Play(progressingAudio,0);
            }
            else
            {
                // Audio B
                currentAudioPlayer?.Stop();
                currentAudioPlayer = BroAudio.Play(idleAudio,0);
            }

            _lastProgressState = isProgressing;
        }
    }

}