using UnityEngine;
using UnityEngine.UI;

public class BackgroundImage : MonoBehaviour
{
    // Переменная для хранения фонового изображения
    public Texture backgroundTexture;

    private void Start()
    {
        // Получаем компонент RawImage на Canvas
        RawImage rawImage = GetComponent<RawImage>();

        if (rawImage == null)
        {
            Debug.LogError("RawImage component not found on the Canvas!");
            return;
        }

        // Устанавливаем текстуру фона
        rawImage.texture = backgroundTexture;

        // Подгоняем изображение под весь экран
        RectTransform rectTransform = rawImage.GetComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
    }
}
