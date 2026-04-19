using UnityEngine;
using System;

public enum AvailableSeedKeys
{
    SurfaceTerrain,
    SurfaceBiome,
    SurfaceDetails,

    Cave,
    Dungeon,
    Item,
    Enemy
}

public static class Seed
{
    public static int Hash(int worldSeed, AvailableSeedKeys key)
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 31 + worldSeed;
            hash = hash * 31 + (int)key;
            return hash;
        }
    }

    public static int Hash(int worldSeed, AvailableSeedKeys key, Vector3Int coord)
    {
        unchecked
        {
            int hash = Hash(worldSeed, key);
            hash = hash * 31 + coord.x;
            hash = hash * 31 + coord.y;
            hash = hash * 31 + coord.z;
            return hash;                        
        }
    }

    public static System.Random CreateRandom(int worldSeed, AvailableSeedKeys key)
    {
        return new System.Random(Hash(worldSeed, key));
    }

    public static System.Random CreateRandom(int worldSeed, AvailableSeedKeys key, Vector3Int coords)
    {
        return new System.Random(Hash(worldSeed, key, coords));
    }

    public static float Range01(int worldSeed, AvailableSeedKeys key)
    {
        var rng = CreateRandom(worldSeed, key);
        return (float)rng.NextDouble();
    }

    public static float Range01(int worldSeed, AvailableSeedKeys key, Vector3Int coords)
    {
        var rng = CreateRandom(worldSeed, key, coords);
        return (float)rng.NextDouble();
    }

    public static Vector2 GetNoiseOffset(int worldSeed, AvailableSeedKeys key)
    {
        var rng = CreateRandom(worldSeed, key);
        return new Vector2(
            rng.Next(-100000, 100000),
            rng.Next(-100000, 100000)
        );
    }

    public static Vector2 GetNoiseOffset(int worldSeed, AvailableSeedKeys key, Vector3Int coord)
    {
        var rng = CreateRandom(worldSeed, key, coord);
        return new Vector2(
            rng.Next(-100000, 100000),
            rng.Next(-100000, 100000)
        );
    }
}
