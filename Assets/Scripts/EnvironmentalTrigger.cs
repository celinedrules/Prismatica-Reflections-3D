using UnityEngine;

public class EnvironmentalTrigger : MonoBehaviour
{
    [Header("Connection")]
    [SerializeField] private LightReceptor sourceReceptor; // The crystal that triggers this
    [SerializeField] private GameObject targetMechanism;   // The bridge, door, or stairs

    [Header("Movement Settings")]
    [SerializeField] private Vector3 activePositionOffset;
    [SerializeField] private float transitionSpeed = 2.0f;

    private Vector3 _startPosition;
    private Vector3 _targetPosition;
    private float _currentTransition = 0f;

    void Start()
    {
        if (targetMechanism != null)
        {
            _startPosition = targetMechanism.transform.localPosition;
            _targetPosition = _startPosition + activePositionOffset;
        }
    }

    void Update()
    {
        if (sourceReceptor == null || targetMechanism == null) return;

        // Check the state from the Receptor logic established in Step 1
        bool shouldBeActive = sourceReceptor.isActivated;

        // Smoothly move the transition value between 0 (start) and 1 (active)
        if (shouldBeActive)
            _currentTransition = Mathf.MoveTowards(_currentTransition, 1f, transitionSpeed * Time.deltaTime);
        else
            _currentTransition = Mathf.MoveTowards(_currentTransition, 0f, transitionSpeed * Time.deltaTime);

        AnimateMechanism();
    }

    private void AnimateMechanism()
    {
        // Apply the lerp to the target mechanism's position
        targetMechanism.transform.localPosition = Vector3.Lerp(_startPosition, _targetPosition, _currentTransition);
        
        // Optional: Add a small rotation or scale pulse here to match the 'magical' aesthetic
    }
}