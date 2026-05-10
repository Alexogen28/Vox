using UnityEngine;

public class VoxelTree : VoxelObject
{
    /*
        Voxel IDs:
        1 -> trunk material
        2 -> leaf material
    */
    private Vector3Int worldPositionInt;

    protected override void GenerateVoxelData()
    {
        worldPositionInt = new Vector3Int(
            Mathf.RoundToInt(worldPosition.x),
            Mathf.RoundToInt(worldPosition.y),
            Mathf.RoundToInt(worldPosition.z)
        );

        System.Random treeRandomGenerator = Seed.CreateRandom(
            worldSeed,
            AvailableSeedKeys.SurfaceDetails,
            worldPositionInt
        );

        int trunkBaseX = dimensions.x / 2;
        int trunkBaseZ = dimensions.z / 2;

        int trunkHeight = Mathf.Clamp(
            dimensions.y / 2 + treeRandomGenerator.Next(-1, 3),
            3,
            dimensions.y - 3
        );

        int trunkLeanX = treeRandomGenerator.Next(-1, 2);
        int trunkLeanZ = treeRandomGenerator.Next(-1, 2);

        GenerateTrunk(trunkBaseX, trunkBaseZ, trunkHeight, trunkLeanX, trunkLeanZ);
        GenerateBranches(treeRandomGenerator, trunkBaseX, trunkBaseZ, trunkHeight, trunkLeanX, trunkLeanZ);
        GenerateLeaves(treeRandomGenerator, trunkBaseX + trunkLeanX, trunkBaseZ + trunkLeanZ, trunkHeight);
    }

    private void GenerateTrunk(int trunkBaseX, int trunkBaseZ, int trunkHeight, int trunkLeanX, int trunkLeanZ)
    {
        for (int y = 0; y < trunkHeight; y++)
        {
            float heightPercent = y / Mathf.Max(1f, trunkHeight - 1f);

            int trunkX = trunkBaseX + Mathf.RoundToInt(trunkLeanX * heightPercent);
            int trunkZ = trunkBaseZ + Mathf.RoundToInt(trunkLeanZ * heightPercent);

            float trunkThicknessNoise = Seed.Range01(
                worldSeed,
                AvailableSeedKeys.SurfaceDetails,
                worldPositionInt + new Vector3Int(trunkX, y, trunkZ)
            );

            int trunkRadius = y < 2 || trunkThicknessNoise > 0.82f ? 1 : 0;

            FillSphereLikeBlob(trunkX, y, trunkZ, trunkRadius, 1);
        }
    }

    private void GenerateBranches(
        System.Random treeRandomGenerator,
        int trunkBaseX,
        int trunkBaseZ,
        int trunkHeight,
        int trunkLeanX,
        int trunkLeanZ
    )
    {
        int branchCount = treeRandomGenerator.Next(2, 5);

        for (int branchIndex = 0; branchIndex < branchCount; branchIndex++)
        {
            int branchStartY = treeRandomGenerator.Next(
                Mathf.Max(2, trunkHeight / 2),
                trunkHeight
            );

            float heightPercent = branchStartY / Mathf.Max(1f, trunkHeight - 1f);

            int branchX = trunkBaseX + Mathf.RoundToInt(trunkLeanX * heightPercent);
            int branchZ = trunkBaseZ + Mathf.RoundToInt(trunkLeanZ * heightPercent);

            Vector3Int branchDirection = GetRandomHorizontalDirection(treeRandomGenerator);
            int branchLength = treeRandomGenerator.Next(1, 3);

            for (int branchStep = 1; branchStep <= branchLength; branchStep++)
            {
                int x = branchX + branchDirection.x * branchStep;
                int y = branchStartY + branchStep / 2;
                int z = branchZ + branchDirection.z * branchStep;

                SetVoxelIfInside(x, y, z, 1);
            }
        }
    }

    private void GenerateLeaves(
        System.Random treeRandomGenerator,
        int leafCenterX,
        int leafCenterZ,
        int trunkHeight
    )
    {
        Vector3 leafCenter = new Vector3(
            leafCenterX + treeRandomGenerator.Next(-1, 2),
            trunkHeight + treeRandomGenerator.Next(1, 3),
            leafCenterZ + treeRandomGenerator.Next(-1, 2)
        );

        float radiusX = dimensions.x * RandomRange(treeRandomGenerator, 0.28f, 0.42f);
        float radiusY = dimensions.y * RandomRange(treeRandomGenerator, 0.18f, 0.28f);
        float radiusZ = dimensions.z * RandomRange(treeRandomGenerator, 0.28f, 0.42f);

        Vector2 leafNoiseOffset = Seed.GetNoiseOffset(
            worldSeed,
            AvailableSeedKeys.SurfaceDetails,
            worldPositionInt
        );

        for (int x = 0; x < dimensions.x; x++)
        {
            for (int y = Mathf.Max(0, trunkHeight - 1); y < dimensions.y; y++)
            {
                for (int z = 0; z < dimensions.z; z++)
                {
                    Vector3 localPosition = new Vector3(x, y, z);

                    float normalizedDistance =
                        Mathf.Pow((localPosition.x - leafCenter.x) / radiusX, 2f) +
                        Mathf.Pow((localPosition.y - leafCenter.y) / radiusY, 2f) +
                        Mathf.Pow((localPosition.z - leafCenter.z) / radiusZ, 2f);

                    float softLeafNoise = Mathf.PerlinNoise(
                        (leafNoiseOffset.x + worldPosition.x + x) * 0.45f,
                        (leafNoiseOffset.y + worldPosition.z + z) * 0.45f
                    );

                    float coordinateLeafNoise = Seed.Range01(
                        worldSeed,
                        AvailableSeedKeys.SurfaceDetails,
                        worldPositionInt + new Vector3Int(x, y, z)
                    );

                    float edgeNoise =
                        Mathf.Lerp(0.78f, 1.16f, softLeafNoise) +
                        Mathf.Lerp(-0.08f, 0.08f, coordinateLeafNoise);

                    if (normalizedDistance <= edgeNoise)
                    {
                        voxels[x, y, z] = 2;
                    }
                }
            }
        }

        int leafHoleCount = Mathf.Clamp(dimensions.x / 2, 2, 5);

        for (int leafHoleIndex = 0; leafHoleIndex < leafHoleCount; leafHoleIndex++)
        {
            Vector3Int holeCoordinate = new Vector3Int(
                treeRandomGenerator.Next(1, dimensions.x - 1),
                treeRandomGenerator.Next(Mathf.Max(1, trunkHeight), dimensions.y - 1),
                treeRandomGenerator.Next(1, dimensions.z - 1)
            );

            float holeChance = Seed.Range01(
                worldSeed,
                AvailableSeedKeys.SurfaceDetails,
                worldPositionInt + holeCoordinate
            );

            if (holeChance > 0.65f && voxels[holeCoordinate.x, holeCoordinate.y, holeCoordinate.z] == 2)
            {
                voxels[holeCoordinate.x, holeCoordinate.y, holeCoordinate.z] = 0;
            }
        }
    }

    private Vector3Int GetRandomHorizontalDirection(System.Random treeRandomGenerator)
    {
        Vector3Int[] possibleDirections =
        {
            new Vector3Int(1, 0, 0),
            new Vector3Int(-1, 0, 0),
            new Vector3Int(0, 0, 1),
            new Vector3Int(0, 0, -1)
        };

        return possibleDirections[treeRandomGenerator.Next(0, possibleDirections.Length)];
    }

    private void FillSphereLikeBlob(int centerX, int centerY, int centerZ, int radius, byte voxelId)
    {
        for (int x = centerX - radius; x <= centerX + radius; x++)
        {
            for (int y = centerY - radius; y <= centerY + radius; y++)
            {
                for (int z = centerZ - radius; z <= centerZ + radius; z++)
                {
                    Vector3 position = new Vector3(x, y, z);
                    Vector3 center = new Vector3(centerX, centerY, centerZ);

                    if (Vector3.Distance(position, center) <= radius + 0.25f)
                    {
                        SetVoxelIfInside(x, y, z, voxelId);
                    }
                }
            }
        }
    }

    private void SetVoxelIfInside(int x, int y, int z, byte voxelId)
    {
        if (x < 0 || y < 0 || z < 0 ||
            x >= dimensions.x || y >= dimensions.y || z >= dimensions.z)
        {
            return;
        }

        voxels[x, y, z] = voxelId;
    }

    private float RandomRange(System.Random treeRandomGenerator, float minInclusive, float maxInclusive)
    {
        return Mathf.Lerp(minInclusive, maxInclusive, (float)treeRandomGenerator.NextDouble());
    }
}