using UnityEngine;
using System.Collections.Generic;

public class BeamSplitter : MonoBehaviour
{
    [Header("Beam Settings")]
    [SerializeField] private BeamSettings beamSettings;
    [SerializeField, Range(0,1)] private float strengthReduction;

    [Header("Output References")] [SerializeField]
    private LineRenderer lineRendererA;
    [SerializeField]
    private LineRenderer lineRendererB;

    // These transforms define the exit points and angles of the split beams
    public Transform splitDirectionA;
    public Transform splitDirectionB;

    private bool isHitThisFrame = false;

    void Update()
    {
        if (isHitThisFrame)
        {
            lineRendererA.enabled = true;
            lineRendererB.enabled = true;
            CastSplitBeam(splitDirectionA, lineRendererA);
            CastSplitBeam(splitDirectionB, lineRendererB);
        }
        else
        {
            lineRendererA.enabled = false;
            lineRendererB.enabled = false;
        }

        // Reset flag for the next frame
        isHitThisFrame = false;
    }

    public void NotifyHit()
    {
        isHitThisFrame = true;
    }

    private void CastSplitBeam(Transform exitPoint, LineRenderer line)
    {
        List<Vector3> points = new List<Vector3>();
        points.Add(exitPoint.position);

        float splitDistance = beamSettings.maxBeamDistance * (1f - strengthReduction);

        BeamUtility.CastBeamSegment(
            exitPoint.position,
            exitPoint.forward,
            splitDistance,
            beamSettings.hitLayers,
            beamSettings.reflectiveLayer,
            points);

        line.positionCount = points.Count;
        line.SetPositions(points.ToArray());
    }
}