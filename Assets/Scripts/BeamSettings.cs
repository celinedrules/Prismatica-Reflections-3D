using UnityEngine;

[CreateAssetMenu(fileName = "BeamSettings", menuName = "Prismatica/Beam Settings")]
public class BeamSettings : ScriptableObject
{
    [Header("Collision")]
    public LayerMask hitLayers;
    public LayerMask reflectiveLayer;

    [Header("Distances")]
    public float maxBeamDistance = 100f;
    public int maxReflections = 10;

    [Header("Visuals")]
    public Color beamColor = Color.white;
    public float textureTiling = 6f;
}