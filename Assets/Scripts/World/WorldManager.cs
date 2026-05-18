using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

/*
    Master class for anything world data related
    Contains informations about     all chunks and their sizes
                                    voxel size
                                    world size
                                    level types
                                    world seed

    Is resposible for   creating the world 
                        recreating it with each new level
                        providing information about the world
*/


public class WorldManager : MonoBehaviour
{
    [Header("Managers")]
    [SerializeField] private GameManager gameManager;

    [Header("World Stats")]
    [SerializeField] public int worldSizeX = 4;
    [SerializeField] public int worldSizeY = 1;
    [SerializeField] public int worldSizeZ = 4;

    [Header("Chunk Stats")]
    [SerializeField] public int chunkSize = 64;
    [SerializeField] public float voxelSize = 0.75f;

    [Header("Temporary Materials Header")]
    [SerializeField] public Material baseMat;

    [Header("World Gen Parameters")]
    [SerializeField] private LevelSO startingLevel;
    [SerializeField] private int worldSeed = 12345;
    [SerializeField] private bool randomiseIntOnPlay = true;
    public int seedMinimumValue = 1;
    public int seedMaximumValue = int.MaxValue;
    private Vector3Int worldSize;
    public Dictionary<Vector3Int, VoxelChunk> worldChunks = new();

    [SerializeField] private NavMeshSurface navMeshSurface;

    public int GetWorldSeed() => worldSeed;

    public VoxelChunk GetChunk(Vector3Int coord) =>
        worldChunks.TryGetValue(coord, out var chunk) ? chunk : null;

    public Dictionary<Vector3Int, VoxelChunk> GetAllWorldChunks =>
        worldChunks;

    public NavMeshSurface GetNavMeshSurface() => navMeshSurface;

    private void SetRandomSeed()
    {
        worldSeed = Random.Range(seedMinimumValue, seedMaximumValue);
    }

    /*
        Old method of generating world via Type
        Deprecated, but kept because its neat
    */
    // void GenerateWorld<T>() where T : VoxelChunk
    // {
    //     worldChunks.Clear();

    //     Debug.Log("Generating world...");

    //     for (int x = 0; x < worldSizeX; x++)
    //     {
    //         for (int y = 0; y < worldSizeY; y++)
    //         {
    //             for (int z = 0; z < worldSizeZ; z++)
    //             {
    //                 var coord = new Vector3Int(x, y, z);

    //                 Vector3 worldCoord = new Vector3(x, y, z) * (chunkSize * voxelSize);

    //                 var chunkGameObject = new GameObject($"Chunk_{x}_{y}_{z}");

    //                 chunkGameObject.transform.SetParent(transform, false);
    //                 chunkGameObject.transform.localPosition = worldCoord;
    //                 chunkGameObject.layer = LayerMask.NameToLayer("Ground");

    //                 var chunk = chunkGameObject.AddComponent<T>();
    //                 chunk.Initialize(chunkSize, voxelSize, baseMat, worldSeed, coord, worldSize, this, currentLevel);

    //                 worldChunks.Add(coord, chunk);
    //                 Debug.Log($"Creating chunk at {coord} / worldPos {worldCoord}");
    //             }
    //         }
    //     }
    // }

    /*
     * Master class for generating the world!
     * 
        Generate the current level based on the given Level type
        One "world" is defined by X, Y, Z extents and is equivalent to one level
        which will guaranteed have a portal generated
    */

    public void GenerateWorld(LevelSO level)
    {
        gameManager.levelManager.SetCurrentLevel(level);
        gameManager.decorationController.ClearWellSpawnPositions();

        if (worldChunks.Count != 0)
            ClearWorld();

        Debug.Log("Generating world...");

        /*
         * First pass of world generation
         * 
         * Sets down all chunks in their base form
         * and stretches out a portal across the floor of the world
         */
        FirstPassGeneration(level);

        /*
         * Second pass of world generation
         * 
         * Determines the possible descent positions 
         * and stores them so Decoration Controller can then spawn wells
         */
        SecondGenerationPass(level);

        /*
         * Third pass of world generation
         * 
         * Adds decorations to the world
         */
        gameManager.decorationController.DecorateWorld(level);



        if (level.levelName == LevelName.Surface)
            DeterminePlayerSpawnLocationOnSurface();
        else
            DeterminePlayerSpawnLocation();


        RenderSettings.fog = level.hasFog;
        RenderSettings.fogDensity = level.fogDensity;
        RenderSettings.fogColor = level.fogColour;
        RenderSettings.ambientLight = level.ambientLightColour;
        RenderSettings.fogMode = level.fogMode;

        /*
         * Fourth pass of world generation
         * 
         * Creates the necessary nav mesh
         * for enemies to be able to move
         */
        FourthGenerationPass(level);
    }

    private void FirstPassGeneration(LevelSO level)
    {
        for (int x = 0; x < worldSizeX; x++)
        {
            if (level.levelName == LevelName.Surface)
                for (int y = 0; y < 1; y++)
                    for (int z = 0; z < worldSizeZ; z++)
                    {
                        var coord = new Vector3Int(x, y, z);

                        GenerateChunk(level, coord);
                    }
            else
                for (int y = 0; y < worldSizeY; y++)
                    for (int z = 0; z < worldSizeZ; z++)
                    {
                        var coord = new Vector3Int(x, y, z);

                        GenerateChunk(level, coord);
                    }
        }


        //spawns 1 large portal across the floor of the level
        gameManager.objectiveManager.SpawnPortal();
    }

    private void SecondGenerationPass(LevelSO level)
    {
        if (level.levelKey == AvailableSeedKeys.Surface)
        {
            Vector3Int chunkToDescendFrom = GetRandomDescentChunk(level.shouldAvoidEdges, 0, level);
            worldChunks[chunkToDescendFrom].CarveDescent();
            return;
        }

        for (int y = worldSizeY - 1; y >= 0; y--)
        {
            Vector3Int chunkToDescendFrom = GetRandomDescentChunk(level.shouldAvoidEdges, y, level);
            worldChunks[chunkToDescendFrom].CarveDescent();
        }
    }

    private void FourthGenerationPass(LevelSO level)
    {
        navMeshSurface.BuildNavMesh();
        gameManager.enemySpawnManager.SpawnEnemies(level);
    }

    public void GenerateWorldOnStartup()
    {
        //get a random seed if allowed
        //this should pass when the player does not input
        //a seed at game start from the main menu
        if (randomiseIntOnPlay) SetRandomSeed();
        Debug.Log("Set Random Seed");

        worldSize = new Vector3Int(worldSizeX, worldSizeY, worldSizeZ);
        Debug.Log("Set World Size Correctly");

        //surface should only place stuff on the top of the world, its not a cave
        List<PlacementPosition> availablePlacementPositions = new List<PlacementPosition>
        {
            PlacementPosition.Bottom
        };

        //initialise and generate the Surface world
        GenerateWorld(startingLevel);
    }


    /*
     * Method to determine spawn location for the player for downward-moving levels
     * 
     * The player should always spawn on the top-most layer of the level
     * and it can use TryGetBottomOYVoxel of the chosen chunk to grab the actual spawn location of the player
     * 
     * TODO safeguard against spawning in an object etc.
     */
    private void DeterminePlayerSpawnLocation()
    {
        Vector3Int spawnChunk = GetRandomChunkCoords(false);
        spawnChunk.y = worldSizeY - 2;

        System.Random spawnRandom = Seed.CreateRandom(worldSeed, AvailableSeedKeys.SpawnPoint, spawnChunk);

        //pick only the middle section of the chunk
        int xInChunk = spawnRandom.Next(chunkSize / 4, (chunkSize / 2) + (chunkSize / 4));
        int zInChunk = spawnRandom.Next(chunkSize / 4, (chunkSize / 2) + (chunkSize / 4));
        Vector3 spawnLocation = new Vector3();
        worldChunks[spawnChunk].TryGetBottomOYVoxel(xInChunk, zInChunk, out spawnLocation);

        spawnLocation.y += voxelSize * 2;
        Debug.Log("Determined spawn location y:" + spawnLocation.y);

        gameManager.playerController.TeleportPlayer(spawnLocation);
    }

    /*
     * Method specific for surface spawning, this only accounts for a single y level
     */
    public void DeterminePlayerSpawnLocationOnSurface()
    {
        Vector3Int spawnChunk = GetRandomChunkCoords(true);
        spawnChunk.y = 0;

        int xInChunk = (int)Random.Range(spawnChunk.x * chunkSize * voxelSize,
            spawnChunk.x * chunkSize * voxelSize + chunkSize * voxelSize);
        int zInChunk = (int)Random.Range(spawnChunk.z * chunkSize * voxelSize,
            spawnChunk.z * chunkSize * voxelSize + chunkSize * voxelSize);

        float yInChunk = chunkSize * voxelSize;
        Vector3 raycastPosition = new Vector3(xInChunk, yInChunk, zInChunk);

        LayerMask groundMask = LayerMask.GetMask("Ground");

        if (Physics.Raycast(raycastPosition, Vector3.down, out RaycastHit hit, Mathf.Infinity, groundMask))
        {
            yInChunk = hit.point.y + 2.5f;
        }

        gameManager.playerController.TeleportPlayer(new Vector3(xInChunk, yInChunk, zInChunk));
    }

    private void GenerateChunk(LevelSO level, Vector3Int coord)
    {
        System.Type levelType = GetChunkTypeForLevel(level);
        Vector3 worldCoord = new Vector3(coord.x, coord.y, coord.z) * (chunkSize * voxelSize);

        var chunkGameObject = new GameObject($"Chunk_{coord.x}_{coord.y}_{coord.z}");

        chunkGameObject.transform.SetParent(transform, false);
        chunkGameObject.transform.localPosition = worldCoord;
        chunkGameObject.layer = LayerMask.NameToLayer("Ground");

        VoxelChunk chunk = (VoxelChunk)chunkGameObject.AddComponent(levelType);
        chunk.Initialize(chunkSize, voxelSize, level.availableMaterial, worldSeed, coord, worldSize, worldCoord, gameManager, level);

        worldChunks.Add(coord, chunk);
        //Debug.Log($"Creating chunk at {coord} / worldPos {worldCoord}");
    }

    public System.Type GetChunkTypeForLevel(LevelSO level)
    {
        switch (level.levelName)
        {
            case LevelName.Surface: return typeof(SurfaceChunk);
            case LevelName.UpperCaves: return typeof(UpperCavesChunk);
            default: return typeof(SurfaceChunk);
        }
    }

    private void ClearWorld()
    {
        foreach (var chunk in worldChunks.Values)
        {
            if (chunk != null)
                Destroy(chunk.gameObject);
        }

        worldChunks.Clear();
    }

    public Vector3Int GetRandomChunkCoords(bool avoidEdges)
    {
        int minX = avoidEdges && worldSize.x > 2 ? 1 : 0;
        int maxX = avoidEdges && worldSize.x > 2 ? worldSize.x - 1 : worldSize.x;

        int minY = avoidEdges && worldSize.y > 2 ? 1 : 0;
        int maxY = avoidEdges && worldSize.y > 2 ? worldSize.y - 1 : worldSize.y;

        int minZ = avoidEdges && worldSize.z > 2 ? 1 : 0;
        int maxZ = avoidEdges && worldSize.z > 2 ? worldSize.z - 1 : worldSize.z;

        System.Random random = Seed.CreateRandom(worldSeed, AvailableSeedKeys.SpawnPoint);

        return new Vector3Int(
            random.Next(minX, maxX),
            random.Next(minY, maxY),
            random.Next(minZ, maxZ)
        );
    }

    public VoxelChunk GetRandomChunk(bool avoidEdges, AvailableSeedKeys key, Vector3Int seedCoordinates)
    {
        int minX = avoidEdges && worldSize.x > 2 ? 1 : 0;
        int maxX = avoidEdges && worldSize.x > 2 ? worldSize.x - 1 : worldSize.x;

        int minY = avoidEdges && worldSize.y > 2 ? 1 : 0;
        int maxY = avoidEdges && worldSize.y > 2 ? worldSize.y - 1 : worldSize.y;

        int minZ = avoidEdges && worldSize.z > 2 ? 1 : 0;
        int maxZ = avoidEdges && worldSize.z > 2 ? worldSize.z - 1 : worldSize.z;

        Vector3Int offsetsX = new Vector3Int(1, 0, 0);
        Vector3Int offsetsY = new Vector3Int(0, 1, 0);
        Vector3Int offsetsZ = new Vector3Int(0, 0, 1);

        System.Random randomX = Seed.CreateRandom(worldSeed, key, seedCoordinates + offsetsX);
        System.Random randomY = Seed.CreateRandom(worldSeed, key, seedCoordinates + offsetsY);
        System.Random randomZ = Seed.CreateRandom(worldSeed, key, seedCoordinates + offsetsZ);

        int chunkX = randomX.Next(minX, maxX);
        int chunkY = randomX.Next(minY, maxY);
        int chunkZ = randomX.Next(minY, maxY);

        return worldChunks[new Vector3Int(chunkX, chunkY, chunkZ)];
    }

    private Vector3Int GetRandomDescentChunk(bool avoidEdges, int yLayer, LevelSO currentLevel)
    {
        int minX = avoidEdges && worldSize.x > 2 ? 1 : 0;
        int maxX = avoidEdges && worldSize.x > 2 ? worldSize.x - 1 : worldSize.x;

        int minZ = avoidEdges && worldSize.z > 2 ? 1 : 0;
        int maxZ = avoidEdges && worldSize.z > 2 ? worldSize.z - 1 : worldSize.z;

        System.Random random = Seed.CreateRandom(worldSeed, currentLevel.levelKey, new Vector3Int(0,yLayer,0));

        return new Vector3Int(
            random.Next(minX, maxX),
            yLayer,
            random.Next(minZ, maxZ)
            );
    }
}
