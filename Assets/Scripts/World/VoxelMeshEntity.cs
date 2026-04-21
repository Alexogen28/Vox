using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshCollider))]
[RequireComponent(typeof(MeshRenderer))]
public abstract class VoxelMeshEntity : MonoBehaviour
{
    [Header("Voxel Settings")]
    [SerializeField] protected float voxelScale = 1f;

    protected byte[,,] voxels;
    protected Vector3Int dimensions;

    protected MeshFilter meshFilter;
    protected MeshCollider meshCollider;
    protected MeshRenderer meshRenderer;

    private static readonly Vector3Int[] Directions =
    {
        new Vector3Int(0, 1, 0),
        new Vector3Int(0, -1, 0),
        new Vector3Int(0, 0, 1),
        new Vector3Int(0, 0, -1),
        new Vector3Int(1, 0, 0),
        new Vector3Int(-1, 0, 0),
    };

    private static readonly Vector3[][] FaceVertices =
    {
        new Vector3[] { new(0,1,0), new(0,1,1), new(1,1,1), new(1,1,0) },
        new Vector3[] { new(0,0,0), new(1,0,0), new(1,0,1), new(0,0,1) },
        new Vector3[] { new(0,0,1), new(1,0,1), new(1,1,1), new(0,1,1) },
        new Vector3[] { new(1,0,0), new(0,0,0), new(0,1,0), new(1,1,0) },
        new Vector3[] { new(1,0,1), new(1,0,0), new(1,1,0), new(1,1,1) },
        new Vector3[] { new(0,0,0), new(0,0,1), new(0,1,1), new(0,1,0) }
    };

    protected virtual void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    protected void InitializeVoxelData(Vector3Int dimensions, float voxelScale, Material material)
    {
        this.dimensions = dimensions;
        this.voxelScale = voxelScale;

        if (material != null)
            meshRenderer.sharedMaterial = material;

        voxels = new byte[dimensions.x, dimensions.y, dimensions.z];
    }

    protected abstract void GenerateVoxelData();

    public void Build()
    {
        GenerateVoxelData();
        GenerateMesh();
    }

    protected virtual void GenerateMesh()
    {
        var vertices = new List<Vector3>();
        var triangles = new List<int>();

        for (int x = 0; x < dimensions.x; x++)
        {
            for (int y = 0; y < dimensions.y; y++)
            {
                for (int z = 0; z < dimensions.z; z++)
                {
                    if (voxels[x, y, z] == 0) continue;

                    Vector3 pos = new Vector3(x, y, z) * voxelScale;
                    AddCubeFaces(x, y, z, pos, vertices, triangles);
                }
            }
        }

        var mesh = new Mesh { indexFormat = UnityEngine.Rendering.IndexFormat.UInt32 };
        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
        mesh.RecalculateNormals();

        meshFilter.sharedMesh = mesh;
        meshCollider.sharedMesh = null;
        meshCollider.sharedMesh = mesh;
    }

    protected virtual void AddCubeFaces(int x, int y, int z, Vector3 pos, List<Vector3> vertices, List<int> triangles)
    {
        int startIndex = vertices.Count;

        for (int i = 0; i < 6; i++)
        {
            Vector3Int dir = Directions[i];
            int nx = x + dir.x;
            int ny = y + dir.y;
            int nz = z + dir.z;

            if (!IsVoxelSolid(nx, ny, nz))
            {
                foreach (var v in FaceVertices[i])
                    vertices.Add(pos + v * voxelScale);

                triangles.Add(startIndex);
                triangles.Add(startIndex + 1);
                triangles.Add(startIndex + 2);
                triangles.Add(startIndex);
                triangles.Add(startIndex + 2);
                triangles.Add(startIndex + 3);

                startIndex += 4;
            }
        }
    }

    protected virtual bool IsVoxelSolid(int x, int y, int z)
    {
        if (x < 0 || y < 0 || z < 0 ||
            x >= dimensions.x || y >= dimensions.y || z >= dimensions.z)
        {
            return false;
        }

        return voxels[x, y, z] != 0;
    }
}