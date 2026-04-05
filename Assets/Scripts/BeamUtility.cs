using System.Collections.Generic;
using UnityEngine;

public enum BeamHitType
{
    None,
    Receptor,
    Splitter,
    ReflectiveSurface,
    Blocked
}

public struct BeamCastResult
{
    public BeamHitType hitType;
    public Vector3 endPoint;
    public Vector3 nextOrigin;
    public Vector3 nextDirection;
    public BeamSplitter splitter;
}

public static class BeamUtility
{
    public static void ConfigureLineRenderer(LineRenderer line, Color beamColor)
    {
        if (line == null)
        {
            return;
        }

        line.startColor = beamColor;
        line.endColor = beamColor;
        line.useWorldSpace = true;
        line.textureMode = LineTextureMode.Stretch;
    }
    
    public static void TraceBeam(
        Vector3 origin,
        Vector3 direction,
        float maxDistance,
        int maxReflections,
        LayerMask hitLayers,
        LayerMask reflectiveLayer,
        List<Vector3> points,
        int reflectionCount = 0)
    {
        if (reflectionCount >= maxReflections)
        {
            points.Add(origin + direction * maxDistance);
            return;
        }

        if (Physics.Raycast(origin, direction, out RaycastHit hit, maxDistance, hitLayers))
        {
            points.Add(hit.point);

            LightReceptor receptor = hit.collider.GetComponent<LightReceptor>();
            if (receptor != null)
            {
                receptor.OnBeamHit();
                return;
            }

            BeamSplitter splitter = hit.collider.GetComponent<BeamSplitter>();
            if (splitter != null)
            {
                splitter.NotifyHit();
                return;
            }

            if (((1 << hit.collider.gameObject.layer) & reflectiveLayer.value) != 0)
            {
                Vector3 reflectedDirection = Vector3.Reflect(direction, hit.normal).normalized;
                Vector3 nextOrigin = hit.point + reflectedDirection * 0.01f;

                TraceBeam(
                    nextOrigin,
                    reflectedDirection,
                    maxDistance,
                    maxReflections,
                    hitLayers,
                    reflectiveLayer,
                    points,
                    reflectionCount + 1);

                return;
            }

            return;
        }

        points.Add(origin + direction * maxDistance);
    }
    
    public static void UpdateBeamTiling(Material beamMaterialInstance, List<Vector3> beamPoints, float textureTiling)
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