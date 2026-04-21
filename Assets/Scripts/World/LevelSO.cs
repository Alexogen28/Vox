using System.Collections.Generic;
using UnityEngine;

public enum LevelName
{
    Surface,
    UpperCaves,
    LowerDepths,
    UndergroundBog
}

public enum Positions
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
    [SerializeField] public LevelName levelName;
    [SerializeField] public List<Positions> allowedPlacementPositions;
}
