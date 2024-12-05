using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ButtonReload : MonoBehaviour
{
    [Header("Button Setup")]
    public Button skillButton;  // Reference to the button
    public CanvasGroup buttonCanvasGroup;  // For controlling transparency
    public float blinkDuration = 0.1f; // Blink duration at the end of reload

    [Header("Reload Settings")]
    public bool startReloadOnAwake = false;  // Should the reload process start on awake?
    public float initialReloadTime = 5f; // Time for the initial reload if starting on awake

    private bool isReloading = false;  // Check if reloading is in progress

    // Method to start the reload process
    public void StartReload(float reloadTime)
    {
        if (isReloading) return; // Prevent starting another reload if already in progress

        StartCoroutine(ReloadButton(reloadTime));
    }

    // Coroutine that handles the reloading process
    private IEnumerator ReloadButton(float reloadTime)
    {
        isReloading = true;

        // Disable the button and make it almost fully transparent
        skillButton.interactable = false;
        buttonCanvasGroup.alpha = 0.2f;  // Set transparency to 20%

        float elapsedTime = 0f;
        while (elapsedTime < reloadTime)
        {
            elapsedTime += Time.deltaTime;
            // Gradually restore transparency over the reload time
            buttonCanvasGroup.alpha = Mathf.Lerp(0.2f, 1f, elapsedTime / reloadTime);
            yield return null;
        }

        // Once the time is over, blink the button to signal reload completion
        yield return BlinkButton();

        // Make the button fully opaque and clickable again
        buttonCanvasGroup.alpha = 1f;
        skillButton.interactable = true;

        isReloading = false;
    }

    // Method to blink the button when the reload finishes
    private IEnumerator BlinkButton()
    {
        float halfBlinkDuration = blinkDuration / 2f;

        // Blink off
        buttonCanvasGroup.alpha = 0f;
        yield return new WaitForSeconds(halfBlinkDuration);

        // Blink on
        buttonCanvasGroup.alpha = 1f;
        yield return new WaitForSeconds(halfBlinkDuration);
    }

    // Called when the script is initialized
    private void Awake()
    {
        if (startReloadOnAwake)
        {
            // If the reload should start on awake, trigger it with the specified initial reload time
            StartReload(initialReloadTime);
        }
    }
}
