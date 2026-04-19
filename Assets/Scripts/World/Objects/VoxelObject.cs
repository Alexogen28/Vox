using UnityEngine;

public abstract class VoxelObject : VoxelMeshEntity
{
    [Header("Object Metadata")]
    [SerializeField] protected Vector3Int objectSize = new Vector3Int(8, 8, 8);

    protected int worldSeed;
    protected Vector3 worldPosition;

    public virtual void InitializeObject(Vector3Int objectSize, float voxelSize, Material material, int worldSeed, Vector3 worldPosition)
    {
        this.worldSeed = worldSeed;
        this.worldPosition = worldPosition;

        InitializeVoxelData(objectSize, voxelSize, material);
    }
}