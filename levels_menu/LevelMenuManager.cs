using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Collections; 

public class LevelMenuManager : MonoBehaviour
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

    public Button[] levelButtons;  // Assign your level buttons in the Unity Inspector
    public Image[] levelImages;    // Assign corresponding level images in the Unity Inspector

    public Sprite unlockedSprite;
    public Sprite unlockedHoverSprite;
    public Sprite unlockedPressedSprite;
    public Sprite lockedSprite;
    public Sprite lockedHoverSprite;
    public Sprite lockedPressedSprite;

    public SceneTransition sceneTransition;
    public int totalLevels = 10; // Specify the total number of levels

    private string filePath;
    private Levels levelData;

    private void Start()
    {
        filePath = Path.Combine(Application.persistentDataPath, "levels.json");
        Debug.Log("File path for levels.json: " + filePath);
        LoadLevelData();
        UpdateLevelMenu();
    }

    private void LoadLevelData()
    {
        if (File.Exists(filePath))
        {
            Debug.Log("Found levels.json file. Loading data.");
            string dataAsJson = File.ReadAllText(filePath);
            levelData = JsonUtility.FromJson<Levels>(dataAsJson);

            // Ensure that the levelData has the correct number of levels
            if (levelData.levels.Count < totalLevels)
            {
                int levelsToAdd = totalLevels - levelData.levels.Count;
                Debug.Log("Adding " + levelsToAdd + " more locked levels to the list.");
                for (int i = 0; i < levelsToAdd; i++)
                {
                    levelData.levels.Add(new LevelData { unlocked = false });
                }
                SaveLevelData();
            }
        }
        else
        {
            Debug.Log("No levels.json file found. Creating default with Level 1 unlocked.");
            // Initialize level data with the total number of levels
            levelData = new Levels();
            levelData.levels = new List<LevelData>();

            for (int i = 0; i < totalLevels; i++)
            {
                // Unlock only the first level, others remain locked
                bool unlocked = (i == 0);
                levelData.levels.Add(new LevelData { unlocked = unlocked });
            }

            SaveLevelData();
        }
    }

    private void SaveLevelData()
    {
        Debug.Log("Saving level data to " + filePath);
        string json = JsonUtility.ToJson(levelData, true);
        File.WriteAllText(filePath, json);
        Debug.Log("Level data saved successfully.");
    }

    private void UpdateLevelMenu()
    {
        Debug.Log("Updating level menu UI.");

        for (int i = 0; i < levelButtons.Length; i++)
        {
            if (i < levelData.levels.Count && levelData.levels[i].unlocked)
            {
                Debug.Log("Level " + (i + 1) + " is unlocked.");

                // Unlocked level settings
                levelImages[i].sprite = unlockedSprite;
                levelButtons[i].interactable = true;

                // Set button sprite swap for unlocked levels
                SpriteState spriteState = new SpriteState();
                spriteState.highlightedSprite = unlockedHoverSprite;
                spriteState.pressedSprite = unlockedPressedSprite;
                levelButtons[i].spriteState = spriteState;

                // Add OnClick listener for scene transition
                int levelIndex = i + 1; // Assuming the level index starts from 1
                string sceneName = levelIndex + "_dungeon_battle"; // Dynamically create the scene name
                Debug.Log("Assigning scene transition to: " + sceneName);
                levelButtons[i].onClick.AddListener(() =>
                {
                    Debug.Log("Transitioning to scene: " + sceneName);
                   SceneManager.LoadScene(sceneName);
                });
            }
            else
            {
                Debug.Log("Level " + (i + 1) + " is locked.");

                // Locked level settings
                levelImages[i].sprite = lockedSprite;
                levelButtons[i].interactable = false;

                // Set button sprite swap for locked levels
                SpriteState spriteState = new SpriteState();
                spriteState.highlightedSprite = lockedHoverSprite;
                spriteState.pressedSprite = lockedPressedSprite;
                levelButtons[i].spriteState = spriteState;

                // No OnClick listener for locked levels
                levelButtons[i].onClick.RemoveAllListeners();
                Debug.Log("Removed all listeners for level " + (i + 1) + " since it's locked.");
            }
        }

        Debug.Log("Level menu UI update completed.");
    }

// Public method to unlock a level
public void Unlock(int level)
{
    Debug.Log("Unlocking level " + level);

    // Ensure the level index exists in the list
    if (level > 0 && level <= totalLevels)
    {
        if (!levelData.levels[level - 1].unlocked)
        {
            levelData.levels[level - 1].unlocked = true;
            SaveLevelData();
            UpdateLevelMenu(); // Update the UI to reflect changes
            Debug.Log("Level " + level + " unlocked and data saved.");

            // Start coroutine to load the new scene after a delay
            StartCoroutine(LoadSceneWithDelay("new_win_dungeon_battle", 1f));
        }
    }
    else
    {
        Debug.LogWarning("Level " + level + " is out of range and cannot be unlocked.");
    }
}

// Coroutine to load a new scene after a specified delay
private IEnumerator LoadSceneWithDelay(string sceneName, float delay)
{
    yield return new WaitForSeconds(delay);
    SceneManager.LoadScene(sceneName);
}


    // Public method to lock a level
    public void Lock(int level)
    {
        Debug.Log("Locking level " + level);

        // Ensure the level index exists in the list
        if (level > 0 && level <= totalLevels)
        {
            levelData.levels[level - 1].unlocked = false;
            SaveLevelData();
            UpdateLevelMenu(); // Update the UI to reflect changes
            Debug.Log("Level " + level + " locked and data saved.");
        }
        else
        {
            Debug.LogWarning("Level " + level + " is out of range and cannot be locked.");
        }
    }
}
