using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RandomObjectSpawner))]
public class RandomPrefabSpawnerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        RandomObjectSpawner spawner = (RandomObjectSpawner)target;
        if (GUILayout.Button("Spawn Prefabs"))
        {
            spawner.SpawnPrefabs();
        }
        if (GUILayout.Button("Clear Prefabs"))
        {
            spawner.LateAbortion();
        }
    }
}
