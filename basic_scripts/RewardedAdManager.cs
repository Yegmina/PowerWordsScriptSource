using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.Events;

public class RewardedAdManager : MonoBehaviour, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
{
    [Header("Ad Events")]
    public UnityEvent OnSuccessfulCompleteWatching;
    public UnityEvent OnCanceled;
    public UnityEvent OnError;

#if UNITY_IOS
    private string gameId = "5722475"; // Replace with your iOS Game ID
    private string adUnitId = "Rewarded_iOS";
#else
    private string gameId = "5722474"; // Replace with your Android Game ID
    private string adUnitId = "Rewarded_Android";
#endif

    private bool testMode = false;
    private bool adStarted = false;

    public bool rebirth = true;

    private void Start()
    {
        // Initialize Unity Ads
        Advertisement.Initialize(gameId, testMode, this);
    }

    public void ShowRewardedAd()
    {
        // Load the ad
        Advertisement.Load(adUnitId, this);
    }

    // Initialization callbacks
    public void OnInitializationComplete()
    {
        Debug.Log("Unity Ads initialization complete.");
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.LogError($"Unity Ads Initialization Failed: {error} - {message}");
        OnError?.Invoke();
    }

    // Load callbacks
    public void OnUnityAdsAdLoaded(string adUnitId)
    {
        Debug.Log("Ad Loaded: " + adUnitId);
        Advertisement.Show(adUnitId, this);
    }

    public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
    {
        Debug.LogError($"Error loading Ad Unit {adUnitId}: {error} - {message}");
        OnError?.Invoke();
    }

    // Show callbacks
    public void OnUnityAdsShowStart(string adUnitId)
    {
        adStarted = true;
        Debug.Log("Ad Started: " + adUnitId);
    }

    public void OnUnityAdsShowClick(string adUnitId)
    {
        Debug.Log("Ad Clicked: " + adUnitId);
    }

    public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
    {
        adStarted = false;
        if (showCompletionState == UnityAdsShowCompletionState.COMPLETED)
        {
            Debug.Log("Ad watched successfully. Giving reward.");
          //  if (rebirth)
//{
    // Retrieve objects and call methods
   // GameObject.Find("Wizard").GetComponent<WizardAnimationController>().PlayerHealed();
    //GameObject.Find("WizDemo1").GetComponent<WizDemo1>().Idle();
    
    // Enable all EnemyManager objects
   // var enemyManagers = GameObject.FindGameObjectsWithTag("EnemyManager");
    //foreach (var enemyManager in enemyManagers)
    //{
   //     enemyManager.GetComponent<EnemyManager>().enabled = true;
   // }

  //  GameObject.Find("CanvasAnimationDeath").SetActive(true);
  //  GameObject.Find("PlayerStats").GetComponent<PlayerStats>().Heal(1);
  //  GameObject.Find("PlayerStats").GetComponent<PlayerStats>().Heal(1);

   // GameObject.Find("wizard_mana").GetComponent<HealthSystemForDummies>().AddToCurrentHealth(750);
//}   
        

            OnSuccessfulCompleteWatching?.Invoke();

        }
        else if (showCompletionState == UnityAdsShowCompletionState.SKIPPED)
        {
            Debug.Log("Ad was canceled before completion.");
            OnCanceled?.Invoke();
        }
        else
        {
            Debug.LogError("There was an error showing the ad.");
            OnError?.Invoke();
        }
    }

    public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
    {
        Debug.LogError($"Error showing Ad Unit {adUnitId}: {error} - {message}");
        OnError?.Invoke();
    }
}
