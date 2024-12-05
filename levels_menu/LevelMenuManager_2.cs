using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class LevelMenuManager_2 : MonoBehaviour
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

    public Button[] levelButtons;
    public Image[] levelImages;

    public Sprite lastUnlockedSprite;
    public Sprite unlockedSprite;
    public Sprite lockedSprite;

    public GameObject lockPrefab;
    public GameObject textPrefab;

    public int totalLevels = 10;

    private string filePath;
    private Levels levelData;

    private void Start()
    {
        filePath = Path.Combine(Application.persistentDataPath, "levels.json");
        LoadLevelData();
        UpdateLevelMenu();
    }

    private void LoadLevelData()
    {
        if (File.Exists(filePath))
        {
            string dataAsJson = File.ReadAllText(filePath);
            levelData = JsonUtility.FromJson<Levels>(dataAsJson);

            if (levelData.levels.Count < totalLevels)
            {
                for (int i = levelData.levels.Count; i < totalLevels; i++)
                {
                    levelData.levels.Add(new LevelData { unlocked = false });
                }
                SaveLevelData();
            }
        }
        else
        {
            levelData = new Levels();
            levelData.levels = new List<LevelData>();
            for (int i = 0; i < totalLevels; i++)
            {
                levelData.levels.Add(new LevelData { unlocked = (i == 0) });
            }
            SaveLevelData();
        }
    }

    private void SaveLevelData()
    {
        string json = JsonUtility.ToJson(levelData, true);
        File.WriteAllText(filePath, json);
    }

    private void UpdateLevelMenu()
    {
        int lastUnlockedIndex = -1;

        for (int i = 0; i < levelData.levels.Count; i++)
        {
            if (levelData.levels[i].unlocked)
            {
                lastUnlockedIndex = i;
            }
        }

        for (int i = 0; i < levelButtons.Length; i++)
        {
            // Remove existing children (lock or text) from button before adding new ones
            foreach (Transform child in levelButtons[i].transform)
            {
                Destroy(child.gameObject);
            }

            if (i < levelData.levels.Count && levelData.levels[i].unlocked)
            {
                levelImages[i].sprite = (i == lastUnlockedIndex) ? lastUnlockedSprite : unlockedSprite;
                levelButtons[i].interactable = true;

                // Create a Text Mesh Pro object to show the level number
                GameObject textObj = Instantiate(textPrefab, levelButtons[i].transform);
                textObj.GetComponent<TMP_Text>().text = (i + 1).ToString();  // Update to use TMP_Text

                int levelIndex = i + 1;
                string sceneName = levelIndex + "_dungeon_battle";
                levelButtons[i].onClick.RemoveAllListeners();
                levelButtons[i].onClick.AddListener(() =>
                {
                    SceneManager.LoadScene(sceneName);
                });
            }
            else
            {
                levelImages[i].sprite = lockedSprite;
                levelButtons[i].interactable = false;

                // Create a lock object for locked levels
                Instantiate(lockPrefab, levelButtons[i].transform);
                levelButtons[i].onClick.RemoveAllListeners();
            }
        }
    }

    public void Unlock(int level)
    {
        if (level > 0 && level <= totalLevels)
        {
            levelData.levels[level - 1].unlocked = true;
            SaveLevelData();
            UpdateLevelMenu();
        }
    }

    public void Lock(int level)
    {
        if (level > 0 && level <= totalLevels)
        {
            levelData.levels[level - 1].unlocked = false;
            SaveLevelData();
            UpdateLevelMenu();
        }
    }
}
