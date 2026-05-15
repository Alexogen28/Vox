
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


    /*
     * Can be used if you want the portals to spawn on the world instead
     * of at the bottom edge
     */
    public void SpawnPortals()
    {
        Vector3 playerLocation = gameManager.playerController.PlayerPosition;

        Vector3 portalSpawnLocation = new Vector3();
        Vector2Int coordinatesInsideChunk = new Vector2Int();

        Vector3Int portalChunkCoordinates = new Vector3Int();
        portalChunkCoordinates.y = 0;

        System.Random randomChunk = Seed.CreateRandom(gameManager.worldManager.GetWorldSeed(),
            AvailableSeedKeys.SpawnChunk);

        portalChunkCoordinates.x = randomChunk.Next(0, gameManager.worldManager.worldSizeX - 1);
        portalChunkCoordinates.z = randomChunk.Next(0, gameManager.worldManager.worldSizeZ - 1);

        System.Random randomPoint = Seed.CreateRandom(gameManager.worldManager.GetWorldSeed(), AvailableSeedKeys.SpawnPoint);

        coordinatesInsideChunk.x = randomPoint.Next(0, gameManager.worldManager.chunkSize);
        coordinatesInsideChunk.y = randomPoint.Next(0, gameManager.worldManager.chunkSize);

        gameManager.worldManager.GetChunk(portalChunkCoordinates).TryGetBottomOYVoxel(coordinatesInsideChunk.x,
            coordinatesInsideChunk.y, out portalSpawnLocation);

        GameObject portalGameObject = Instantiate(mainLevelPortal, portalSpawnLocation, Quaternion.identity);
        Portal portal = portalGameObject.GetComponentInChildren<Portal>();

        Debug.Log("Pulled portal component:" + portal);

        if (portal == null)
            return;

        portal.Initialise(gameManager, gameManager.levelManager.DetermineNextLevel());
    }

    /*
     * Method that spawns 1 large, long portal across the entire floor of the 
     * game world. Makes descending a whole lot cooler!
     * Could do the opposite with a vertical portal, but that one would 
     * need to be masked without an actual mesh, only a trigger
     */

    public void SpawnPortal()
    {
        Vector3 spawnLocation = new Vector3(
            gameManager.worldManager.worldSizeX * gameManager.worldManager.chunkSize * gameManager.worldManager.voxelSize * 0.5f,
            -1,
            gameManager.worldManager.worldSizeZ * gameManager.worldManager.chunkSize * gameManager.worldManager.voxelSize * 0.5f
            );

        float portalScaleX = gameManager.worldManager.worldSizeX * gameManager.worldManager.chunkSize * 
            gameManager.worldManager.voxelSize;
        float portalScaleZ = gameManager.worldManager.worldSizeZ * gameManager.worldManager.chunkSize *
            gameManager.worldManager.voxelSize;

        GameObject portalGameObject = Instantiate(mainLevelPortal, spawnLocation, Quaternion.identity);
        Portal portal = portalGameObject.GetComponentInChildren<Portal>();
        portal.Initialise(gameManager, gameManager.levelManager.DetermineNextLevel());
        portalGameObject.transform.localScale = new Vector3(portalScaleX, 1, portalScaleZ);
    }
}
