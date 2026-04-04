using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class BeamEmitter : MonoBehaviour
{
    [Header("Beam Settings")]
    [SerializeField] private float maxBeamDistance = 100f;
    [SerializeField] private int maxReflections = 10;
    [SerializeField] private Color beamColor = Color.white;
    [SerializeField] private LayerMask hitLayers;
    [SerializeField] private LayerMask reflectiveLayer;

    [Header("Beam Visuals")]
    [SerializeField] private float textureTiling = 6f;

    private LineRenderer lineRenderer;
    private Material beamMaterialInstance;
    private readonly List<Vector3> beamPoints = new();

    /// <summary>
    /// Cache components and initialize the LineRenderer.
    /// </summary>
    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();

        lineRenderer.startColor = beamColor;
        lineRenderer.endColor = beamColor;
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
        // Stop infinite bouncing.
        if (reflectionCount >= maxReflections)
        {
            beamPoints.Add(origin + direction * maxBeamDistance);
            return;
        }

        if (Physics.Raycast(origin, direction, out RaycastHit hit, maxBeamDistance, hitLayers))
        {
            beamPoints.Add(hit.point);

            BeamSplitter splitter = hit.collider.GetComponent<BeamSplitter>();
            
            if (splitter != null)
            {
                splitter.NotifyHit();
                return;
            }

            
            // Only reflect if the object is on the reflective layer mask.
            if (((1 << hit.collider.gameObject.layer) & reflectiveLayer.value) != 0)
            {
                Vector3 reflectedDirection = Vector3.Reflect(direction, hit.normal).normalized;

                // Small offset to prevent immediately hitting the same surface again.
                Vector3 nextOrigin = hit.point + (reflectedDirection * 0.01f);

                ReflectBeam(nextOrigin, reflectedDirection, reflectionCount + 1);
            }
        }
        else
        {
            // Nothing hit, so draw the segment out to max distance.
            beamPoints.Add(origin + direction * maxBeamDistance);
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

        beamMaterialInstance.SetFloat("_Tiling", totalBeamLength * textureTiling);
    }
}

// using UnityEngine;
//
// [RequireComponent(typeof(LineRenderer))]
// public class BeamEmitter : MonoBehaviour
// {
//     [Header("Beam Settings")]
//     public float maxBeamDistance = 100f;
//     public Color beamColor = Color.white;
//     public LayerMask hitLayers;
//
//     [Header("Beam Visuals")]
//     public float textureTiling = 6f;
//
//     private LineRenderer lineRenderer;
//     private Material beamMaterialInstance;
//
//     private void Awake()
//     {
//         lineRenderer = GetComponent<LineRenderer>();
//
//         lineRenderer.startColor = beamColor;
//         lineRenderer.endColor = beamColor;
//         lineRenderer.positionCount = 2;
//         lineRenderer.useWorldSpace = true;
//         lineRenderer.textureMode = LineTextureMode.Stretch;
//
//         beamMaterialInstance = lineRenderer.material;
//     }
//
//     private void Update()
//     {
//         CastBeam();
//     }
//
//     private void CastBeam()
//     {
//         Vector3 origin = transform.position;
//         Vector3 direction = transform.forward;
//         Vector3 endPoint = origin + (direction * maxBeamDistance);
//
//         if (Physics.Raycast(origin, direction, out RaycastHit hit, maxBeamDistance, hitLayers))
//         {
//             endPoint = hit.point;
//         }
//
//         lineRenderer.SetPosition(0, origin);
//         lineRenderer.SetPosition(1, endPoint);
//
//         float beamLength = Vector3.Distance(origin, endPoint);
//
//         if (beamMaterialInstance != null)
//         {
//             beamMaterialInstance.SetFloat("_Tiling", beamLength * textureTiling);
//         }
//     }
// }