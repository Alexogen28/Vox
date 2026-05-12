using System.Collections.Generic;
using UnityEngine;

public enum LevelName
{
    Surface,
    UpperCaves,
    LowerDepths,
    UndergroundBog
}

public enum PlacementPosition
{
    Top,
    Bottom,
    Left,
    Right,
    Front,
    Rear
}

[CreateAssetMenu(fileName = "LevelSO", menuName = "Vox Obscura/World/LevelSO")]
public class LevelSO : ScriptableObject
{
    [SerializeField] public AvailableSeedKeys levelKey;
    [SerializeField] public LevelName levelName;
    [SerializeField] public List<PlacementPosition> allowedPlacementPositions;
    [SerializeField] public Material availableMaterial;
    [SerializeField] public bool shouldAvoidEdges;

    [Header("Fog and Light settings")]
    [SerializeField] public bool hasFog;
    [SerializeField] public float fogDensity;
    [SerializeField] public Color fogColour;
    [SerializeField] public FogMode fogMode;
    [SerializeField] public Color ambientLightColour;
}
