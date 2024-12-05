using UnityEngine;

public class BookChanger : MonoBehaviour
{
    // Method to change the book name and save it to PlayerPrefs
    public void ChangeBook(string newBookName)
    {
        Debug.Log($"Changing book name to: {newBookName}");

        // Update the PlayerPrefs with the new book name
        PlayerPrefs.SetString("book_name", newBookName);
        PlayerPrefs.Save(); // Save the changes to PlayerPrefs

        Debug.Log($"Book name '{newBookName}' saved to PlayerPrefs.");
    }
}
