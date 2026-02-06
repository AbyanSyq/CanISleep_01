using UnityEngine;
using UnityEngine.Events;

// Memastikan komponen Input Handler ada di object ini (opsional, tapi bagus untuk safety)
[RequireComponent(typeof(PlayerInputHandler2D))] 
public class InteractHandler2D : MonoBehaviour
{
    [Header("Interact Settings")]
    [SerializeField] private Transform detectionStartPoint;
    [SerializeField] private float detectionRadius = 0.5f;
    [SerializeField] private LayerMask interactableLayer = ~0;
    
    [Header("Debug Info")]
    [SerializeField] private Object currentInteractableObject; 
    private IInteractable currentInteractable = null;

    [Header("Control")]
    [SerializeField] private bool enableInteraction = true;

    // Buffer Physics
    private readonly Collider2D[] _hitColliders = new Collider2D[10]; 

    // REFERENCE KE INPUT HANDLER
    [Header("References")]
    [SerializeField] private PlayerInputHandler2D inputHandler; 

    [Header("Events")]
    public UnityEvent<IInteractable> OnCurrentInteractableChanged;

    private void Awake()
    {
        // Otomatis cari input handler jika belum di-assign di Inspector
        if (inputHandler == null)
            inputHandler = GetComponent<PlayerInputHandler2D>();
    }

    private void OnEnable()
    {
        // SUBSCRIBE: Mendengarkan event dari Input Handler
        if (inputHandler != null)
        {
            inputHandler.OnInteractEvent += Interact;
        }
    }

    private void OnDisable()
    {
        // UNSUBSCRIBE: Wajib dilakukan agar tidak memory leak
        if (inputHandler != null)
        {
            inputHandler.OnInteractEvent -= Interact;
        }
    }

    private void Update()
    {
        if (!enableInteraction) return;
        HandleInteractDetection();
    }

    // Method ini dipanggil otomatis saat PlayerInputHandler2D mendeteksi tombol tekan
    public void Interact()
    {
        if (!enableInteraction || currentInteractable == null) return;
        Debug.Log("Interacting with: " + currentInteractableObject.name);

        currentInteractable.Interact();
    }

    public void SetInteractionActive(bool value)
    {
        enableInteraction = value;
        if (!value && currentInteractable != null) 
        {
            ClearCurrentInteractable();
        }
    }

    private void HandleInteractDetection()
    {
        int hitCount = Physics2D.OverlapCircleNonAlloc(
            detectionStartPoint.position,
            detectionRadius,
            _hitColliders,
            interactableLayer
        );

        IInteractable foundInteractable = GetClosestInteractable(hitCount);

        if (foundInteractable != null)
        {
            if (currentInteractable != foundInteractable)
            {
                SetCurrentInteractable(foundInteractable);
            }
        }
        else if (currentInteractable != null)
        {
            ClearCurrentInteractable();
        }
    }

    private IInteractable GetClosestInteractable(int hitCount)
    {
        IInteractable closestInteractable = null;
        float closestDistanceSqr = float.MaxValue;
        Vector2 origin = detectionStartPoint.position;

        for (int i = 0; i < hitCount; i++)
        {
            Collider2D hitCollider = _hitColliders[i];

            if (!hitCollider.TryGetComponent(out IInteractable interactable))
                continue;

            if (!interactable.IsCanInteract())
                continue;

            float distanceSqr = ((Vector2)hitCollider.transform.position - origin).sqrMagnitude;

            if (distanceSqr < closestDistanceSqr)
            {
                closestDistanceSqr = distanceSqr;
                closestInteractable = interactable;
            }
        }

        return closestInteractable;
    }

    private void SetCurrentInteractable(IInteractable interactable)
    {
        currentInteractable?.SetCurrentInteractable(false);
        
        currentInteractable = interactable;
        currentInteractableObject = currentInteractable as Object;
        
        currentInteractable?.SetCurrentInteractable(true);

        OnCurrentInteractableChanged?.Invoke(currentInteractable);
    }

    private void ClearCurrentInteractable()
    {
        currentInteractable?.SetCurrentInteractable(false);
        currentInteractable = null;
        currentInteractableObject = null;
        OnCurrentInteractableChanged?.Invoke(null);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (detectionStartPoint == null) return;

        Gizmos.color = enableInteraction ? Color.green : Color.gray;
        Gizmos.DrawWireSphere(detectionStartPoint.position, detectionRadius);
    }
#endif
}