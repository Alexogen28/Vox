using System.Collections.Generic;
using UnityEngine;

public abstract class VoxelObject : VoxelMeshEntity
{
    [Header("Object Data")]
    [SerializeField] protected Vector3Int objectSize = new Vector3Int(8, 8, 8);

    public virtual void InitializeObject(Vector3Int objectSize, float voxelScale, 
        int worldSeed, Vector3 worldPosition, Material[] materialsList)
    {
        this.worldSeed = worldSeed;
        this.worldPosition = worldPosition;
        this.materialsList = materialsList;

        InitializeVoxelData(objectSize, voxelScale);
        GenerateVoxelData();
        GenerateMesh();
    }
}