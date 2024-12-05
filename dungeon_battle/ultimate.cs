using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;


public class ultimate : MonoBehaviour
{
    public WordQuiz wordQuiz;
	    public HealthSystemForDummies enemyHealthSystem; // Reference to the enemy's health system
    public WizzardAnimationController playerWizard; // Reference to the player's wizard controller
    private int damageToEnemy = 100; // Damage dealt to enemy on correct answer (adjustable in Inspector)


    void Start()
    {
        wordQuiz.OnCorrectAnswer.AddListener(OnCorrectAnswer);
        wordQuiz.OnWrongAnswer.AddListener(OnWrongAnswer);
    }

    void OnCorrectAnswer()
    {
		          //   enemyHealthSystem.AddToCurrentHealth(-damageToEnemy); // Reduce enemy's health by damage amount
          //   playerWizard.PlayerAttack(); // Trigger attack

        Debug.Log("Activate ultimate!");
    }

    void OnWrongAnswer()
    {
        Debug.Log("Kill yourself!");
    }
}
