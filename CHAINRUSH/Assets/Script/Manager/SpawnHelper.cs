using UnityEngine;

public static class SpawnHelper
{
    public static bool TryGetValidSpawnPoint(
        Terrain terrain,
        LayerMask overlapMask,
        float checkRadius,
        out Vector3 spawnPosition,
        int maxAttempts = 10)
    {
        for(int i = 0; i < maxAttempts; ++i)
        {
            Vector3 candidate = GetRandomPointOnTerrain(terrain);
            if (!Physics.CheckSphere(candidate, checkRadius, overlapMask))
            {
                spawnPosition = candidate;
                return true;
            }
        }

        spawnPosition = Vector3.zero;
        return false;
    }

    private static Vector3 GetRandomPointOnTerrain(Terrain terrain)
    {
        float terrainWidth = terrain.terrainData.size.x;
        float terrainLength = terrain.terrainData.size.z;

        float x = Random.Range(0f, terrainWidth);
        float z = Random.Range(0f, terrainLength);
        float y = terrain.SampleHeight(new Vector3(x, 0f, z)) + terrain.transform.position.y;

        return new Vector3(
            x + terrain.transform.position.x,
            y,
            z + terrain.transform.position.z
        );
    }

}
