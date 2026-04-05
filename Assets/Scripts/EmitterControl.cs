using UnityEngine;

public class EmitterControl : MonoBehaviour
{
    [Header("Rotation Settings")]
    [Tooltip("How many degrees the emitter rotates per click.")]
    [SerializeField] private float rotationStep = 45f;

    [Tooltip("Optional child transform to rotate. Leave empty to rotate this object.")]
    [SerializeField] private Transform rotatingPart;

    [Tooltip("Minimum allowed local Y angle.")]
    [SerializeField] private float minYAngle = -90f;

    [Tooltip("Maximum allowed local Y angle.")]
    [SerializeField] private float maxYAngle = 90f;

    // Stores the logical angle we want the emitter to reach.
    private float targetYRotation;

    private void Start()
    {
        if (rotatingPart == null)
        {
            rotatingPart = transform;
        }

        // Initialize from the current editor placement.
        targetYRotation = NormalizeAngle(rotatingPart.localEulerAngles.y);
    }

    /// <summary>
    /// Unity callback for mouse/touch input.
    /// Requires a Collider on the clicked object.
    /// </summary>
    private void OnMouseDown()
    {
        RotateEmitter();
    }

    private void RotateEmitter()
    {
        targetYRotation += rotationStep;

        // Wrap back to the minimum once we go past the maximum.
        if (targetYRotation > maxYAngle)
        {
            targetYRotation = minYAngle;
        }

        targetYRotation = Mathf.Clamp(targetYRotation, minYAngle, maxYAngle);

        Vector3 localEulerAngles = rotatingPart.localEulerAngles;
        localEulerAngles.y = targetYRotation;
        rotatingPart.localEulerAngles = localEulerAngles;

        Debug.Log($"Emitter rotated to: {targetYRotation}");
    }

    private float NormalizeAngle(float angle)
    {
        if (angle > 180f)
        {
            angle -= 360f;
        }

        return angle;
    }
}