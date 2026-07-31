using System;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.LightTransport;

public class RewardManager : MonoBehaviour
{
    private GameManager gameManager;
    //private CyberpunkGridManager gridManager;

    public int playerLives; 
    
    [SerializeField] private int correctAnswer = 7;      //This is a test but the correct answers are stored here and will change when u do random
    [SerializeField] private int additionalTime;
    

    //So like the logic for randomising numbers will be here 

    private void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();

        //playerLives = 3;
    }



    //This is a test function to check this will communicate with the GM make sure this is last in the script
    public bool CheckSelection(int selectedValue)
    {
        //Debug.Log("Received " + selectedValue);

        if (selectedValue == correctAnswer)
        {
            Debug.Log("Correct!");
            return true;
        }

        Debug.Log("Not part of the reward.");
        if (gameManager != null)
        {
            Debug.Log("Calling Lose() on GameManager.Piece of crap works ");
            gameManager.Lose();
        }
        else
        {
            Debug.LogWarning("GameManager reference is null. Cannot call Lose().im dumb");
        }
        
        return false;
    }

    public void IncreaseGridSize() // Inreases grid size by one row and column. Gives additional time
    {
        if (CyberpunkGridManager.gridSize <= 8)
        {
            CyberpunkGridManager.gridSize++; 
        }
        gameManager.gameTime += additionalTime;
    }
    public void IncreaseBufferLength() //Increase the amount of "incorrect" selections the player is allowed to make
    {
        CyberpunkGridManager.bufferSize++;
    }
    public void AddPlayerLife()
    {
        if (playerLives < 4 )
        {
            playerLives++;
        }   
    }

    public void ResetProgress()
    {
        //GameManager.currentRound = 1;
        GameManager.initialGameTime = 60f;
        CyberpunkGridManager.gridSize = 5;
        CyberpunkGridManager.bufferSize = 6;
        Debug.Log("Progress Reset");
    }



    //Ok bro when you see this comment (this is tira's work you can use it if you want or do ur own thing just want  to give you options)

    //Basically below is a  reward manager thats on the grid system script.so if u wanna reuse it you are going to have to replace variables 
    //It does not work in its curret state

    //And when you are done be sure to delete this code within the grid manager script because it might cause conflicts with your work...

    //So it does everything we want so thats why i feel like it shoudl be looked at.


    //void CheckForMatches()
    //{
    //    bool allSolved = true;

    //    for (int i = 0; i < targetCodes.Count; i++)
    //    {
    //        if (solvedSequences[i]) continue; // Skip if already solved

    //        List<int> seq = targetCodes[i];

    //        // Only check if the buffer has enough numbers to form the sequence
    //        if (playerBuffer.Count >= seq.Count)
    //        {
    //            bool isMatch = true;
    //            int bufferStartIndex = playerBuffer.Count - seq.Count;

    //            // Check if the end of the buffer perfectly matches this sequence
    //            for (int j = 0; j < seq.Count; j++)
    //            {
    //                if (playerBuffer[bufferStartIndex + j] != seq[j])
    //                {
    //                    isMatch = false;
    //                    break;
    //                }
    //            }

    //            if (isMatch) solvedSequences[i] = true;
    //        }

    //        if (!solvedSequences[i]) allSolved = false;
    //    }

    //    DisplayTargetCodes(); // Refresh UI to show newly solved sequences

    //    // End Game Conditions
    //    if (allSolved)
    //    {
    //        gameActive = false;
    //        bufferText.color = Color.green; // Visual win indicator
    //    }
    //    else if (playerBuffer.Count >= bufferSize)
    //    {
    //        gameActive = false;
    //        bufferText.color = Color.red; // Visual loss indicator
    //    }
    //}
}