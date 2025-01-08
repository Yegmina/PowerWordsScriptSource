using UnityEngine;

public class UIVisualPrefabManager : MonoBehaviour
{
    void Start()
    {
        Debug.Log("Starting prefab assignment process...");
        AddPrefabToUIObjects("Fireball", "visual_fireball");
        AddPrefabToUIObjects("Snowball", "visual_snowball");
        AddPrefabToUIObjects("Ultimate", "visual_ultimate");
        AddPrefabToUIObjects("MoreHeath", "visual_add_health");
        AddPrefabToUIObjects("healing", "visual_healing");
        AddPrefabToUIObjects("Poison", "visual_poison");
        AddPrefabToUIObjects("Healing-antiPoison (1)", "visual_long_healing");
        Debug.Log("Prefab assignment process completed.");
    }

    void AddPrefabToUIObjects(string objectName, string prefabName)
    {
        Debug.Log($"Searching for objects with name: {objectName}");

        // Find all objects in the scene with the given name
        GameObject[] objects = GameObject.FindObjectsOfType<GameObject>();
        bool foundObject = false;

        foreach (GameObject obj in objects)
        {
            if (obj.name == objectName)
            {
                foundObject = true;
                Debug.Log($"Found object with name: {obj.name}");

                // Load the prefab by name
                GameObject prefab = Resources.Load<GameObject>(prefabName);
                if (prefab == null)
                {
                    Debug.LogError($"Prefab with name '{prefabName}' not found in Resources folder.");
                    continue;
                }

                Debug.Log($"Instantiating prefab '{prefabName}' for object '{obj.name}'...");

                // Instantiate the prefab and make it a child of the found object
                GameObject instantiatedPrefab = Instantiate(prefab, obj.transform);

                // Optional: Reset the prefab's local position/rotation/scale
                instantiatedPrefab.transform.localPosition = Vector3.zero;
                instantiatedPrefab.transform.localRotation = Quaternion.identity;
                instantiatedPrefab.transform.localScale = Vector3.one;

                Debug.Log($"Prefab '{prefabName}' successfully instantiated for object '{obj.name}'.");
            }
        }

        if (!foundObject)
        {
            Debug.LogWarning($"No objects found with name: {objectName}");
        }
    }
}
