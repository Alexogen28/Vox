using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField] private LevelSO levelToTeleportTo;
    [SerializeField] private GameManager gameManager;

    public void Initialise(GameManager gameManager, LevelSO levelToTeleportTo)
    {
        this.levelToTeleportTo = levelToTeleportTo;
        Debug.Log("Sucessfuly added level to teleport to to portal");
        this.gameManager = gameManager;
        Debug.Log("Sucessfully added Game Manager to portal");
    }

    private void TeleportToNextLevel()
    {
        gameManager.worldManager.GenerateWorld(levelToTeleportTo);
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            TeleportToNextLevel();
        }
    }
}
