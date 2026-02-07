using UnityEngine;
using UnityEngine.InputSystem;

public class RotationPuzzleHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private string animationParameter = "LightParam";
    [SerializeField] private PlayerInputHandler2D inputHandler;

    [Header("Settings")]
    [Range(0, 1)] public float rotationProgress = 0f;
    [Tooltip("Semakin besar, semakin cepat penuh (360 derajat * sensitivity = progres)")]
    public float sensitivity = 0.001f;
    public float decayRate = 0.05f; // Progres berkurang jika diam

    [Header("Haptics Settings")]
    public bool useVibration = true;
    [Range(0, 1)] public float vibrationIntensity = 0.2f;

    [Header("Rotation Direction")]
    public bool clockwiseOnly = true;

    private float _lastAngle = 0f;
    private bool _isFirstTouch = true;

    private void Update()
    {
        if (inputHandler == null) return;

        Vector2 input = inputHandler.look;

        if (input.magnitude > 0.5f && rotationProgress < 1f)
        {
            ProcessRotation(input);
            TriggerVibration(vibrationIntensity, vibrationIntensity);
        }
        else
        {
            ResetRotationState();
            ApplyDecay();
            StopVibration(); // Matikan getar saat analog dilepas atau progres selesai
        }
    }
    private void LateUpdate()
    {
        animator.SetFloat(animationParameter, rotationProgress);
    }

    private void OnDisable()
    {
        StopVibration();
    }

    private void ProcessRotation(Vector2 input)
    {
        float currentAngle = Mathf.Atan2(input.y, input.x) * Mathf.Rad2Deg;

        if (!_isFirstTouch)
        {
            float deltaAngle = Mathf.DeltaAngle(_lastAngle, currentAngle);

            // Mathf.DeltaAngle menghasilkan:
            // Negatif (-) jika diputar searah jarum jam (CW)
            // Positif (+) jika diputar berlawanan jarum jam (CCW)

            if (clockwiseOnly)
            {
                // Hanya tambah progres jika deltaAngle negatif (Searah jarum jam)
                if (deltaAngle < 0) 
                {
                    rotationProgress += Mathf.Abs(deltaAngle) * sensitivity;
                }
            }
            else
            {
                // Hanya tambah progres jika deltaAngle positif (Berlawanan jarum jam)
                if (deltaAngle > 0) 
                {
                    rotationProgress += deltaAngle * sensitivity;
                }
            }

            rotationProgress = Mathf.Clamp01(rotationProgress);

            if (rotationProgress >= 1f)
            {
                OnPuzzleComplete();
            }
        }

        _lastAngle = currentAngle;
        _isFirstTouch = false;
    }

    private void ResetRotationState()
    {
        _isFirstTouch = true;
    }

    private void ApplyDecay()
    {
        if (rotationProgress > 0 && rotationProgress < 1)
        {
            rotationProgress -= decayRate * Time.deltaTime;
            rotationProgress = Mathf.Max(0, rotationProgress);
        }
    }

    private void OnPuzzleComplete()
    {
        Debug.Log("Puzzle Berhasil Diselesaikan!");
        // Tambahkan logika kemenangan atau buka pintu di sini
    }


    private void TriggerVibration(float leftMotor, float rightMotor)
    {
        if (!useVibration) return;

        // Mengambil gamepad yang sedang aktif digunakan
        var gamepad = Gamepad.current;
        if (gamepad != null)
        {
            // SetMotorSpeeds(lowFrequencyMotor, highFrequencyMotor)
            // Left: Getaran berat/kasar, Right: Getaran tajam/halus
            gamepad.SetMotorSpeeds(leftMotor, rightMotor);
        }
    }

    private void StopVibration()
    {
        var gamepad = Gamepad.current;
        if (gamepad != null)
        {
            gamepad.SetMotorSpeeds(0f, 0f);
        }
    }
}