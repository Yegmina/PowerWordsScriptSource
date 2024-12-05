using System.Collections; // Required for IEnumerator
using UnityEngine;
using UnityEngine.Events;

public class WaitAndDo : MonoBehaviour
{
    [Header("Events")]
    public UnityEvent onActionComplete;

    // Method to wait for a specified time and then perform the action
    public void StartWaitAndDo(float time) // Renamed method to avoid conflict with class name
    {
        StartCoroutine(DoAfterTime(time));
    }

    // Coroutine to handle the waiting and executing action
    private IEnumerator DoAfterTime(float time)
    {
        yield return new WaitForSeconds(time); // Wait for the specified time
        onActionComplete.Invoke(); // Invoke the event
    }
}
