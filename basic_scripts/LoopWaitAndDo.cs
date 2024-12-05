using System.Collections; // Required for IEnumerator
using UnityEngine;
using UnityEngine.Events;

public class LoopWaitAndDo : MonoBehaviour
{
    [Header("Settings")]
    public float timeToWait = 1.0f; // Time to wait between actions
    public int loopCount = 5; // Number of iterations

    [Header("Events")]
    public UnityEvent onActionComplete; // Event triggered on each action

    // Start the looping process
    public void StartLoop()
    {
        StartCoroutine(LoopAndDo());
    }

    // Coroutine to handle the waiting and looping actions
    private IEnumerator LoopAndDo()
    {
        for (int i = 0; i < loopCount; i++)
        {
            yield return new WaitForSeconds(timeToWait); // Wait for the specified time
            onActionComplete.Invoke(); // Invoke the event
        }
    }
}
