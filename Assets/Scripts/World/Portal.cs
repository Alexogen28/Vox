using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField] private LevelSO levelToTeleportTo;
    [SerializeField] private GameManager gameManager;

    public void Initialise(GameManager gameManager, LevelSO levelToTeleportTo)
    {
        this.levelToTeleportTo = levelToTeleportTo;
        this.gameManager = gameManager;
    }

    private void TeleportToNextLevel()
    {
        gameManager.worldManager.GenerateWorld(levelToTeleportTo);
    }

    public void OnColliderEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            TeleportToNextLevel();
        }
    }
}
