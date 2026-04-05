using UnityEngine;
using System.Collections.Generic;

public class BeamSplitter : MonoBehaviour
{
    [Header("Beam Settings")]
    [SerializeField] private BeamSettings beamSettings;
    [SerializeField, Range(0, 1)] private float strengthReduction;

    [Header("Output References")] [SerializeField]
    private LineRenderer lineRendererA;
    [SerializeField]
    private LineRenderer lineRendererB;

    private Material beamMaterialInstanceA;
    private Material beamMaterialInstanceB;

    public Transform splitDirectionA;
    public Transform splitDirectionB;
    
    private bool isHitThisFrame;

    private void Awake()
    {
        BeamUtility.ConfigureLineRenderer(lineRendererA, beamSettings.beamColor);
        BeamUtility.ConfigureLineRenderer(lineRendererB, beamSettings.beamColor);

        beamMaterialInstanceA = lineRendererA.material;
        beamMaterialInstanceB = lineRendererB.material;
    }

    private void Update()
    {
        if (isHitThisFrame)
        {
            lineRendererA.enabled = true;
            lineRendererB.enabled = true;

            List<Vector3> pointsA = CastSplitBeam(splitDirectionA, lineRendererA);
            List<Vector3> pointsB = CastSplitBeam(splitDirectionB, lineRendererB);

            BeamUtility.UpdateBeamTiling(beamMaterialInstanceA, pointsA, beamSettings.textureTiling);
            BeamUtility.UpdateBeamTiling(beamMaterialInstanceB, pointsB, beamSettings.textureTiling);
        }
        else
        {
            lineRendererA.enabled = false;
            lineRendererB.enabled = false;
        }

        isHitThisFrame = false;
    }

    public void NotifyHit()
    {
        isHitThisFrame = true;
    }

    private List<Vector3> CastSplitBeam(Transform exitPoint, LineRenderer line)
    {
        List<Vector3> points = new() { exitPoint.position };

        float splitDistance = beamSettings.maxBeamDistance * (1f - strengthReduction);

        BeamUtility.TraceBeam(
            exitPoint.position,
            exitPoint.forward,
            splitDistance,
            beamSettings.maxReflections,
            beamSettings.hitLayers,
            beamSettings.reflectiveLayer,
            points);

        line.positionCount = points.Count;
        line.SetPositions(points.ToArray());

        return points;
    }
}