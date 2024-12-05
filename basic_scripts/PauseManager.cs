using UnityEngine;

public class PauseManager : MonoBehaviour
{
    [Header("Pause Settings")]
    public bool shouldStopGame = false;
    public bool shouldStopUI = false;
    public bool shouldStopAudio = false;
    public bool shouldStopAnimations = false;
    
    private bool isPaused = false;

    public void TogglePause()
    {
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        isPaused = true;

        if (shouldStopGame)
        {
            Time.timeScale = 0f;
        }

        if (shouldStopAudio)
        {
            AudioListener.pause = true;
        }

        if (shouldStopAnimations)
        {
            Animator[] animators = FindObjectsOfType<Animator>();
            foreach (Animator animator in animators)
            {
                animator.enabled = false;
            }
        }

        if (shouldStopUI)
        {
            Canvas[] canvases = FindObjectsOfType<Canvas>();
            foreach (Canvas canvas in canvases)
            {
                canvas.enabled = false;
            }
        }

        Debug.Log("Game Paused");
    }

    public void ResumeGame()
    {
        isPaused = false;

        if (shouldStopGame)
        {
            Time.timeScale = 1f;
        }

        if (shouldStopAudio)
        {
            AudioListener.pause = false;
        }

        if (shouldStopAnimations)
        {
            Animator[] animators = FindObjectsOfType<Animator>();
            foreach (Animator animator in animators)
            {
                animator.enabled = true;
            }
        }

        if (shouldStopUI)
        {
            Canvas[] canvases = FindObjectsOfType<Canvas>();
            foreach (Canvas canvas in canvases)
            {
                canvas.enabled = true;
            }
        }

        Debug.Log("Game Resumed");
    }
}
