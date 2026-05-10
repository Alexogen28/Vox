using UnityEngine;

public class VoxelRock : VoxelObject
{
    /*
        Materials to be listed as 
        1 -- rock material
    */
    protected override void GenerateVoxelData()
    {
        Vector3 center = new Vector3(
            dimensions.x / 2f,
            dimensions.y * 0.35f,
            dimensions.z / 2f
        );

        float radiusX = dimensions.x * 0.45f;
        float radiusY = dimensions.y * 0.30f;
        float radiusZ = dimensions.z * 0.45f;

        for (int x = 0; x < dimensions.x; x++)
        {
            for (int y = 0; y < dimensions.y; y++)
            {
                for (int z = 0; z < dimensions.z; z++)
                {
                    Vector3 localPosition = new Vector3(x, y, z);

                    float normalizedDistance =
                        Mathf.Pow((localPosition.x - center.x) / radiusX, 2f) +
                        Mathf.Pow((localPosition.y - center.y) / radiusY, 2f) +
                        Mathf.Pow((localPosition.z - center.z) / radiusZ, 2f);

                    float noise = Mathf.PerlinNoise(
                        (worldPosition.x + x) * 0.25f,
                        (worldPosition.z + z) * 0.25f
                    );

                    float surfaceVariation = Mathf.Lerp(0.85f, 1.15f, noise);

                    if (normalizedDistance <= surfaceVariation)
                    {
                        voxels[x, y, z] = 1;
                    }
                }
            }
        }
    }
}