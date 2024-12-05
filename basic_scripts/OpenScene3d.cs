using UnityEngine;
using UnityEngine.SceneManagement;
/* Відкриття сцени при натисканні на 3д об'єкт на екрані */
public class OpenScene3d : MonoBehaviour
{
    public string sceneNameToLoad;

    void Update()
    {
        // Перевіряємо, чи було натискання на екран
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            // Створюємо промінь із камери в точку натискання на екрані
            Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            RaycastHit hit;

            // Перевіряємо перетин променя з об'єктами
            if (Physics.Raycast(ray, out hit))
            {
                // Перевіряємо, якщо було натискання на цей об'єкт
                if (hit.transform.gameObject == gameObject)
                {
                    // Завантажуємо вказану сцену
                    SceneManager.LoadScene(sceneNameToLoad);
                }
            }
        }
    }
}
