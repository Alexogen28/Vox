using UnityEngine;
using System;

public class VoxelTree : VoxelObject
{
    protected override void GenerateVoxelData()
    {
        Vector3Int worldPositionInt = new Vector3Int(
            (int)worldPosition.x,
            (int)worldPosition.y,
            (int)worldPosition.z
        );
        
        var rng = Seed.CreateRandom(worldSeed, AvailableSeedKeys.SurfaceDetails, worldPositionInt);

        int trunkX = dimensions.x / 2;
        int trunkZ = dimensions.z / 2;

        int trunkHeight = Mathf.Max(2, dimensions.y / 2 + rng.Next(-1, 2));

        for (int y = 0; y < trunkHeight; y++)
            voxels[trunkX, y, trunkZ] = 1;

        Vector3 leafCenter = new Vector3(
            trunkX + rng.Next(-1, 2),
            trunkHeight + 1,
            trunkZ + rng.Next(-1, 2)
        );

        float leafRadius = Mathf.Min(dimensions.x, dimensions.z) *
                           (0.35f + (float)rng.NextDouble() * 0.2f);

        for (int x = 0; x < dimensions.x; x++)
        {
            for (int y = trunkHeight; y < dimensions.y; y++)
            {
                for (int z = 0; z < dimensions.z; z++)
                {
                    Vector3 p = new Vector3(x, y, z);

                    if (Vector3.Distance(p, leafCenter) <= leafRadius)
                        voxels[x, y, z] = 2;
                }
            }
        }
    }
}