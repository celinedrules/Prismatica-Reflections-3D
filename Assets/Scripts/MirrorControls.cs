using UnityEngine;

public class MirrorControl : MonoBehaviour
{
    [Header("Rotation Settings")]
    [Tooltip("How many degrees the mirror rotates per click.")]
    public float rotationStep = 45f;

    // targetRotation stores the logical angle we want to reach
    private Vector3 targetRotation;

    void Start()
    {
        // Initialize targetRotation with the current editor placement
        targetRotation = transform.eulerAngles;
    }

    /// <summary>
    /// Unity callback for mouse/touch input. 
    /// Requires a Collider on the Mirror object.
    /// </summary>
    void OnMouseDown()
    {
        RotateMirror();
    }

    void RotateMirror()
    {
        // Increment the Y-axis rotation by our fixed step
        targetRotation.y += rotationStep;

        // Apply the rotation immediately
        // Note: For this novice implementation, we use direct assignment.
        // This ensures the Beam System's Raycast detects the change instantly.
        transform.eulerAngles = targetRotation;
        
        // Integration Point: Trigger a subtle haptic or sound here 
        // to match the 'Celestial Ruin' aesthetic.
        Debug.Log($"Mirror rotated to: {transform.eulerAngles.y}");
    }
}