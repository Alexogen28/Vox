
using UnityEngine;

/*
 * Responsible for spawning in the following:
 * 1. Level portals
 * 2. Loot chests
 * 3. Special structures
 */


public class ObjectiveManager : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GameObject mainLevelPortal;

    public void SpawnPortals()
    {
        Vector3 playerLocation = gameManager.playerController.PlayerPosition;

        Vector3 portalSpawnLocation = new Vector3();
        Vector2Int coordinatesInsideChunk = new Vector2Int();

        //try and get a viable chunk to spawn the portal in;
        //it should not be in the same chunk as the player or in a nearby chunk

        Vector3Int portalChunkCoordinates = new Vector3Int();
        portalChunkCoordinates.y = 0;

        System.Random randomChunk = Seed.CreateRandom(gameManager.worldManager.GetWorldSeed(), 
            AvailableSeedKeys.SpawnChunk);

        portalChunkCoordinates.x = randomChunk.Next(0, gameManager.worldManager.worldSizeX-1);
        portalChunkCoordinates.z = randomChunk.Next(0, gameManager.worldManager.worldSizeZ-1);

        System.Random randomPoint = Seed.CreateRandom(gameManager.worldManager.GetWorldSeed(), AvailableSeedKeys.SpawnPoint);

        coordinatesInsideChunk.x = randomPoint.Next();
        coordinatesInsideChunk.y = randomPoint.Next();

        gameManager.worldManager.GetChunk(portalChunkCoordinates).TryGetBottomOYVoxel(coordinatesInsideChunk.x,
            coordinatesInsideChunk.y, out portalSpawnLocation);

        GameObject portalGameObject = Instantiate(mainLevelPortal, portalSpawnLocation, Quaternion.identity);
        Portal portal = portalGameObject.GetComponent<Portal>();

        if (portal == null)
            return;

        portal.Initialise(gameManager, gameManager.levelManager.DetermineNextLevel());
    }
}
