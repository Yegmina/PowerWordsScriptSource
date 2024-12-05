using System.Collections; // Ensure this is included for IEnumerator
using UnityEngine;
using UnityEngine.Events;

public class EnemyManager : MonoBehaviour
{
    public HealthSystemForDummies healthSystem;
    public Animator enemyAnimator;
    public Animator wizardAnimator; // Reference to the wizard animator
    public GameObject enemy; // Reference to the enemy GameObject
    public HealthBarHUDTester playerHealth; // Reference to the player's health system

    public float rotationSpeed = 100f; // Speed at which the enemy rotates
    public bool isDeadRotationStarted = false; // Track whether rotation has started
    public float rotationTarget = -75f; // Target rotation in Z-axis
    public bool enemyHidden = false; // Track if the enemy has been hidden
    public float hideDelay = 1f; // Delay before hiding the enemy after death
    public float hideTimer = 0f; // Timer to track delay for hiding
	
	public float AttackStrength=0.25f;
	public float CritStrength=1.5f;


    private float attackCooldown = 3f; // Time between attacks
    private float attackTimer = 0f; // Timer to track when to attack
    private bool isAttacking = false; // Check if enemy is currently attacking

    // Particle systems for damage and heal effects
    public ParticleSystem damageParticleSystem; // Particle system for damage effect
    public ParticleSystem healParticleSystem; // Particle system for healing effect

    public delegate void WinEvent();
    public static event WinEvent OnWin;
	
	public string damage_animation_name="damage";
	public string attack_animation_name_1="attack";
	public string attack_animation_name_2="attack02";
	public string death_animation_name="damage";
	
	public UnityEvent OnEnemyAttacking; // Event for when the enemy attacks
public UnityEvent OnEnemyDamaged; // Event for when the enemy takes damage
public UnityEvent OnCritAttack; // Event for when the enemy takes damage



    void Update()
    {
        if (healthSystem.IsAlive)
        {
            HandleAttack();
        }

        // Check if the enemy is dead and the rotation hasn't started yet
        if (!healthSystem.IsAlive && !isDeadRotationStarted)
        {
            // Trigger "damage" animation in Animator
            enemyAnimator.SetBool(death_animation_name, true);

            // Start the rotation process
            isDeadRotationStarted = true;
        }

        // If rotation has started but the enemy isn't hidden yet
        if (isDeadRotationStarted && !enemyHidden)
        {
            RotateEnemyAndHide();
        }
    }

    void HandleAttack()
    {
        // Check if enough time has passed for the next attack
        attackTimer += Time.deltaTime;
        if (attackTimer >= attackCooldown && !enemyAnimator.GetBool(damage_animation_name))
        {
            attackTimer = 0f;

            // Randomly trigger "attack" or "attack02" animation
            if (Random.value > 0.5f)
            {
                enemyAnimator.SetBool(attack_animation_name_1, true);
            }
            else
            {
                enemyAnimator.SetBool(attack_animation_name_2, true);
            }

            // Call the Hurt function to damage the player
			int random_value=Random.Range(1,9);
			if (random_value==7){
			playerHealth.Hurt(CritStrength);
						OnCritAttack?.Invoke(); // Add this line in EnemyDamaged() to trigger the damage event


			} else {            playerHealth.Hurt(AttackStrength);
}
			
			OnEnemyAttacking?.Invoke(); // Add this line in HandleAttack() where the enemy attacks


            // Trigger Wizard's hurt animation
            StartCoroutine(WizardHurtAnimation());

            // Reset attack animation after some time
            StartCoroutine(ResetAttackAnimation());
        }
    }

    IEnumerator ResetAttackAnimation()
    {
        yield return new WaitForSeconds(1f); // Wait for 1 second
        enemyAnimator.SetBool(attack_animation_name_1, false);
        enemyAnimator.SetBool(attack_animation_name_2, false);
    }

    IEnumerator WizardHurtAnimation()
    {
        yield return new WaitForSeconds(1.0f); // Wait for half a second

        wizardAnimator.SetBool("hurt", true);
		
        yield return new WaitForSeconds(0.5f); // Wait for half a second
        wizardAnimator.SetBool("hurt", false);
    }

    void RotateEnemyAndHide()
    {
        // Gradually rotate the enemy on the Z-axis towards -75 degrees
        float currentZRotation = enemy.transform.eulerAngles.z;
        float targetRotation = Mathf.MoveTowardsAngle(currentZRotation, rotationTarget, rotationSpeed * Time.deltaTime);

        enemy.transform.eulerAngles = new Vector3(
            enemy.transform.eulerAngles.x,
            enemy.transform.eulerAngles.y,
            targetRotation
        );

        // Check if the rotation has reached the target angle
        if (Mathf.Approximately(targetRotation, rotationTarget))
        {
            // Start the countdown to hide the enemy
            hideTimer += Time.deltaTime;

            if (hideTimer >= hideDelay)
            {
                // Hide the enemy
                enemy.SetActive(false);
                enemyHidden = true;

                // Trigger the win event
                OnWin?.Invoke();
            }
        }
    }

    public void PlayDamageAnimation()
    {
        // Start a coroutine to handle the delay and particle effect
        StartCoroutine(PlayDamageWithDelay());
    }

    // Coroutine to wait for 0.5 seconds, then play the damage animation and particles
    IEnumerator PlayDamageWithDelay()
    {
        yield return new WaitForSeconds(0.5f); // Wait for 0.5 seconds

        // Play the damage particle effect
        EnemyDamaged();

        // Set "damage" parameter to true in the animator
        enemyAnimator.SetBool(damage_animation_name, true);
    }

    // Public method to play damage particle effect
    public void EnemyDamaged()
    {
        if (damageParticleSystem != null)
        {
            damageParticleSystem.Play();
			OnEnemyDamaged?.Invoke(); // Add this line in EnemyDamaged() to trigger the damage event

        }
    }

    // Public method to play heal particle effect
    public void EnemyHealed()
    {
        if (healParticleSystem != null)
        {
            healParticleSystem.Play();
        }
    }
}
