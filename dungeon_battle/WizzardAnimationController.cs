using System.Collections.Generic;
using UnityEngine;
using TMPro;  // For TextMeshPro
using UnityEngine.UI; // For Dropdown
using System.IO;
using System.Collections; // Ensure this is included for IEnumerator
using UnityEngine.Events;

public class WizzardAnimationController : MonoBehaviour
{
    public Animator wizzardAnimator; // Reference to the wizard's Animator
    public ParticleSystem healParticleSystem; // Particle system for heal effect

    // Method to handle the healing animation and particles
    public void PlayerHealed()
    {
        // Set "isLookUp" parameter to true in the animator
        wizzardAnimator.SetBool("isLookUp", true);

        // Play the heal particle effect
        if (healParticleSystem != null)
        {
            healParticleSystem.Play();
        }

        // Reset "isLookUp" after 1 second
        StartCoroutine(ResetIsLookup());
    }

    // Coroutine to reset the "isLookUp" parameter
    IEnumerator ResetIsLookup()
    {
        yield return new WaitForSeconds(1f); // Wait for 1 second
        wizzardAnimator.SetBool("isLookUp", false);
    }

    // Method to handle the attack animation
    public void PlayerAttack()
    {
        // Ensure "isLookUp" is false
        wizzardAnimator.SetBool("isLookUp", false);

        // Trigger the attack animation
        wizzardAnimator.SetBool("attack", true);
    }
}
