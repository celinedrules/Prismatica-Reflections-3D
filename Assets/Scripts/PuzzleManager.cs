using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

public class PuzzleManager : MonoBehaviour
{
    [Header("Level Configuration")]
    [SerializeField] private List<LightReceptor> targetReceptors = new List<LightReceptor>();

    [Header("Events")]
    public UnityEvent onLevelComplete;

    private bool levelIsFinished = false;

    private void OnEnable()
    {
        SubscribeToReceptors();
    }

    private void OnDisable()
    {
        UnsubscribeFromReceptors();
    }

    private void SubscribeToReceptors()
    {
        foreach (LightReceptor receptor in targetReceptors)
        {
            if (receptor == null)
            {
                continue;
            }

            receptor.ActivationChanged -= HandleReceptorActivationChanged;
            receptor.ActivationChanged += HandleReceptorActivationChanged;
        }
    }

    private void UnsubscribeFromReceptors()
    {
        foreach (LightReceptor receptor in targetReceptors)
        {
            if (receptor == null)
            {
                continue;
            }

            receptor.ActivationChanged -= HandleReceptorActivationChanged;
        }
    }

    private void HandleReceptorActivationChanged(LightReceptor receptor, bool isActive)
    {
        CheckWinCondition();
    }

    public void CheckWinCondition()
    {
        if (levelIsFinished)
        {
            return;
        }

        int totalReceptors = 0;
        int activatedCount = 0;

        foreach (LightReceptor receptor in targetReceptors)
        {
            if (receptor == null)
            {
                continue;
            }

            totalReceptors++;

            if (receptor.isActivated)
            {
                activatedCount++;
            }
        }

        if (totalReceptors > 0 && activatedCount >= totalReceptors)
        {
            CompleteLevel();
        }
    }

    private void CompleteLevel()
    {
        levelIsFinished = true;
        Debug.Log("Sanctuary Restored!");
        onLevelComplete?.Invoke();
    }
}