using UnityEngine;
using System.Collections.Generic;

public class BeamSplitter : MonoBehaviour
{
    [Header("Beam Settings")]
    public Color beamColor = Color.cyan;
    public float maxBeamDistance = 50f;

    [Header("Output References")][SerializeField] 
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

        Ray ray = new Ray(exitPoint.position, exitPoint.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxBeamDistance))
        {
            points.Add(hit.point);
            
            // Integration: If we hit another splitter, notify it
            BeamSplitter otherSplitter = hit.collider.GetComponent<BeamSplitter>();
            if (otherSplitter != null)
            {
                otherSplitter.NotifyHit();
            }
            
            // Logic for hitting ReflectiveLayer objects can be handled here 
            // by calling the ReflectBeam() logic established in previous steps.
        }
        else
        {
            points.Add(exitPoint.position + (exitPoint.forward * maxBeamDistance));
        }

        line.positionCount = points.Count;
        line.SetPositions(points.ToArray());
    }
}