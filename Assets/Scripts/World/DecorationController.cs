
using System.Collections.Generic;
using UnityEngine;

public enum DecorationType
{
    Voxel,
    Prefab
}

public enum VoxelDecorationType
{
    VoxelRock,
    VoxelTree
}

public enum PlacementSpots
{
    Floor,
    Ceiling,
    LeftWall,
    RightWall,
    ForwardWall,
    BackWall
}


public class DecorationController : MonoBehaviour
{
    [System.Serializable]
    public class LevelDecorationCollection
    {
        public Level level;
        public List<DecorationDefinitionSO> decorations = new();
    }

    [Header("Relevant Manager")]
    [SerializeField] private WorldManager worldManager;

    [Header("Generation Parameters")]
    [SerializeField, Tooltip("Number of tries to spawn decorations per chunk")] private float decorationDensity;

    [Header("Available Decorations")]
    [SerializeField]
    private List<LevelDecorationCollection> availableDecorations = new();

    [Header("For Debugging")]
    [SerializeField] private Transform decorationsRoot;
    [SerializeField] private List<Vector3> placedDecorations = new();

    public void DecorateWorld(Level level)
    {
        List<DecorationDefinitionSO> availableDecorations = GetAllDecorationsForLevel(level);
        IReadOnlyDictionary<Vector3Int, VoxelChunk> worldChunks = worldManager.GetAllWorldChunks;

        foreach (KeyValuePair<Vector3Int, VoxelChunk> chunkEntry in worldChunks)
        {
            VoxelChunk chunk = chunkEntry.Value;

            DecorateChunk(chunk, availableDecorations);
        }
    }

    private void DecorateChunk(VoxelChunk chunk, List<DecorationDefinitionSO> decorations)
    {
        for (int i = 0; i < decorationDensity; i++)
        {
            //get a random index for the decoration thats gonna try and spawn
            int randomDecorationIndex = (int)Random.Range(0, decorations.Count);

            /*
                Get a random direction from which the Decoration should spawn
                Directions table
                0 -- Floor
                1 -- Ceiling
                2 -- Left Wall
                3 -- Right Wall
                4 -- Front Wall
                5 -- Back Wall
            */
            int randomDecorationDirection = (int)Random.Range(0, 6);

            int x,y,z = 0;
            bool success = false;
            Vector3Int spawnPoint = new Vector3Int();

            switch (randomDecorationDirection)
            {
                case 0:
                    x = Random.Range(0, chunk.chunkSize);
                    z = Random.Range(0, chunk.chunkSize);

                    success = chunk.TryGetBottomOYVoxel(x, z, out spawnPoint);
                    if (success)
                        TrySpawnDecoration(decorations[randomDecorationIndex], spawnPoint, 0, 
                            worldManager.currentLevelType);

                    break;
                case 1:
                    x = Random.Range(0, chunk.chunkSize);
                    z = Random.Range(0, chunk.chunkSize);    

                    success = chunk.TryGetTopOYVoxel(x,z, out spawnPoint);
                    
                    if(success)
                        TrySpawnDecoration(decorations[randomDecorationIndex], spawnPoint, 1, 
                            worldManager.currentLevelType);

                    break;
                case 2:
                    y = Random.Range(0, chunk.chunkSize);    
                    z = Random.Range(0, chunk.chunkSize);    

                    success = chunk.TryGetFirstOXVoxel(y,z, out spawnPoint);

                    if(success)
                        TrySpawnDecoration(decorations[randomDecorationIndex], spawnPoint, 2, 
                            worldManager.currentLevelType);

                    break;
                case 3:
                    y = Random.Range(0, chunk.chunkSize);    
                    z = Random.Range(0, chunk.chunkSize);    

                    success = chunk.TryGetLastOXVoxel(y,z, out spawnPoint);

                    if(success)
                        TrySpawnDecoration(decorations[randomDecorationIndex], spawnPoint, 3, 
                            worldManager.currentLevelType);

                    break;
                case 4:
                    x = Random.Range(0, chunk.chunkSize);    
                    y = Random.Range(0, chunk.chunkSize);    

                    success = chunk.TryGetFirstOZVoxel(x,y, out spawnPoint);

                    if(success)
                        TrySpawnDecoration(decorations[randomDecorationIndex], spawnPoint, 4, 
                            worldManager.currentLevelType);

                    break;                
                case 5:
                    x = Random.Range(0, chunk.chunkSize);    
                    y = Random.Range(0, chunk.chunkSize);    

                    success = chunk.TryGetLastOZVoxel(x,y, out spawnPoint);

                    if(success)
                        TrySpawnDecoration(decorations[randomDecorationIndex], spawnPoint, 5, 
                            worldManager.currentLevelType);   

                    break;  
            }
        }
    }

    public bool TrySpawnDecoration(DecorationDefinitionSO decorationDefinition, 
        Vector3 position, int direction, Level currentLevel)
    {
        if(decorationDefinition.ClearanceHeight < 0)
            return false;

        GameObject newDecoration = decorationDefinition.prefab;

        

        return false;
    }

    /*
        Helper method to get all available decorations for the given level
    */

    public List<DecorationDefinitionSO> GetAllDecorationsForLevel(Level level)
    {
        foreach (LevelDecorationCollection innerList in availableDecorations)
        {
            if (innerList.level == level)
            {
                return innerList.decorations;
            }
        }

        //return default surface decorations
        //would make for a funny intentional bug instead of breaking everything
        return availableDecorations[0].decorations;
    }


    /*
        Kill all decoration children
    */
    public void ClearDecorations()
    {
        Transform parentTransform = decorationsRoot != null ? decorationsRoot : transform;

        List<GameObject> childrenToDestroy = new List<GameObject>();

        for (int i = 0; i < placedDecorations.Count; i++)
        {
            childrenToDestroy.Add(parentTransform.GetChild(i).gameObject);
        }

        foreach (GameObject child in childrenToDestroy)
        {
            Destroy(child);
        }
    }
}
