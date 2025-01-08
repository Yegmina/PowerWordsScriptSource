using UnityEngine;
using UnityEngine.UI;

public class WizardHealthVisualSetup : MonoBehaviour
{
    private GameObject wizardHealthVisual;
    private PlayerStats playerStats;
    private HealthSystemForDummies healthSystem;
    private Image healthBarFillImage;

    void Start()
    {
        // Step 1: Find the wizard_mana object in the scene
        GameObject wizardMana = GameObject.Find("wizard_mana");
        if (wizardMana == null)
        {
            Debug.LogError("wizard_mana object not found in the scene!");
            return;
        }

        // Step 2: Clone the wizard_mana object
        wizardHealthVisual = Instantiate(wizardMana, wizardMana.transform.parent);
        if (wizardHealthVisual == null)
        {
            Debug.LogError("Failed to clone wizard_mana object.");
            return;
        }

        // Step 3: Rename the cloned object
        wizardHealthVisual.name = "wizard_health_visual";

        // Step 4: Move the cloned object slightly downward
        Vector3 newPosition = wizardMana.transform.position;
				//rn hardcoded, can be changed in future
        newPosition.y -= 0.4f; // Adjust the offset as needed
		

        wizardHealthVisual.transform.position = newPosition;

        // Step 5: Find the HealthBarCanvas under wizard_health_visual
        Transform healthBarCanvas = wizardHealthVisual.transform.Find("HealthBarCanvas");
        if (healthBarCanvas == null)
        {
            Debug.LogError("HealthBarCanvas not found under wizard_health_visual.");
            return;
        }

        // Step 6: Adjust the positions of Health Bar Fill, Health Bar Border, and Text
        Transform healthBarFillTransform = healthBarCanvas.Find("Health Bar Fill");
        Transform healthBarBorderTransform = healthBarCanvas.Find("Health Bar Border");
        Transform textTransform = healthBarCanvas.Find("Text");

        if (healthBarFillTransform == null || healthBarBorderTransform == null || textTransform == null)
        {
            Debug.LogError("One or more child objects (Health Bar Fill, Health Bar Border, Text) not found under HealthBarCanvas.");
            return;
        }

        // Move each UI element downward for the health bar
        float offset = -40f; // Adjust this value as needed for vertical spacing

        healthBarFillTransform.localPosition += new Vector3(0, offset, 0);
        healthBarBorderTransform.localPosition += new Vector3(0, offset, 0);
        textTransform.localPosition += new Vector3(0, offset, 0);

        // Step 7: Change the color of the Health Bar Fill to red
        healthBarFillImage = healthBarFillTransform.GetComponent<Image>();
        if (healthBarFillImage == null)
        {
            Debug.LogError("Health Bar Fill does not have an Image component.");
            return;
        }

        healthBarFillImage.color = new Color(1f, 0f, 0f); // Set color to red (FF0000)

        Debug.Log("wizard_health_visual setup completed successfully with adjusted positions.");

        // Step 8: Find the Wizard object and initialize components for synchronization
        GameObject wizard = GameObject.Find("Wizard");
        if (wizard == null)
        {
            Debug.LogError("Wizard object not found in the scene!");
            return;
        }

        playerStats = wizard.GetComponentInChildren<PlayerStats>();
        if (playerStats == null)
        {
            Debug.LogError("PlayerStats script not found in the Wizard object!");
            return;
        }

        healthSystem = wizardHealthVisual.GetComponent<HealthSystemForDummies>();
        if (healthSystem == null)
        {
            Debug.LogError("HealthSystemForDummies script not found on wizard_health_visual!");
            return;
        }
    }

    void Update()
    {
        // Continuously synchronize health values during runtime
        if (playerStats != null && healthSystem != null && healthBarFillImage != null)
        {
            healthSystem.CurrentHealth = playerStats.Health;
            healthSystem.MaximumHealth = playerStats.MaxHealth;

            // Calculate health percentage
            float healthPercentage = (float)playerStats.Health / playerStats.MaxHealth;

            // Update the fill amount of the Health Bar Fill image
            healthBarFillImage.fillAmount = healthPercentage;

            // Optional: Debug log for runtime monitoring
            Debug.Log($"Health Synced: {playerStats.Health}/{playerStats.MaxHealth} ({healthPercentage * 100:F2}%)");
        }
    }
}
