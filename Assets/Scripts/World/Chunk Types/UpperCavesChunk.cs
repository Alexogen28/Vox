using JetBrains.Annotations;
using UnityEngine;

public class UpperCavesChunk : VoxelChunk
{
    private float noiseScale = 0.01f;
    private float noiseHeightScale = 0.5f;
    private float persistence = 0.3f;
    private float lacunarity = 5.0f;

    private float[,] mapXOY;
    private float[,] mapXOZ;
    private float[,] mapYOZ;

    protected override void GenerateChunkData()
    {
        Vector3 floatChunkCoord = new Vector3(
            (float)chunkCoord.x,
            (float)chunkCoord.y,
            (float)chunkCoord.z
        );

        mapXOY = new float[chunkSize, chunkSize];
        mapXOZ = new float[chunkSize, chunkSize];
        mapYOZ = new float[chunkSize, chunkSize];

        Vector3 chunkVoxelWorldPosition = floatChunkCoord * chunkSize;
        Vector2 xOffset = Seed.GetNoiseOffset(worldSeed, AvailableSeedKeys.UpperCavesTerrain, new Vector3Int(0, 0, 0));
        Vector2 yOffset = Seed.GetNoiseOffset(worldSeed, AvailableSeedKeys.UpperCavesTerrain, new Vector3Int(1, 0, 0));
        Vector2 zOffset = Seed.GetNoiseOffset(worldSeed, AvailableSeedKeys.UpperCavesTerrain, new Vector3Int(2, 0, 0));
        Vector2 layerOffset = Seed.GetNoiseOffset(worldSeed, AvailableSeedKeys.UpperCavesTerrain, new Vector3Int(chunkCoord.y, 3, 0));
        //Vector2 globalOffset = Seed.GetNoiseOffset(worldSeed, AvailableSeedKeys.UpperCavesTerrain);

        int x, y, z = 0;
        float worldX, worldY, worldZ;

        //Calculate the YOZ height plane
        for (z = 0; z < chunkSize; z++)
        {
            for (y = 0; y < chunkSize; y++)
            {
                worldZ = chunkVoxelWorldPosition.z + z;
                worldY = chunkVoxelWorldPosition.y + y;
                mapYOZ[y, z] = Mathf.PerlinNoise(
                    (worldZ + xOffset.x) * noiseScale,
                    (worldY + xOffset.y) * noiseScale
                );
            }
        }

        //Calculate the XOZ height plane
        for (x = 0; x < chunkSize; x++)
        {
            for (z = 0; z < chunkSize; z++)
            {
                worldX = chunkVoxelWorldPosition.x + x;
                worldZ = chunkVoxelWorldPosition.z + z;
                mapXOZ[x, z] = Mathf.PerlinNoise(
                    (worldX + yOffset.x) * noiseScale,
                    (worldZ + yOffset.y) * noiseScale
                );
            }
        }

        //Calculate the XOY height plane
        for (x = 0; x < chunkSize; x++)
        {
            for (y = 0; y < chunkSize; y++)
            {
                worldX = chunkVoxelWorldPosition.x + x;
                worldY = chunkVoxelWorldPosition.y + y;
                mapXOY[x, y] = Mathf.PerlinNoise(
                    (worldX + zOffset.x) * noiseScale,
                    (worldY + zOffset.y) * noiseScale
                );
            }
        }


        for (x = 0; x < chunkSize; x++)
            for (z = 0; z < chunkSize; z++)
            {
                float amplitude = noiseHeightScale * chunkSize;
                float frequency = noiseScale;
                float caveCenterY = 0.0f;

                for (int i = 0; i < 2; i++)
                {
                    caveCenterY += Mathf.PerlinNoise(
                        (chunkVoxelWorldPosition.x + x + layerOffset.x) * frequency,
                        (chunkVoxelWorldPosition.z + z + layerOffset.y) * frequency
                        ) * amplitude;

                    amplitude *= persistence;
                    frequency *= lacunarity;
                }

                for (y = 0; y < chunkSize; y++)
                {
                    float distFromCenter = Mathf.Abs(y - caveCenterY);

                    float thicknessNoise = (mapXOY[x, y] + mapYOZ[y, z]) / 2f;
                    float bandThickness = Mathf.Lerp(8f, 20f, thicknessNoise);

                    if (distFromCenter < bandThickness)
                        voxels[x, y, z] = 1;
                }
            }


        //fill in the sides of the cave if we're at the edge
        //or the top of the cave if we're on the top chunk
        BuildEdges();
    }

    private void BuildEdges()
    {
        int x, y, z;
        if (chunkCoord.x == 0)
        {
            for (z = 0; z < chunkSize; z++)
                for (y = 0; y < chunkSize; y++)
                {
                    voxels[0, y, z] = 1;
                }
        }

        if (chunkCoord.x == worldSize.x - 1)
        {
            for (z = 0; z < chunkSize; z++)
                for (y = 0; y < chunkSize; y++)
                {
                    voxels[chunkSize - 1, y, z] = 1;
                }
        }

        if (chunkCoord.z == 0)
        {
            for (x = 0; x < chunkSize; x++)
                for (y = 0; y < chunkSize; y++)
                {
                    voxels[x, y, 0] = 1;
                }
        }

        if (chunkCoord.z == worldSize.z - 1)
        {
            for (x = 0; x < chunkSize; x++)
                for (y = 0; y < chunkSize; y++)
                {
                    voxels[x, y, chunkSize - 1] = 1;
                }
        }

        if (chunkCoord.y == worldSize.y - 1)
        {
            for (x = 0; x < chunkSize; x++)
                for (z = 0; z < chunkSize; z++)
                {
                    voxels[x, chunkSize - 1, z] = 1;
                }
        }
    }
}
