using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;

public class DictionaryManager : MonoBehaviour
{
	public GameObject additionalObject; // The object to activate when the slider is moved
    public GameObject wordTranslationPrefab; // Prefab with Word-Translation structure
    public Transform parentPanel; // Panel where word-translation objects will be added
	public string jsonFileName = "dictionary.json"; // File name for the dictionary
	public string jsonFilePath;    
	public Slider slider; // Reference to the UI slider for scrolling
    public RectTransform contentPanel; // The panel that will contain the word-translation objects
	public Button clearAllButton; // Button to clear the entire dictionary

    [System.Serializable]
    public class DictionaryData
    {
        public List<string> keys = new List<string>();
        public List<string> values = new List<string>();
    }

    private DictionaryData dictionaryData = new DictionaryData();

    private float totalContentHeight; // Total height of all word-translation pairs
    private float viewHeight; // Height of the viewable area

    private void Start()
    {
		jsonFilePath = Path.Combine(Application.persistentDataPath, jsonFileName);
        LoadDictionary();
		if (clearAllButton != null)
	{
		clearAllButton.onClick.AddListener(ClearAllDictionary);
	}

    }

    private void LoadDictionary()
    {
        // Load JSON data from file
        string json = LoadJsonFile(jsonFilePath);
        if (!string.IsNullOrEmpty(json))
        {
            dictionaryData = JsonUtility.FromJson<DictionaryData>(json);
            PopulateUI();
        }
    }

    private string LoadJsonFile(string filePath)
    {
        if (File.Exists(filePath))
        {
            return File.ReadAllText(filePath);
        }
        else
        {
            Debug.LogError("JSON file not found at " + filePath);
            return null;
        }
    }

    private void PopulateUI()
    {
        if (dictionaryData.keys.Count != dictionaryData.values.Count)
        {
            Debug.LogError("Keys and values count mismatch in the JSON file.");
            return;
        }

        float verticalOffset = 130f; // Vertical distance between prefabs
        Vector3 startingPosition = Vector3.zero; // Starting position for first prefab

        // Calculate the total content height and viewable area height
        totalContentHeight = dictionaryData.keys.Count * verticalOffset;
        viewHeight = contentPanel.rect.height;

        // Set the slider's max value based on the content height and view height
        if (totalContentHeight > viewHeight)
        {
            slider.maxValue = (totalContentHeight - viewHeight + 300)*1.05f;
            slider.onValueChanged.AddListener(OnSliderValueChanged);
        }
        else
        {
            slider.gameObject.SetActive(false); // Disable slider if content fits the view
        }

        for (int i = 0; i < dictionaryData.keys.Count; i++)
        {
            // Instantiate a new word-translation object from the prefab
            GameObject newEntry = Instantiate(wordTranslationPrefab, parentPanel);

            // Get the TextMeshProUGUI components for word and translation
            TextMeshProUGUI wordText = newEntry.transform.Find("word").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI translationText = newEntry.transform.Find("translation").GetComponent<TextMeshProUGUI>();

            if (wordText != null && translationText != null)
            {
                wordText.text = dictionaryData.keys[i];
                translationText.text = dictionaryData.values[i];
            }

            // Adjust position of the new entry
            RectTransform entryRectTransform = newEntry.GetComponent<RectTransform>();
            if (entryRectTransform != null)
            {
                float newYPosition = startingPosition.y - (i * verticalOffset);
                entryRectTransform.anchoredPosition = new Vector2(startingPosition.x, newYPosition);
            }

            // Add the delete button functionality
            Button deleteButton = newEntry.transform.Find("delete").GetComponent<Button>();
            if (deleteButton != null)
            {
                int index = i; // Store index for this particular entry
                deleteButton.onClick.AddListener(() => DeleteEntry(index));
            }
        }
    }

    // Method to handle slider movement
    private void OnSliderValueChanged(float value)
    {
        // Move the parentPanel up or down based on the slider value
        parentPanel.localPosition = new Vector3(parentPanel.localPosition.x, value, parentPanel.localPosition.z);
		// Activate the additional object
        if (additionalObject != null && !additionalObject.activeSelf)
        {
            additionalObject.SetActive(true); // Activate the object
            Debug.Log("Additional object activated!");
        }
    }

    // Method to handle the deletion of an entry
    private void DeleteEntry(int index)
    {
        Debug.Log($"Deleting entry at index {index}: {dictionaryData.keys[index]}");

        // Remove the entry from the in-memory dictionary
        dictionaryData.keys.RemoveAt(index);
        dictionaryData.values.RemoveAt(index);

        // Update the JSON file
        SaveDictionaryToFile();

        // Restart the scene to reflect the changes
        RestartScene();
    }

    // Save the dictionary to a JSON file after deletion
    private void SaveDictionaryToFile()
    {
        Debug.Log("Saving updated dictionary to JSON file...");
        string json = JsonUtility.ToJson(dictionaryData, true);
        File.WriteAllText(jsonFilePath, json);
        Debug.Log($"Dictionary saved to: {jsonFilePath}");
    }

    // Restart the scene to refresh the UI
    private void RestartScene()
    {
        // Reload the current scene
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
	
	private void ClearAllDictionary()
	{
    Debug.Log("Clearing all dictionary entries...");

    // Clear the dictionary data
    dictionaryData.keys.Clear();
    dictionaryData.values.Clear();

    // Save the cleared dictionary to the file
    SaveDictionaryToFile();

    // Restart the scene to refresh the UI
    RestartScene();
	}

}
