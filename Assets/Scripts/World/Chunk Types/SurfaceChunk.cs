using UnityEngine;

public class SurfaceChunk : VoxelChunk 
{
    [Header("Perlin Noise Customization")]
    [SerializeField] public float noiseScale = 0.02f;
    [SerializeField] public float noiseHeightScale = 0.5f;
    private float persistence = 0.5f;
    private float lacunarity = 2f;

    protected override void GenerateChunkData()
    {
        Vector3 floatChunkCoord = new Vector3(
            (float) chunkCoord.x,
            (float) chunkCoord.y,
            (float) chunkCoord.z
        );

        Vector3 chunkVoxelWorldPosition = floatChunkCoord * chunkSize;
        Vector2 globalOffset = Seed.GetNoiseOffset(worldSeed, AvailableSeedKeys.SurfaceTerrain);

        int solidCount = 0;

        //Debug.Log($"Generating chunk data for {gameObject.name}");

        for (int x = 0; x < chunkSize; x++)
            for (int z = 0; z < chunkSize; z++)
            {
                float worldX = chunkVoxelWorldPosition.x + x;
                float worldZ = chunkVoxelWorldPosition.z + z;
                float height = 0f;
                float amplitude = noiseHeightScale * chunkSize;
                float frequency = noiseScale;
                if (chunkCoord.x == 0 || chunkCoord.x == worldSize.x-1 || chunkCoord.z == 0 || chunkCoord.z == worldSize.z-1 )
                {
                    float mountainNoiseScale = noiseHeightScale * 3.0f;
                    for (int octave = 0; octave < 4; octave++)
                    {
                        height += Mathf.PerlinNoise(
                            (worldX + globalOffset.x) * frequency,
                            (worldZ + globalOffset.y) * frequency
                        ) * amplitude * mountainNoiseScale;

                        amplitude *= persistence;
                        frequency *= lacunarity;
                    }
                }
                else
                {
                    for (int octave = 0; octave < 4; octave++)
                    {
                        height += Mathf.PerlinNoise(
                            (worldX + globalOffset.x) * frequency,
                            (worldZ + globalOffset.y) * frequency
                        ) * amplitude;

                        amplitude *= persistence;
                        frequency *= lacunarity;
                    }
                }

                //if (x == 0 && z == 0)
                //    Debug.Log($"{gameObject.name}: sample height = {height}");

                for (int y = 0; y < chunkSize; y++)
                {
                    float worldY = chunkVoxelWorldPosition.y + y;
                    voxels[x, y, z] = (worldY < height) ? (byte)1 : (byte)0;

                    if(voxels[x,y,z] != 0)
                        solidCount++;
                }
            }
        //Debug.Log($"{gameObject.name}: solid voxels written = {solidCount}");
    }
}
