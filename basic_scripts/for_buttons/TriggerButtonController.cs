using UnityEngine;
using UnityEngine.UI;

public class TriggerButtonController : MonoBehaviour
{
    public GameObject buttonPrefab;
    public Transform playerTransform;
    public float activationDistance = 2.0f;
    public Vector3 buttonOffset = new Vector3(0, 1, 0);
    public int boo = 0;

    private GameObject activeButton;
    public Canvas canvas;
    private bool isButtonClicked = false;
    private float timer = 0f;
    private float timerDuration = 3f;

    private void Start()
    {
        
    }

    private void Update()
    {
        float distance = Vector3.Distance(playerTransform.position, transform.position);

        if (distance <= activationDistance)
        {
            ShowButton();
        }
        else
        {
            HideButton();
        }

        if (isButtonClicked)
        {
            timer += Time.deltaTime;
            if (timer >= timerDuration)
            {
                HideButton();
                isButtonClicked = false;
                timer = 0f;
            }
        }
    }

    private void ShowButton()
    {
        if (boo == 0)
        {
            if (activeButton == null)
            {
                activeButton = Instantiate(buttonPrefab, canvas.transform);
                activeButton.GetComponent<Button>().onClick.AddListener(OnButtonClick);
				//activeButton=Instantiate(canvas.transform)<position>.onClick.prefab;
            }

            Vector3 newPosition = playerTransform.position + buttonOffset;
            activeButton.transform.position = newPosition;
        }
    }

    private void HideButton()
    {
        if (activeButton != null)
        {
            Destroy(activeButton);
            activeButton = null;
        }
    }

    private void OnButtonClick()
    {
        Debug.Log("Button clicked!");
        boo = 1;
        isButtonClicked = true;
        // Ничего не нужно делать здесь, таймер будет обрабатываться в Update()
    }
}
