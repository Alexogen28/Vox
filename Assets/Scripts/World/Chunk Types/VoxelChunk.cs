using System.Collections.Generic;
using Unity.VisualScripting;
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
    public Level level;

    public byte[,,] voxels;
    protected MeshFilter meshFilter;
    protected MeshCollider meshCollider;
    protected MeshRenderer meshRenderer;

    protected virtual void Awake()
    {
        meshFilter   = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();
        meshRenderer = GetComponent<MeshRenderer>();
    }


    public void Initialize(int chunkSize, float voxelSize, Material material, int worldSeed, Vector3Int chunkCoord, Vector3Int worldSize, WorldManager worldManager, Level level)
    {
        this.chunkSize = chunkSize;
        this.voxelSize = voxelSize;
        this.worldSeed = worldSeed;
        this.chunkCoord = chunkCoord;
        this.worldSize = worldSize;
        this.worldManager = worldManager;
        this.level = level;

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


    /*
        Query the chunk from the left to the right at a given Y,Z coordinate
        and return the first Solid voxel in front of it which is Air
    */
    public bool TryGetFirstOXVoxel(int y, int z, out Vector3Int positionInChunk)
    {
        for(int x = 0; x < chunkSize-1; x++)
        {
            if(voxels[x,y,z] != 0 && voxels[x+1,y,z] == 0)
            {
                positionInChunk = new Vector3Int(x,y,z);
                return true;
            }
        }

        positionInChunk = default;
        return false;
    }

    /*
        Query the chunk from the right to the left at a given Y,Z coordinate
        and return the first Solid voxel in front of it which is Air
    */
    public bool TryGetLastOXVoxel(int y, int z, out Vector3Int positionInChunk)
    {
        for(int x = chunkSize-1; x > 0; x--)
        {
            if(voxels[x,y,z] != 0 && voxels[x-1,y,z] == 0)
            {
                positionInChunk = new Vector3Int(x,y,z);
                return true;
            }
        }

        positionInChunk = default;
        return false;
    }   


    /*
        Query the chunk from the bottom to the top at a given X,Z coordinate
        and return the first Solid voxel above which is Air
    */
    public bool TryGetBottomOYVoxel(int x, int z, out Vector3Int positionInChunk)
    {
        for(int y = 0; y < chunkSize-1; y++)
        {
            if(voxels[x,y,z] !=0 && voxels[x,y+1,z] == 0)
            {
                positionInChunk = new Vector3Int(x,y,z);
                return true;
            }
        }

        positionInChunk = default;
        return false;
    }

    /*
        Query the chunk from the top to the bottom at a given X,Z coordinate
        and return the first Solid voxel under which is Air
    */
    public bool TryGetTopOYVoxel(int x, int z, out Vector3Int positionInChunk)
    {
        for(int y = chunkSize-1; y > 0; y--)
        {
            if(voxels[x,y,z] != 0 && voxels[x,y-1,z] == 0)
            {
                positionInChunk = new Vector3Int(x,y,z);
                return true;
            }
        }

        positionInChunk = default;
        return false;
    }


    /*
        Query the chunk from the back to the front at a given X,Y coordinate
        and return the first Solid voxel in front of it which is Air
    */
    public bool TryGetFirstOZVoxel(int x, int y, out Vector3Int positionInChunk)
    {
        for(int z = 0; z < chunkSize-1; z++)
        {
            if(voxels[x,y,z] != 0 && voxels[x,y,z+1] == 0)
            {
                positionInChunk = new Vector3Int(x,y,z);
                return true;
            }
        }

        positionInChunk = default;
        return false;
    }

    /*
        Query the chunk from the front to the back at a given X,Y coordinate
        and return the first Solid voxel in front of it which is Air
    */
    public bool TryGetLastOZVoxel(int x, int y, out Vector3Int positionInChunk)
    {
        for(int z = chunkSize-1 ; z > 0; z--)
        {
            if(voxels[x,y,z] != 0 && voxels[x,y,z-1] == 0)
            {
                positionInChunk = new Vector3Int(x,y,z);
                return true;
            }
        }

        positionInChunk = default;
        return false;
    }
}
