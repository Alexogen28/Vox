using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnManager : MonoBehaviour
{
    [System.Serializable]
    public class LevelEnemyCollection
    {
        public LevelSO level;
        public List<EnemyDefinitionSO> enemies = new(); 
    }

    [SerializeField] private GameManager gameManager;

    [SerializeField] private GameObject enemiesParentObject;

    [Header("Generation Parameters")]
    [SerializeField, Tooltip("Number of tries to spawn enemies per chunk")]
    private int enemyDensity;

    [Header("Available Enemies")]
    [SerializeField] private List<LevelEnemyCollection> availableEnemies;

    public void SpawnEnemies(LevelSO level)
    {
        foreach (Transform child in enemiesParentObject.transform)
        {
            Destroy(child.gameObject);
        }

        List<EnemyDefinitionSO> availableEnemies = GetAllEnemiesForLevel(level);

        IReadOnlyDictionary<Vector3Int, VoxelChunk> worldChunks = gameManager.worldManager.GetAllWorldChunks;

        foreach (KeyValuePair<Vector3Int, VoxelChunk> chunkEntry in worldChunks)
        {
            VoxelChunk chunk = chunkEntry.Value;

            for (int i = 0; i < enemyDensity; i++)
            {
                System.Random rng = Seed.CreateRandom(
                    gameManager.worldManager.GetWorldSeed(),
                    AvailableSeedKeys.Enemy,
                    new Vector3Int(chunkEntry.Key.x * enemyDensity + i, chunkEntry.Key.y, chunkEntry.Key.z)
                );

                int x = rng.Next(0, chunk.chunkSize);
                int z = rng.Next(0, chunk.chunkSize);

                if (!chunk.TryGetBottomOYVoxel(x, z, out Vector3 spawnPosition))
                    continue;

                int enemyIndex = rng.Next(0, availableEnemies.Count);
                SpawnEnemy(availableEnemies[enemyIndex], spawnPosition);
            }
        }
    }

    private void SpawnEnemy(EnemyDefinitionSO definition, Vector3 spawnPosition)
    {
        GameObject enemyObject = Instantiate(definition.prefab, spawnPosition, Quaternion.identity, enemiesParentObject.transform);

        EnemyController controller = enemyObject.GetComponent<EnemyController>();
        if (controller == null)
        {
            Destroy(enemyObject);
            return;
        }

        controller.Initialise(definition);
    }

    private List<EnemyDefinitionSO> GetAllEnemiesForLevel(LevelSO level)
    {
        foreach (LevelEnemyCollection innerList in availableEnemies)
        {
            if (innerList.level == level)
                return innerList.enemies;
        }

        return availableEnemies[0].enemies;
    }
}
