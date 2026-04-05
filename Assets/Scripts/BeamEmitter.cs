using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class BeamEmitter : MonoBehaviour
{
    [Header("Beam Settings")]
    [SerializeField] private BeamSettings beamSettings;

    private LineRenderer lineRenderer;
    private Material beamMaterialInstance;
    private readonly List<Vector3> beamPoints = new();

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        BeamUtility.ConfigureLineRenderer(lineRenderer, beamSettings.beamColor);
        beamMaterialInstance = lineRenderer.material;
    }

    private void Update()
    {
        GenerateBeam();
    }

    private void GenerateBeam()
    {
        CastBeam();
        BeamUtility.UpdateBeamTiling(beamMaterialInstance, beamPoints, beamSettings.textureTiling);
    }

    private void CastBeam()
    {
        beamPoints.Clear();
        beamPoints.Add(transform.position);

        BeamUtility.TraceBeam(
            transform.position,
            transform.forward,
            beamSettings.maxBeamDistance,
            beamSettings.maxReflections,
            beamSettings.hitLayers,
            beamSettings.reflectiveLayer,
            beamPoints);

        lineRenderer.positionCount = beamPoints.Count;
        lineRenderer.SetPositions(beamPoints.ToArray());
    }
}