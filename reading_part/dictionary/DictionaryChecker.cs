using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;


public class DictionaryChecker : MonoBehaviour
{
	public string jsonFileName = "dictionary.json"; // File name for the dictionary
	public string jsonFilePath;    
    private void Start()
    {
		jsonFilePath = Path.Combine(Application.persistentDataPath, jsonFileName);

        CheckIfDictionaryIsEmpty();
    }

    public void CheckIfDictionaryIsEmpty()
    {
        if (IsDictionaryEmpty())
        {
            // Call your custom method for handling an empty dictionary
            HandleEmptyDictionary();
        }
    }

    private bool IsDictionaryEmpty()
    {
        // Load JSON data from the file
        if (File.Exists(jsonFilePath))
        {
            string json = File.ReadAllText(jsonFilePath);
            SerializableDictionary dictionaryData = JsonUtility.FromJson<SerializableDictionary>(json);

            // Check if keys or values are empty
            return dictionaryData.keys.Count == 0 || dictionaryData.values.Count == 0;
        }
        else
        {
            Debug.LogError("JSON file not found at " + jsonFilePath);
            return true; // Consider the dictionary "empty" if the file doesn't exist
        }
    }

    // Public method to handle an empty dictionary (to be customized)
    public void HandleEmptyDictionary()
    {
        Debug.Log("Dictionary is empty. Implement your custom handling here.");
        SceneManager.LoadScene("DictionaryEmptyError");
    }

    // Serializable dictionary class to match JSON structure
    [System.Serializable]
    public class SerializableDictionary
    {
        public List<string> keys;
        public List<string> values;
    }
}
