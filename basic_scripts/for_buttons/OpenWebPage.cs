using UnityEngine;

public class OpenWebPage : MonoBehaviour
{
    public string urlToOpen = "https://yegmina.github.io/physics/"; // Change this to the desired URL

    public void OpenWebPageURL()
    {
        Application.OpenURL(urlToOpen);
    }
}
