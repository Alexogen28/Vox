using System.Collections.Generic;
using UnityEngine;


[DisallowMultipleComponent]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshCollider))]
[RequireComponent(typeof(MeshRenderer))]
public abstract class VoxelChunk : MonoBehaviour
{
    //Chunk settings
    public int chunkSize;
    public float voxelSize;
    public int worldSeed;
    public Vector3Int chunkCoord;
    public Vector3Int worldSize;
    protected WorldManager worldManager;

    protected byte[,,] voxels;
    protected MeshFilter meshFilter;
    protected MeshCollider meshCollider;
    protected MeshRenderer meshRenderer;

    protected virtual void Awake()
    {
        meshFilter   = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();
        meshRenderer = GetComponent<MeshRenderer>();
    }


    public void Initialize(int chunkSize, float voxelSize, Material material, int worldSeed, Vector3Int chunkCoord, Vector3Int worldSize, WorldManager worldManager)
    {
        this.chunkSize = chunkSize;
        this.voxelSize = voxelSize;
        this.worldSeed = worldSeed;
        this.chunkCoord = chunkCoord;
        this.worldSize = worldSize;
        this.worldManager = worldManager;

        meshRenderer.sharedMaterial = material;

        voxels = new byte[chunkSize, chunkSize, chunkSize];
        GenerateChunkData();
        GenerateMesh();
    }

    protected abstract void GenerateChunkData();


    protected virtual void GenerateMesh()
    {
        var vertices = new List<Vector3>();
        var triangles = new List<int>();

        for (int x = 0; x < chunkSize; x++)
            for (int y = 0; y < chunkSize; y++)
                for (int z = 0; z < chunkSize; z++)
                {
                    if (voxels[x, y, z] == 0) continue;
                    Vector3 pos = new Vector3(x, y, z) * voxelSize;
                    AddCubeFaces(x, y, z, pos, vertices, triangles);

                    
                }

        var mesh = new Mesh { indexFormat = UnityEngine.Rendering.IndexFormat.UInt32 };
        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
        mesh.RecalculateNormals();

        meshFilter.sharedMesh = mesh;
        meshCollider.sharedMesh = null;
        meshCollider.sharedMesh = mesh;

        Debug.Log($"{gameObject.name}: vertices={vertices.Count}, triangles={triangles.Count}");
    }

    protected virtual void AddCubeFaces(int x, int y, int z, Vector3 pos, List<Vector3> vertices, List<int> triangles)
    {
        int startIndex = vertices.Count;

        Vector3Int[] directions =
        {
            new Vector3Int(0, 1, 0), // Top
            new Vector3Int(0, -1, 0), // Bottom
            new Vector3Int(0, 0, 1), // Front
            new Vector3Int(0, 0, -1), // Back
            new Vector3Int(1, 0, 0), // Right
            new Vector3Int(-1, 0, 0), // Left
        };

        Vector3[][] faceVertices = {
            // Top (+Y)
            new Vector3[] { new(0,1,0), new(0,1,1), new(1,1,1), new(1,1,0) },
            // Bottom (-Y)
            new Vector3[] { new(0,0,0), new(1,0,0), new(1,0,1), new(0,0,1) },
            // Front (+Z)
            new Vector3[] { new(0,0,1), new(1,0,1), new(1,1,1), new(0,1,1) },
            // Back (-Z)
            new Vector3[] { new(1,0,0), new(0,0,0), new(0,1,0), new(1,1,0) },
            // Right (+X)
            new Vector3[] { new(1,0,1), new(1,0,0), new(1,1,0), new(1,1,1) },
            // Left (-X)
            new Vector3[] { new(0,0,0), new(0,0,1), new(0,1,1), new(0,1,0) }
        };


        for (int i = 0; i < 6; i++)
        {
            Vector3Int dir = directions[i];
            int nx = x + dir.x, ny = y + dir.y, nz = z + dir.z;

            if (!IsVoxelSolid(nx, ny, nz))
            {
                foreach (var v in faceVertices[i])
                    vertices.Add(pos + v * voxelSize);

                triangles.Add(startIndex); triangles.Add(startIndex + 1); triangles.Add(startIndex + 2);
                triangles.Add(startIndex); triangles.Add(startIndex + 2); triangles.Add(startIndex + 3);
                startIndex += 4;
            }
        }
    }

    protected bool IsVoxelSolid(int localX, int localY, int localZ)
    {
        if (localX >= 0 && localX < chunkSize &&
        localY >= 0 && localY < chunkSize &&
        localZ >= 0 && localZ < chunkSize)
        {
            return voxels[localX, localY, localZ] != 0;
        }

        Vector3Int neighborChunkCoord = chunkCoord;
        int neighborLocalX = localX;
        int neighborLocalY = localY;
        int neighborLocalZ = localZ;

        if (neighborLocalX < 0)
        {
            neighborChunkCoord.x--;
            neighborLocalX += chunkSize;
        }
        else if (neighborLocalX >= chunkSize)
        {
            neighborChunkCoord.x++;
            neighborLocalX -= chunkSize;
        }

        if (neighborLocalY < 0)
        {
            neighborChunkCoord.y--;
            neighborLocalY += chunkSize;
        }
        else if (neighborLocalY >= chunkSize)
        {
            neighborChunkCoord.y++;
            neighborLocalY -= chunkSize;
        }

        if (neighborLocalZ < 0)
        {
            neighborChunkCoord.z--;
            neighborLocalZ += chunkSize;
        }
        else if (neighborLocalZ >= chunkSize)
        {
            neighborChunkCoord.z++;
            neighborLocalZ -= chunkSize;
        }

        VoxelChunk neighborChunk = worldManager.GetChunk(neighborChunkCoord);
        if (neighborChunk == null)
            return false;

        return neighborChunk.voxels[neighborLocalX, neighborLocalY, neighborLocalZ] != 0;
    }

}
