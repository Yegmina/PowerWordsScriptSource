using System.Collections.Generic;
using UnityEngine;
using TMPro; // For TextMeshPro
using UnityEngine.UI; // For buttons
using UnityEngine.Events; // For global eventsusing UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using System.Linq;

public class WordQuiz : MonoBehaviour
{
    public TextMeshProUGUI wordTextUI; // UI element to display the random word
    public Button[] answerButtons; // Array of buttons for displaying translations
    private Dictionary<string, string> dictionaryList = new Dictionary<string, string>(); // Store words and translations
    public Canvas quizCanvas; // Canvas to disable after answer
	private bool isFiToEn; // True if Finnish-to-English, false if English-to-Finnish

    private string correctTranslation; // Correct answer for the current quiz

    // Global events for correct and wrong answers
    public UnityEvent OnCorrectAnswer;
    public UnityEvent OnWrongAnswer;

    void Start()
    {
        // Load dictionary from your existing JSON file
        LoadDictionaryFromFile();
    }

 // Method to generate the quiz question
public void GenerateQuiz()
{
    // Randomize quiz mode
    isFiToEn = Random.value < 0.5f;
    string randomWord; // Word to display in the question

    // Check if the dictionary is empty
    if (dictionaryList.Count == 0)
    {
        SceneManager.LoadScene("DictionaryEmptyError");
        Debug.LogError("Dictionary is empty. Cannot generate quiz.");
        return;
    }

    // Determine the mode and select the random word and correct answer
    if (isFiToEn)
    {
        // Finnish to English
        List<string> words = new List<string>(dictionaryList.Keys); // Finnish words
        randomWord = words[Random.Range(0, words.Count)]; // Random Finnish word
        correctTranslation = dictionaryList[randomWord]; // Corresponding English translation
    }
    else
    {
        // English to Finnish
        List<string> words = new List<string>(dictionaryList.Values); // English words
        randomWord = words[Random.Range(0, words.Count)]; // Random English word

        // Find the Finnish translation (key) corresponding to the random English word
        foreach (KeyValuePair<string, string> entry in dictionaryList)
        {
            if (entry.Value == randomWord)
            {
                correctTranslation = entry.Key; // Correct Finnish translation
                break; // Exit the loop as soon as the correct translation is found
            }
        }
    }

    // Display the random word in the UI
    wordTextUI.text = randomWord;

    // Generate options for answers
    List<string> optionsPool = isFiToEn
        ? new List<string>(dictionaryList.Values) // Pool of English words for Finnish-to-English mode
        : new List<string>(dictionaryList.Keys); // Pool of Finnish words for English-to-Finnish mode

    // Remove the correct answer from the pool
    if (isFiToEn)
    {
        optionsPool.Remove(correctTranslation);
    }
    else
    {
        optionsPool.Remove(randomWord);
    }

    // Ensure there are enough options for incorrect answers
    if (optionsPool.Count < 2)
    {
        Debug.LogError("Not enough options to generate incorrect answers.");
        return;
    }

	// Select two unique incorrect answers
	HashSet<string> incorrectAnswers = new HashSet<string>();

	while (incorrectAnswers.Count < 2)
	{
		string incorrect = optionsPool[Random.Range(0, optionsPool.Count)];
		if (incorrect != correctTranslation) // Ensure the incorrect answer isn't the same as the correct answer
		{
			incorrectAnswers.Add(incorrect); // Add to the set to ensure uniqueness
		}
	}

	// Convert incorrect answers to a list
	List<string> incorrectList = new List<string>(incorrectAnswers);
	string incorrect1 = incorrectList[0];
	string incorrect2 = incorrectList[1];


    // Log details for debugging
    if (isFiToEn)
    {
        Debug.Log($"Mode: FiToEn, Random: {randomWord}, Correct: {correctTranslation}, Incorrect: {incorrect1}, {incorrect2}");
    }
    else
    {
        Debug.Log($"Mode: EnToFi, Random: {randomWord}, Correct: {correctTranslation}, Incorrect: {incorrect1}, {incorrect2}");
    }

    // Create and shuffle the options
    string[] options = { correctTranslation, incorrect1, incorrect2 };
    ShuffleArray(options);

    // Assign options to buttons
    for (int i = 0; i < answerButtons.Length; i++)
    {
        answerButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = options[i];

        // Remove existing listeners to avoid multiple subscriptions
        answerButtons[i].onClick.RemoveAllListeners();

        // Assign the appropriate click event (check if the clicked answer is correct)
        if (options[i] == correctTranslation)
        {
            answerButtons[i].onClick.AddListener(CorrectAnswer);
        }
        else
        {
            answerButtons[i].onClick.AddListener(WrongAnswer);
        }
    }
}




    // Method to shuffle the options randomly
	void ShuffleArray(string[] array)
	{
		for (int i = array.Length - 1; i > 0; i--)
		{
			int j = Random.Range(0, i + 1);
			(array[i], array[j]) = (array[j], array[i]); // Swap elements
		}
	}


    // Correct answer event
    public void CorrectAnswer()
    {
        Debug.Log("Correct Answer!");
        // Call the correct_answer method or perform other actions
        correct_answer();

        // Trigger global event for correct answer
        OnCorrectAnswer.Invoke();
    }

    // Wrong answer event
    public void WrongAnswer()
    {
        Debug.Log("Wrong Answer!");
        // Call the wrong_answer method or perform other actions
        wrong_answer();

        // Trigger global event for wrong answer
        OnWrongAnswer.Invoke();
    }

    // Placeholder methods for correct and wrong answers
    public void correct_answer()
    {
        quizCanvas.enabled = false;
    }

    public void wrong_answer()
    {
        quizCanvas.enabled = false;
    }

    // Method to start the quiz (call this from other scripts when needed)
    public void start_quiz()
    {
        GenerateQuiz();
    }

	void LoadDictionaryFromFile()
	{
		string jsonFileName = "dictionary.json";
		string dictionaryFilePath = Path.Combine(Application.persistentDataPath, jsonFileName);

		if (File.Exists(dictionaryFilePath))
		{
			string json = File.ReadAllText(dictionaryFilePath);
			SerializableDictionary loadedDictionary = JsonUtility.FromJson<SerializableDictionary>(json);

			// Ensure keys and values are aligned
			if (loadedDictionary.keys.Count != loadedDictionary.values.Count)
			{
				Debug.LogError("Mismatch in keys and values in the JSON file.");
				return;
			}

			// Merge loaded dictionary with the current dictionary, ensuring no duplicates
			for (int i = 0; i < loadedDictionary.keys.Count; i++)
			{
				string key = loadedDictionary.keys[i];
				string value = loadedDictionary.values[i];
				if (!dictionaryList.ContainsKey(key))
				{
					dictionaryList.Add(key, value);
				}
			}
		}
		else
		{
			Debug.LogError("Dictionary JSON file not found.");
		}
	}


    // Serializable dictionary class (already in your other script)
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
}
