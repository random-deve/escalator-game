using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class RandomObjectSpawner : MonoBehaviour
{
    [System.Serializable]
    public class ObjectChance
    {
        public GameObject prefab;
        [Range(0, 1)] public float chance;
    }

    public List<ObjectChance> prefabChances = new List<ObjectChance>();
    public LayerMask hitLayer;
    public Vector2 spawnAreaSize = new Vector2(10, 10);
    public int numberOfPrefabsToSpawn = 10;
    public Vector3 minRotationVector;
    public Vector3 maxRotationVector;
    public float maxRaycastDistance = 100f;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector3(spawnAreaSize.x, 0, spawnAreaSize.y));
    }

    public void LateAbortion()
    {
        foreach (Transform child in transform)
        {
            DestroyImmediate(child.gameObject);
        }
    }

    public void SpawnPrefabs()
    {
        for (int i = 0; i < numberOfPrefabsToSpawn; i++)
        {
            Debug.Log("spawning");
            Vector3 randomPosition = GetRandomPositionInArea();
            if (Physics.Raycast(randomPosition, Vector3.down, out RaycastHit hit, maxRaycastDistance, hitLayer))
            {
                GameObject prefabToSpawn = GetRandomPrefab();
                if (prefabToSpawn != null)
                {
                    GameObject spawnedObject = Instantiate(prefabToSpawn, hit.point, Quaternion.Euler(Random.Range(minRotationVector.x, maxRotationVector.x),
                        Random.Range(minRotationVector.y, maxRotationVector.y), Random.Range(minRotationVector.z, maxRotationVector.z)), transform);
                }
            }
        }

        SortChildrenByName();
    }

    private Vector3 GetRandomPositionInArea()
    {
        float x = Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2);
        float z = Random.Range(-spawnAreaSize.y / 2, spawnAreaSize.y / 2);
        return new Vector3(x, 100, z) + transform.position;
    }

    private GameObject GetRandomPrefab()
    {
        float totalChance = 0;
        foreach (var prefabChance in prefabChances)
        {
            totalChance += prefabChance.chance;
        }

        float randomValue = Random.Range(0, totalChance);
        float cumulativeChance = 0;

        foreach (var prefabChance in prefabChances)
        {
            cumulativeChance += prefabChance.chance;
            if (randomValue < cumulativeChance)
            {
                return prefabChance.prefab;
            }
        }

        return null;
    }

    private void SortChildrenByName()
    {
        List<Transform> children = new List<Transform>();
        foreach (Transform child in transform)
        {
            children.Add(child);
        }

        children.Sort((x, y) => string.Compare(x.name, y.name));

        foreach (Transform child in children)
        {
            child.SetSiblingIndex(children.IndexOf(child));
        }
    }
}
