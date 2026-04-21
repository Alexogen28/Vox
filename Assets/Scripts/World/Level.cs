using System.Collections.Generic;
using UnityEngine;



public class Level : MonoBehaviour
{
    [SerializeField] private LevelName levelName;
    [SerializeField] private List<PlacementPosition> allowedPlacementPositions;

    public void Initialise(LevelName levelName, List<PlacementPosition> allowedPlacementPosition)
    {
        this.levelName = levelName;
        foreach(PlacementPosition pos in allowedPlacementPosition)
            this.allowedPlacementPositions.Add(pos);
    }

    public LevelName GetLevelName() => levelName;

    public void AddPlacementPosition(PlacementPosition pos)
    {
        allowedPlacementPositions.Add(pos);
    }
}
