using System;
using UnityEngine;

public class LightReceptor : MonoBehaviour
{
    public event Action<LightReceptor, bool> ActivationChanged;
    
    [Header("State")]
    public bool isActivated = false;

    [Header("Visuals")]
    [SerializeField] private MeshRenderer crystalRenderer;
    [SerializeField] private Material inactiveMaterial;
    [SerializeField] private Material activationMaterial;
    [SerializeField] private GameObject activationEffect;

    private bool _hitThisFrame = false;

    public void OnBeamHit()
    {
        _hitThisFrame = true;
    }

    private void LateUpdate()
    {
        EvaluateActivationState();
        _hitThisFrame = false;
    }

    private void EvaluateActivationState()
    {
        if (_hitThisFrame && !isActivated)
        {
            ActivateReceptor();
        }
        else if (!_hitThisFrame && isActivated)
        {
            DeactivateReceptor();
        }
    }

    private void ActivateReceptor()
    {
        isActivated = true;

        if (crystalRenderer != null && activationMaterial != null)
        {
            crystalRenderer.material = activationMaterial;
        }

        if (activationEffect != null)
        {
            activationEffect.SetActive(true);
        }

        Debug.Log($"{gameObject.name} Activated!");
        ActivationChanged?.Invoke(this, true);
    }

    private void DeactivateReceptor()
    {
        isActivated = false;

        if (crystalRenderer != null && inactiveMaterial != null)
        {
            crystalRenderer.material = inactiveMaterial;
        }

        if (activationEffect != null)
        {
            activationEffect.SetActive(false);
        }
        
        Debug.Log($"{gameObject.name} Deactivated!");
        ActivationChanged?.Invoke(this, false);
    }
}