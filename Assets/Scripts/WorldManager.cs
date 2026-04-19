using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    [Header ("World Stats")]
    [SerializeField] public int worldSizeX = 4;
    [SerializeField] public int worldSizeY = 1;
    [SerializeField] public int worldSizeZ = 4;
 
    [Header("Chunk Stats")]
    [SerializeField] public int chunkSize = 64;
    [SerializeField] public float voxelSize = 0.75f;

    [Header("Temporary Materials Header")]
    [SerializeField] public Material baseMat;

    [Header("World Gen Parameters")]
    private int worldSeed = 12345;
    private bool randomiseIntOnPlay = true;
    public int seedMinimumValue = 1;
    public int seedMaximumValue = int.MaxValue;

    private Vector3Int worldSize;

    private Dictionary<Vector3Int, VoxelChunk> worldChunks = new();


    void Start()
    {
        //GenerateWorld<SurfaceChunk>();
    }

    private void SetRandomSeed()
    {
        worldSeed = Random.Range(seedMinimumValue, seedMaximumValue);
    }

    public void GenerateWorldOnStartup()
    {
        if(randomiseIntOnPlay) SetRandomSeed();
        worldSize = new Vector3Int(worldSizeX, worldSizeY, worldSizeZ);
        GenerateWorld<SurfaceChunk>();
    }

    void GenerateWorld<T>() where T : VoxelChunk
    {
        worldChunks.Clear();

        Debug.Log("Generating world...");

        for (int x = 0; x < worldSizeX; x++)
        {
            for(int  y = 0; y < worldSizeY; y++)
            {
                for(int z = 0; z < worldSizeZ; z++)
                {
                    var coord = new Vector3Int(x, y, z);

                    Vector3 worldCoord = new Vector3(x, y, z) * (chunkSize * voxelSize);

                    var chunkGameObject = new GameObject($"Chunk_{x}_{y}_{z}");

                    chunkGameObject.transform.SetParent(transform, false);
                    chunkGameObject.transform.localPosition = worldCoord;
                    chunkGameObject.layer = LayerMask.NameToLayer("Ground");

                    var chunk = chunkGameObject.AddComponent<T>();
                    chunk.Initialize(chunkSize, voxelSize, baseMat, worldSeed, coord, worldSize, this);
                    
                    worldChunks.Add(coord, chunk);
                    Debug.Log($"Creating chunk at {coord} / worldPos {worldCoord}");
                }
            }    
        }
    }

    public VoxelChunk GetChunk(Vector3Int coord) =>
        worldChunks.TryGetValue(coord, out var chunk) ? chunk : null;

    public Vector3Int GetRandomChunkCoords()
    {
        int x = (int) Random.Range(1, worldSizeX-1);
        int y = (int) Random.Range(1, worldSizeY-1);
        int z = (int) Random.Range(1, worldSizeZ-1);

        Vector3Int chunkCoords = new Vector3Int(x,y,z);
        return chunkCoords;
    }
}
