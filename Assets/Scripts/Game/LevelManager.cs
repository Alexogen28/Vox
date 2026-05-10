using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private LevelSO currentLevel;
    [SerializeField] private List<LevelSO> levelsList;

    public LevelSO CurrentLevel => currentLevel;
    public void SetCurrentLevel(LevelSO level) => currentLevel = level;

    //TODO: actually make the method work
    public LevelSO DetermineNextLevel()
    {
        return levelsList[levelsList.Count - 1];
    }
}

