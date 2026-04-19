using UnityEngine;
using System;

public class VoxelRock : VoxelObject
{
    protected override void GenerateVoxelData()
    {
        Vector3Int worldPositionInt = new Vector3Int(
            (int)worldPosition.x,
            (int)worldPosition.y,
            (int)worldPosition.z
        );
        var rng = Seed.CreateRandom(worldSeed, AvailableSeedKeys.SurfaceDetails, worldPositionInt);

        Vector3 center = new Vector3(
            dimensions.x / 2f,
            dimensions.y / 2f,
            dimensions.z / 2f
        );

        float radius = Mathf.Min(dimensions.x, dimensions.y, dimensions.z) * 0.4f;

        for (int x = 0; x < dimensions.x; x++)
        {
            for (int y = 0; y < dimensions.y; y++)
            {
                for (int z = 0; z < dimensions.z; z++)
                {
                    Vector3 position = new Vector3(x, y, z);
                    float dist = Vector3.Distance(position, center);

                    float variation = ((float)rng.NextDouble() * 1.0f) - 0.5f;

                    if (dist <= radius + variation)
                        voxels[x, y, z] = 1;
                }
            }
        }
    }
}