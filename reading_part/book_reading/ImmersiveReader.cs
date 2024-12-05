using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;  // For TextMeshPro
using System.IO;
using UnityEngine.UI;  // For ScrollRect
using UnityEngine.Networking;  // For UnityWebRequest

public class ImmersiveReader : MonoBehaviour
{
    public TextMeshProUGUI bookTextUI; // Text UI element to display the book content
    public TextMeshProUGUI dictionaryUI; // UI element to display saved translations (optional)
    public TextMeshProUGUI translationUI; // UI element to display clicked word translation
    public ScrollRect scrollRect; // ScrollRect to control scrolling
    public RectTransform contentRectTransform; // To control content size within ScrollRect

    private string bookName; // Store book name retrieved from PlayerPrefs
    private string originalBookFilePath; // Path to original text (URL)
    private string translatedBookFilePath; // Path to translated text (URL)
    public string jsonFileName = "dictionary.json"; // File name for the dictionary
    public string dictionaryFilePath;

    private Dictionary<string, string> dictionaryList = new Dictionary<string, string>(); // Store clicked words and their translations
    private Dictionary<string, string> translationMap = new Dictionary<string, string>(); // Map original words to their translations
    private string[] originalWords; // Store the words from the original text
    private string selectedLanguage = "en"; // Hardcoded Finnish language


void Start()
{
    Debug.Log("Start: Loading book content and setting up UI.");
    dictionaryFilePath = Path.Combine(Application.persistentDataPath, jsonFileName);

    bookName = PlayerPrefs.GetString("book_name", "RomeoAndJuliet"); // Use "RomeoAndJuliet" as an example

    // Set URLs for original and translated book files on GitHub
    originalBookFilePath = $"https://raw.githubusercontent.com/Yegmina/books_for_game/main/books/{bookName}/original.txt";
    translatedBookFilePath = $"https://raw.githubusercontent.com/Yegmina/books_for_game/main/books/{bookName}/translated.txt";

    LoadDictionaryFromFile();
    StartCoroutine(LoadBook());
    DisplayBook();

    bookTextUI.richText = true;
    bookTextUI.ForceMeshUpdate();
}

IEnumerator LoadBook()
{
    Debug.Log("Loading book files from URLs...");

    // Load original book content from URL
    UnityWebRequest originalRequest = UnityWebRequest.Get(originalBookFilePath);
    yield return originalRequest.SendWebRequest();
    if (originalRequest.result != UnityWebRequest.Result.Success)
    {
        Debug.LogError($"Failed to load original file: {originalRequest.error}");
        yield break;
    }
    string originalBookContent = originalRequest.downloadHandler.text;
    originalWords = originalBookContent.Split(new[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);
    Debug.Log($"Original book content loaded: {originalWords.Length} words found.");

    // Load translated book content from URL
    UnityWebRequest translatedRequest = UnityWebRequest.Get(translatedBookFilePath);
    yield return translatedRequest.SendWebRequest();
    if (translatedRequest.result != UnityWebRequest.Result.Success)
    {
        Debug.LogError($"Failed to load translated file: {translatedRequest.error}");
        yield break;
    }
    string translatedBookContent = translatedRequest.downloadHandler.text;
    string[] translatedLines = translatedBookContent.Split(new[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
    Debug.Log($"Translated book content loaded: {translatedLines.Length} translations found.");

    translationMap.Clear();
    foreach (var line in translatedLines)
    {
        if (line.Contains($"{selectedLanguage}["))
        {
            int start = line.IndexOf('[') + 1;
            int end = line.IndexOf(']');
            string originalWord = line.Substring(start, end - start).ToLower();
            string translatedWord = line.Substring(line.IndexOf('=') + 2).Trim('"');

            if (!translationMap.ContainsKey(originalWord))
            {
                translationMap[originalWord] = translatedWord;
                Debug.Log($"Mapped '{originalWord}' to '{translatedWord}' for language '{selectedLanguage}'");
            }
        }
    }

    // Once both files are loaded, display the book content
    DisplayBook();
}

IEnumerator LoadTextFromURL(string url)
{
    UnityWebRequest request = UnityWebRequest.Get(url);
    yield return request.SendWebRequest();

    if (request.result != UnityWebRequest.Result.Success)
    {
        Debug.LogError($"Failed to load file from {url}: {request.error}");
        yield break;
    }
    yield return request.downloadHandler.text;
}

void DisplayBook()
{
    if (bookTextUI == null)
    {
        Debug.LogError("bookTextUI is not assigned!");
        return;
    }
    if (contentRectTransform == null)
    {
        Debug.LogError("contentRectTransform is not assigned!");
        return;
    }
    Debug.Log("Displaying the book content with clickable words and separate punctuation.");

    bookTextUI.text = "";

    // Regular expression to split text into words and punctuation
    string pattern = @"(\w+|\p{P})"; // Matches words (\w+) and punctuation (\p{P})

    bool previousWasWord = false; // Track if the previous part was a word

    foreach (var word in originalWords)
    {
        // If the word is exactly a "<br>", treat it as plain HTML tag
        if (word.Equals("<br>", System.StringComparison.OrdinalIgnoreCase))
        {
            bookTextUI.text += "<br>"; // Add line break without wrapping it as a link
            previousWasWord = false;
            continue;
        }

        // Split the word into parts using the regex
        var parts = System.Text.RegularExpressions.Regex.Matches(word, pattern);

        foreach (var part in parts)
        {
            string partText = part.ToString();

            // If it's an HTML-like tag (e.g., <br>), skip wrapping it in a link
            if (partText.StartsWith("<") && partText.EndsWith(">"))
            {
                bookTextUI.text += partText;
                previousWasWord = false;
            }
            // Check if the part is a word (matches \w+)
            else if (System.Text.RegularExpressions.Regex.IsMatch(partText, @"\w+")) // If it's a word
            {
                // Add space only if the previous part was a word (i.e., not punctuation)
                if (previousWasWord)
                {
                    bookTextUI.text += " ";
                }

                // Make the word clickable
                string linkTag = $"<link=\"{partText}\">{partText}</link>";
                bookTextUI.text += linkTag;

                previousWasWord = true; // Mark that we just added a word
            }
            else // If it's punctuation
            {
                // Add punctuation directly without adding a preceding space
                bookTextUI.text += partText;
                previousWasWord = false; // Mark that we added punctuation
                bookTextUI.text += " "; // Add space between words and punctuation
            }
        }
    }

    // Replace <link="br">br</link> with <br> for proper line breaks
    bookTextUI.text = bookTextUI.text.Replace("<link=\"br\">br</link>", "<br>");

    bookTextUI.ForceMeshUpdate(); // Ensure the text mesh is updated

    // Adjust content size based on the text length and enable scrolling
    AdjustContentSize();
    Debug.Log("Book content displayed with words clickable and punctuation as plain text.");
}





    // Adjust the content size within ScrollRect to handle scrolling
    void AdjustContentSize()
    {
        float textHeight = bookTextUI.preferredHeight;
        contentRectTransform.sizeDelta = new Vector2(contentRectTransform.sizeDelta.x, textHeight);
    }

    void Update()
    {
        // Check for word click
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(bookTextUI, Input.mousePosition, Camera.main);
        if (linkIndex != -1)
        {
            Debug.Log($"Link detected at index: {linkIndex}");

            if (Input.GetMouseButtonDown(0)) // On mouse click
            {
                TMP_LinkInfo linkInfo = bookTextUI.textInfo.linkInfo[linkIndex];
                string clickedWord = linkInfo.GetLinkID();

                Debug.Log($"Word clicked: '{clickedWord}'");

                // Handle the clicked word
                OnWordClick(clickedWord);
            }
        }
        else if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Click detected, but no word was clicked.");
        }
    }

    // Handle the word click
    void OnWordClick(string word)
    {
        Debug.Log($"OnWordClick: Processing word '{word}'");

        // Clean up the word (remove punctuation and convert to lowercase)
        string cleanedWord = word.ToLower().Trim(new char[] { '.', ',', '!', '?', ';' });

        // If the word exists in the translation map
        if (translationMap.ContainsKey(cleanedWord))
        {
            string translation = translationMap[cleanedWord];
            Debug.Log($"Translation found for '{cleanedWord}': '{translation}'");

            // Show the translation in the translationUI
            translationUI.text = translation;

            // Add word and translation to dictionary list if not already added
            if (!dictionaryList.ContainsKey(cleanedWord))
            {
                dictionaryList.Add(cleanedWord, translation);
                Debug.Log($"Word '{cleanedWord}' added to the dictionary.");
                UpdateDictionaryUI();
                SaveDictionaryToFile(); // Save dictionary after each word click
            }
            else
            {
                Debug.Log($"Word '{cleanedWord}' is already in the dictionary.");
            }
        }
        else
        {
            Debug.LogWarning($"No translation found for clicked word: '{cleanedWord}'");
            // here I need to process with translation api
            // Example usage
            StartCoroutine(GetTranslation(cleanedWord, (translation) =>
            {
                if (translation != null)
                {
                    Debug.Log("Translation: " + translation);
                    Debug.Log($"Translation via API for '{cleanedWord}': '{translation}'");

                    // Show the translation in the translationUI
                    translationUI.text = translation;

                    // Add word and translation to dictionary list if not already added
                    if (!dictionaryList.ContainsKey(cleanedWord))
                    {
                        dictionaryList.Add(cleanedWord, translation);
                        Debug.Log($"Word '{cleanedWord}' added to the dictionary.");
                        UpdateDictionaryUI();
                        SaveDictionaryToFile(); // Save dictionary after each word click
                    }
                    else
                    {
                        Debug.Log($"Word '{cleanedWord}' is already in the dictionary.");
                    }

                }
                else
                {
                    Debug.LogError("Failed to get translation.");
                }
            }));
        }
    }

    // Update the UI to show saved translations
    void UpdateDictionaryUI()
    {
        Debug.Log("Updating dictionary UI with new entries.");

        dictionaryUI.text = "Dictionary List:\n";

        foreach (var entry in dictionaryList)
        {
            dictionaryUI.text += $"{entry.Key} -> {entry.Value}\n";
            Debug.Log($"Dictionary entry: {entry.Key} -> {entry.Value}");
        }

        Debug.Log("Dictionary UI updated.");
    }

    // Save the dictionary to a JSON file
    void SaveDictionaryToFile()
    {
        Debug.Log("Saving dictionary to JSON file...");

        // Convert dictionaryList to JSON format
        string json = JsonUtility.ToJson(new SerializableDictionary(dictionaryList), true);

        // Save the JSON to a file
        File.WriteAllText(dictionaryFilePath, json);

        Debug.Log($"Dictionary saved to: {dictionaryFilePath}");
    }

    // Load the dictionary from a JSON file if it exists
    void LoadDictionaryFromFile()
    {
        if (File.Exists(dictionaryFilePath))
        {
            Debug.Log("Loading existing dictionary from JSON file...");

            string json = File.ReadAllText(dictionaryFilePath);
            SerializableDictionary loadedDictionary = JsonUtility.FromJson<SerializableDictionary>(json);

            // Merge loaded dictionary with the current dictionary, ensuring no duplicates
            for (int i = 0; i < loadedDictionary.keys.Count; i++)
            {
                string key = loadedDictionary.keys[i];
                string value = loadedDictionary.values[i];
                if (!dictionaryList.ContainsKey(key))
                {
                    dictionaryList.Add(key, value);
                    Debug.Log($"Loaded dictionary entry: {key} -> {value}");
                }
            }

            UpdateDictionaryUI(); // Update UI with loaded entries
        }
        else
        {
            Debug.Log("No existing dictionary file found. Starting fresh.");
        }
    }

    // Class to make the dictionary serializable to JSON
    [System.Serializable]
    public class SerializableDictionary
    {
        public List<string> keys;
        public List<string> values;

        public SerializableDictionary(Dictionary<string, string> dictionary)
        {
            keys = new List<string>(dictionary.Keys);
            values = new List<string>(dictionary.Values);
        }
    }

    IEnumerator GetTranslation(string word, System.Action<string> callback)
    {
        // Create the JSON payload
        string jsonData = $"{{\"platform\":\"api\",\"data\":\"{word}\",\"from\":\"fi\",\"to\":\"en\"}}";

        // Create the UnityWebRequest
        using (UnityWebRequest request = new UnityWebRequest("https://api-b2b.backenster.com/b1/api/v3/translate", "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("accept", "application/json");
            request.SetRequestHeader("Authorization", ""); // Replace with your actual API key

            // Send the request and wait for the response
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                // Parse the response JSON
                string jsonResponse = request.downloadHandler.text;
                Debug.Log("Raw Response: " + jsonResponse);

                try
                {
                    TranslationResponse response = JsonUtility.FromJson<TranslationResponse>(jsonResponse);

                    if (response != null)
                    {
                        // Translation successful
                        callback(response.result);
                    }
                    else
                    {
                        Debug.LogError("Failed to parse response.");
                        callback(null);
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError("JSON Parsing Error: " + e.Message);
                    callback(null);
                }
            }
            else
            {
                Debug.LogError($"Request Error: {request.error}");
                Debug.LogError("Details: " + request.downloadHandler.text);
                callback(null);
            }
        }
    }


    // Data model for the API response
    [System.Serializable]
    public class TranslationResponse
    {
        public string err;    // Error field
        public string result; // Translation result
    }
}
