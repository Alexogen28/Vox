
using System.Collections.Generic;
using System.Data.Common;
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
        public LevelSO level;
        public List<DecorationDefinitionSO> decorations = new();
    }

    [SerializeField] private GameManager gameManager;

    [Header("Generation Parameters")]
    [SerializeField, Tooltip("Number of tries to spawn decorations per chunk")] 
    private int decorationDensity;

    [Header("Available Decorations")]
    [SerializeField] private List<LevelDecorationCollection> availableDecorations;
    [SerializeField] private GameObject descentWell;

    [Header("For Debugging")]
    [SerializeField] private Transform decorationsRoot;

    [Header("Parent of all decoration objects")]
    [SerializeField] private GameObject decorationsParent;

    public void DecorateWorld(LevelSO level)
    {
        foreach(Transform child in decorationsParent.transform)
        {
            Destroy(child.gameObject);
        }

        List<DecorationDefinitionSO> availableDecorations = GetAllDecorationsForLevel(level);
        //Debug.Log("Snatched all available decorations");

        IReadOnlyDictionary<Vector3Int, VoxelChunk> worldChunks = gameManager.worldManager.GetAllWorldChunks;
        //Debug.Log("Got all available worldChunks");

        foreach (KeyValuePair<Vector3Int, VoxelChunk> chunkEntry in worldChunks)
        {
            VoxelChunk chunk = chunkEntry.Value;

            //Debug.Log("Decorating chunk" + chunk.gameObject.name);
            DecorateChunk(chunk, availableDecorations);
        }
    }

    private void DecorateChunk(VoxelChunk chunk, List<DecorationDefinitionSO> decorations)
    {
        int runtimeDecorationDensity = decorationDensity;
        if(gameManager.levelManager.CurrentLevel.levelName == LevelName.Surface)
            runtimeDecorationDensity *= 3;

        for (int i = 0; i < runtimeDecorationDensity; i++)
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
            Vector3 spawnPoint = new Vector3();

            switch (randomDecorationDirection)
            {
                case 0:
                    x = Random.Range(0, chunk.chunkSize);
                    z = Random.Range(0, chunk.chunkSize);

                    success = chunk.TryGetBottomOYVoxel(x, z, out spawnPoint);

                    if (success)
                        TrySpawnDecoration(decorations[randomDecorationIndex], spawnPoint, 0);

                    break;
                case 1:
                    x = Random.Range(0, chunk.chunkSize);
                    z = Random.Range(0, chunk.chunkSize);    

                    success = chunk.TryGetTopOYVoxel(x,z, out spawnPoint);
                    
                    if(success)
                        TrySpawnDecoration(decorations[randomDecorationIndex], spawnPoint, 1);

                    break;
                case 2:
                    y = Random.Range(0, chunk.chunkSize);    
                    z = Random.Range(0, chunk.chunkSize);    

                    success = chunk.TryGetFirstOXVoxel(y,z, out spawnPoint);

                    if(success)
                        TrySpawnDecoration(decorations[randomDecorationIndex], spawnPoint, 2);

                    break;
                case 3:
                    y = Random.Range(0, chunk.chunkSize);    
                    z = Random.Range(0, chunk.chunkSize);    

                    success = chunk.TryGetLastOXVoxel(y,z, out spawnPoint);

                    if(success)
                        TrySpawnDecoration(decorations[randomDecorationIndex], spawnPoint, 3);

                    break;
                case 4:
                    x = Random.Range(0, chunk.chunkSize);    
                    y = Random.Range(0, chunk.chunkSize);    

                    success = chunk.TryGetFirstOZVoxel(x,y, out spawnPoint);

                    if(success)
                        TrySpawnDecoration(decorations[randomDecorationIndex], spawnPoint, 4);

                    break;                
                case 5:
                    x = Random.Range(0, chunk.chunkSize);    
                    y = Random.Range(0, chunk.chunkSize);    

                    success = chunk.TryGetLastOZVoxel(x,y, out spawnPoint);

                    if(success)
                        TrySpawnDecoration(decorations[randomDecorationIndex], spawnPoint, 5);   

                    break;  
            }
        }
    }


    /*
        Attempt to spawn the decoration following multiple steps

        Create a GameObject with the prefab inside the decorationDefinition SO
        Move it to its spawn location
        Rotate it around if it has to spawn on a wall/ceiling
        If its a voxel object, trigger the Initialise script on it so it generates automatically
    */
    public bool TrySpawnDecoration(DecorationDefinitionSO decorationDefinition,
        Vector3 spawnPosition, int direction)
    {
        if (decorationDefinition.ClearanceHeight < 0)
            return false;

        if(direction > 0 && gameManager.levelManager.CurrentLevel.levelName == LevelName.Surface)
            return false;

        GameObject newDecoration = Instantiate(decorationDefinition.prefab, spawnPosition, 
            Quaternion.identity, decorationsRoot);

        /*                
            Directions table
            0 -- Floor
            1 -- Ceiling
            2 -- Left Wall
            3 -- Right Wall
            4 -- Front Wall
            5 -- Back Wall
        */

        switch (direction)
        {
            case 0:
                newDecoration.transform.rotation = Quaternion.identity;
                break;
            case 1:
                newDecoration.transform.rotation = Quaternion.Euler(0f, 0f, 180f);
                break;
            case 2:
                newDecoration.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
                break;
            case 3:
                newDecoration.transform.rotation = Quaternion.Euler(0f, 0f, 270f);
                break;
            case 4:
                newDecoration.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
                break;
            case 5:
                newDecoration.transform.rotation = Quaternion.Euler(270f, 0f, 0f);

                break;
            default: break;
        }

        VoxelObject voxelObject = newDecoration.GetComponent<VoxelObject>();
        //Debug.Log("Got the following voxelObject:" + voxelObject.name);

        if(voxelObject != null)
        {
            voxelObject.InitializeObject(decorationDefinition.VoxelObjectSize, decorationDefinition.VoxelScale,
                gameManager.worldManager.GetWorldSeed(), spawnPosition, decorationDefinition.materialsList);
            //Debug.Log("Initialised Voxel object" + voxelObject.name);
        }

        newDecoration.transform.parent = decorationsParent.transform;
        return true;
    }

    /*
        Helper method to get all available decorations for the given level

        Just pass in the level and get a List of decorations
    */

    public List<DecorationDefinitionSO> GetAllDecorationsForLevel(LevelSO level)
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

        for (int i = 0; i < decorationsRoot.childCount; i++)
        {
            childrenToDestroy.Add(parentTransform.GetChild(i).gameObject);
        }

        foreach (GameObject child in childrenToDestroy)
        {
            Destroy(child);
        }
    }
}
