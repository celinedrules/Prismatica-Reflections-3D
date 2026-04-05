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
    public static BeamCastResult CastBeamSegment(
        Vector3 origin,
        Vector3 direction,
        float maxDistance,
        LayerMask hitLayers,
        LayerMask reflectiveLayer,
        List<Vector3> points,
        float surfaceOffset = 0.01f)
    {
        BeamCastResult result = new BeamCastResult();

        if (Physics.Raycast(origin, direction, out RaycastHit hit, maxDistance, hitLayers))
        {
            points.Add(hit.point);
            result.endPoint = hit.point;

            LightReceptor receptor = hit.collider.GetComponent<LightReceptor>();
            if (receptor != null)
            {
                receptor.OnBeamHit();
                result.hitType = BeamHitType.Receptor;
                return result;
            }

            BeamSplitter splitter = hit.collider.GetComponent<BeamSplitter>();
            if (splitter != null)
            {
                splitter.NotifyHit();
                result.hitType = BeamHitType.Splitter;
                result.splitter = splitter;
                return result;
            }

            if (((1 << hit.collider.gameObject.layer) & reflectiveLayer.value) != 0)
            {
                Vector3 reflectedDirection = Vector3.Reflect(direction, hit.normal).normalized;

                result.hitType = BeamHitType.ReflectiveSurface;
                result.nextDirection = reflectedDirection;
                result.nextOrigin = hit.point + (reflectedDirection * surfaceOffset);
                return result;
            }

            result.hitType = BeamHitType.Blocked;
            return result;
        }

        Vector3 endPoint = origin + direction * maxDistance;
        points.Add(endPoint);

        result.hitType = BeamHitType.None;
        result.endPoint = endPoint;
        return result;
    }
}