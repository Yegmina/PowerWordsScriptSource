using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;

public class LevelCompletionManager : MonoBehaviour
{
    [System.Serializable]
    public class LevelData
    {
        public bool unlocked;
    }

    [System.Serializable]
    public class Levels
    {
        public List<LevelData> levels;
    }

    public TMP_Text previousLevelText; // Text to display the last unlocked level
    public Button continueButton; // Button to continue

    private string filePath;
    private Levels levelData;

    private void Start()
    {
        filePath = Path.Combine(Application.persistentDataPath, "levels.json");
        LoadLevelData();
        DisplayPreviousLevel();

        continueButton.onClick.AddListener(OnContinue);
    }

    private void LoadLevelData()
    {
        if (File.Exists(filePath))
        {
            string dataAsJson = File.ReadAllText(filePath);
            levelData = JsonUtility.FromJson<Levels>(dataAsJson);
        }
        else
        {
            Debug.LogError("Level data file not found!");
        }
    }

    private void DisplayPreviousLevel()
    {
        if (levelData == null || levelData.levels.Count == 0)
        {
            previousLevelText.text = "No level data found.";
            return;
        }

        // Find the last unlocked level
        int lastUnlockedLevel = -1;
        for (int i = 0; i < levelData.levels.Count; i++)
        {
            if (levelData.levels[i].unlocked)
            {
                lastUnlockedLevel = i + 1; // Levels are 1-based for display purposes
            }
        }

        // Calculate and display the previous level
        int previousLevel = lastUnlockedLevel - 1;
        if (previousLevel > 0)
        {
            previousLevelText.text =  (previousLevel+1).ToString();
        }
        else
        {
            previousLevelText.text = "";
        }
    }

    private void OnContinue()
    {
        // Logic to continue to the next scene or main menu
    }
}
