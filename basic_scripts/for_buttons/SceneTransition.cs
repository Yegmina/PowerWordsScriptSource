using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SceneTransition : MonoBehaviour
{
    public string sceneName;
    public bool enableFade = false; // По умолчанию False
    public float fadeDuration = 1f; // Длительность затемнения
    public CanvasGroup fadeCanvasGroup; // Ссылка на CanvasGroup для затемнения

    private Canvas parentCanvas; // Ссылка на Canvas родительского объекта

    private void Start()
    {
        // Находим Canvas родительского объекта
        if (fadeCanvasGroup != null)
        {
            parentCanvas = fadeCanvasGroup.GetComponentInParent<Canvas>();

            // Устанавливаем Canvas и CanvasGroup в неактивное состояние, чтобы не блокировать клики
            if (parentCanvas != null)
            {
                parentCanvas.gameObject.SetActive(false);
            }
            fadeCanvasGroup.alpha = 0f;
            fadeCanvasGroup.blocksRaycasts = false;
        }
    }

    public void TransitionToScene()
    {
        if (enableFade && fadeCanvasGroup != null)
        {
            if (parentCanvas != null)
            {
                parentCanvas.gameObject.SetActive(true); // Включаем Canvas, если он выключен
            }
            StartCoroutine(FadeOutAndSwitchScene(sceneName));
        }
        else
        {
            SceneManager.LoadScene(sceneName);
        }
    }

    private IEnumerator FadeOutAndSwitchScene(string scene)
    {
        // Включаем блокировку кликов при запуске затемнения
        fadeCanvasGroup.blocksRaycasts = true;

        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            yield return null;
        }

        // Переход на новую сцену
        SceneManager.LoadScene(scene);
    }

    public IEnumerator FadeIn()
    {
        // Плавно исчезаем, чтобы показывать содержимое новой сцены
        float timer = fadeDuration;
        while (timer > 0f)
        {
            timer -= Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            yield return null;
        }

        // Отключаем блокировку кликов после завершения затемнения
        fadeCanvasGroup.blocksRaycasts = false;

        // Отключаем Canvas, если он был включен для затемнения
        if (parentCanvas != null)
        {
            parentCanvas.gameObject.SetActive(false);
        }
    }
}
