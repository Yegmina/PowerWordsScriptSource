using UnityEngine;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;

public class PresetManager : MonoBehaviour
{
    [System.Serializable]
    public class DictionaryData
    {
        public List<string> keys = new List<string>();
        public List<string> values = new List<string>();
    }

    public List<string> a1Words; // List of words in the native language
    public List<string> a1Translations; // Corresponding list of translations
    public List<string> a2Words;
    public List<string> a2Translations;
    public List<string> b1Words;
    public List<string> b1Translations;
    public List<string> b2Words;
    public List<string> b2Translations;
    public List<string> c1Words;
    public List<string> c1Translations;
    public List<string> c2Words;
    public List<string> c2Translations;

    public Button a1Button, a2Button, b1Button, b2Button, c1Button, c2Button;

    private string jsonFilePath;

    private void Start()
    {
        jsonFilePath = Path.Combine(Application.persistentDataPath, "dictionary.json");
		
        // Attach button listeners
        a1Button.onClick.AddListener(() => AddPresetWords(a1Words, a1Translations));
        a2Button.onClick.AddListener(() => AddPresetWords(a2Words, a2Translations));
        b1Button.onClick.AddListener(() => AddPresetWords(b1Words, b1Translations));
        b2Button.onClick.AddListener(() => AddPresetWords(b2Words, b2Translations));
        c1Button.onClick.AddListener(() => AddPresetWords(c1Words, c1Translations));
        c2Button.onClick.AddListener(() => AddPresetWords(c2Words, c2Translations));
    }

    private void AddPresetWords(List<string> words, List<string> translations)
    {
        if (words.Count != translations.Count)
        {
            Debug.LogError("Words and translations lists must be of the same length.");
            return;
        }

        Debug.Log($"Adding {words.Count} words with translations");

        // Load existing dictionary data from the JSON file
        DictionaryData dictionaryData = LoadDictionary();

        // Add each word and its translation to the dictionary data
        for (int i = 0; i < words.Count; i++)
        {
            dictionaryData.keys.Add(words[i]);
            dictionaryData.values.Add(translations[i]);
        }

        // Save the updated dictionary data back to the JSON file
        SaveDictionary(dictionaryData);

        Debug.Log("Preset words added and saved to JSON.");
    }

    private DictionaryData LoadDictionary()
    {
        if (File.Exists(jsonFilePath))
        {
            string json = File.ReadAllText(jsonFilePath);
            return JsonUtility.FromJson<DictionaryData>(json);
        }
        else
        {
            Debug.LogWarning("JSON file not found, creating new dictionary.");
            return new DictionaryData();
        }
    }

    private void SaveDictionary(DictionaryData dictionaryData)
    {
        string json = JsonUtility.ToJson(dictionaryData, true);
        File.WriteAllText(jsonFilePath, json);
        Debug.Log($"Dictionary saved to: {jsonFilePath}");
    }
}
