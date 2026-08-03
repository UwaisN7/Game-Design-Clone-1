using System;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.LightTransport;

public class RewardManager : MonoBehaviour
{
   public GameManager gameManager;
   public CyberpunkGridManager gridManager;

    public int playerLives = 2;
    [SerializeField] private int additionalTime = 5;

    private void Start()
    {
        playerLives = 2;
    }

    // Call this from GridManager
    public void GrantSequenceReward(int sequenceIndex, int sequenceLength)
    {
        
        switch (sequenceIndex)
        {
            case 0:
                AddTime(); 
                Debug.Log($"Added {additionalTime} seconds to the game time.");
                break;
            case 1:
                AddPlayerLife(); 
                gameManager.livesText.text = "Lives: " + playerLives.ToString();
                Debug.Log($"Player lives increased to {playerLives}.");
                break;
            case 2:
                IncreaseBufferLength();
                Debug.Log($"Buffer size increased to {CyberpunkGridManager.bufferSize}.");
                break;
            default:
                Debug.Log($"Sequence {sequenceIndex} solved!");
                break;
        }
    }
    
    private void IncreaseBufferLength() //Increase the amount of "incorrect" selections the player is allowed to make
    {
        CyberpunkGridManager.bufferSize++;
    }
    private void AddPlayerLife()
    {
        if (playerLives < 4 )
        {
            playerLives++;
        }   
    }
    void AddTime()
    {
        GameManager.initialGameTime += additionalTime;
    }

    public void ResetProgress()
    {
        GameManager.initialGameTime = 30f;
        CyberpunkGridManager.gridSize = 5;
        CyberpunkGridManager.bufferSize = 6;
        Debug.Log("Progress Reset");
    }
    //This is a test function to check this will communicate with the GM make sure this is last in the script
    public bool CheckSelection(int value)
    {
        return gridManager.ValidateSelection(value);
    }


}