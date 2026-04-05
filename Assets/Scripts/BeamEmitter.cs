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

    /// <summary>
    /// Cache components and initialize the LineRenderer.
    /// </summary>
    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();

        lineRenderer.startColor = beamSettings.beamColor;
        lineRenderer.endColor = beamSettings.beamColor;
        lineRenderer.useWorldSpace = true;

        // Keep Stretch since your Shader Graph is handling tiling via the _Tiling property.
        lineRenderer.textureMode = LineTextureMode.Stretch;

        // Grab an instance so changes to _Tiling only affect this emitter.
        beamMaterialInstance = lineRenderer.material;
    }

    /// <summary>
    /// Regenerate the beam every frame.
    /// </summary>
    private void Update()
    {
        GenerateBeam();
    }

    /// <summary>
    /// Builds the beam path, including reflections, and updates the LineRenderer.
    /// </summary>
    private void GenerateBeam()
    {
        beamPoints.Clear();
        beamPoints.Add(transform.position);

        ReflectBeam(transform.position, transform.forward, 0);

        lineRenderer.positionCount = beamPoints.Count;
        lineRenderer.SetPositions(beamPoints.ToArray());

        UpdateBeamTiling();
    }

    /// <summary>
    /// Casts the beam forward and recursively reflects it from reflective surfaces.
    /// </summary>
    /// <param name="origin">Starting point of this beam segment.</param>
    /// <param name="direction">Direction of this beam segment.</param>
    /// <param name="reflectionCount">Current reflection depth.</param>
    private void ReflectBeam(Vector3 origin, Vector3 direction, int reflectionCount)
    {
        if (reflectionCount >= beamSettings.maxReflections)
        {
            beamPoints.Add(origin + direction * beamSettings.maxBeamDistance);
            return;
        }

        BeamCastResult result = BeamUtility.CastBeamSegment(
            origin,
            direction,
            beamSettings.maxBeamDistance,
            beamSettings.hitLayers,
            beamSettings.reflectiveLayer,
            beamPoints);

        if (result.hitType == BeamHitType.ReflectiveSurface)
        {
            ReflectBeam(result.nextOrigin, result.nextDirection, reflectionCount + 1);
        }
    } 

    /// <summary>
    /// Calculates the total beam length across all segments and sends it to Shader Graph.
    /// </summary>
    private void UpdateBeamTiling()
    {
        if (beamMaterialInstance == null || beamPoints.Count < 2)
        {
            return;
        }

        float totalBeamLength = 0f;

        for (int i = 0; i < beamPoints.Count - 1; i++)
        {
            totalBeamLength += Vector3.Distance(beamPoints[i], beamPoints[i + 1]);
        }

        beamMaterialInstance.SetFloat("_Tiling", totalBeamLength * beamSettings.textureTiling);
    }
}