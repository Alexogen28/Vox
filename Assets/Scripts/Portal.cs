using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField] private LevelSO levelToTeleportTo;
    [SerializeField] private WorldManager worldManager;

    public void Initialise(Vector3 spawnPosition, LevelSO levelToTeleportTo, WorldManager worldManager)
    {
        transform.position = spawnPosition;
        this.levelToTeleportTo = levelToTeleportTo;
        this.worldManager = worldManager;
    }

    private void TeleportToNextLevel()
    {
        worldManager.GenerateWorld(levelToTeleportTo);
    }

    
}
