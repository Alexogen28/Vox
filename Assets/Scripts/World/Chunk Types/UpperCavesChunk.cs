using UnityEngine;

public class UpperCavesChunk : VoxelChunk
{
    private float noiseScale = 0.01f;

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

        int solidCount = 0;

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
            for (y = 0; y < chunkSize; y++)
                for (z = 0; z < chunkSize; z++)
                {
                    float caveCenterY = Mathf.PerlinNoise(
                        (chunkVoxelWorldPosition.x + x + layerOffset.x) * noiseScale,
                        (chunkVoxelWorldPosition.z + z + layerOffset.y) * noiseScale
                        ) * chunkSize;

                    float distFromCenter = Mathf.Abs(y - caveCenterY);

                    float thicknessNoise = (mapXOY[x, y] + mapYOZ[y, z]) / 2f;
                    float bandThickness = Mathf.Lerp(8f, 20f, thicknessNoise);

                    if (distFromCenter < bandThickness)
                        voxels[x, y, z] = 1;
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

    public void CarveDescent(int centerX, int centerZ, bool topChunk, int radius)
    {
        Vector3Int positionToCarve = new Vector3Int();  

        if(topChunk)
        {
            TryGetBottomOYVoxelInChunk(centerX, centerZ, out positionToCarve);
            CarveDown(positionToCarve, radius);
        }
        else
        {
            TryGetTopOYVoxelInChunk(centerX, centerZ, out positionToCarve);
            CarveUp(positionToCarve, radius);
        }

        RedrawMesh();
    }

    private void CarveDown(Vector3Int positionToCarve, int radius)
    {
        for (int y = positionToCarve.y; y >= 0; y--)
        {
            for(int x = Mathf.Max(0, positionToCarve.x - radius); x <= Mathf.Min(positionToCarve.x + radius, chunkSize-1); x++)
            {
                for (int z = Mathf.Max(0, positionToCarve.z - radius); z <= Mathf.Min(positionToCarve.z + radius, chunkSize-1); z++)
                {
                    voxels[x, y, z] = 0;
                }
            }
        }
    }

    private void CarveUp(Vector3Int positionToCarve, int radius)
    {
        for (int y = positionToCarve.y; y <= chunkSize-1; y++)
        {
            for (int x = Mathf.Max(0, positionToCarve.x - radius); x <= Mathf.Min(positionToCarve.x + radius, chunkSize - 1); x++)
            {
                for (int z = Mathf.Max(0, positionToCarve.z - radius); z <= Mathf.Min(positionToCarve.z + radius, chunkSize - 1); z++)
                {
                    voxels[x, y, z] = 0;
                }
            }
        }
    }
}
