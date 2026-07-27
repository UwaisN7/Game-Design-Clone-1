using UnityEngine;

public class RewardManager : MonoBehaviour
{
    [SerializeField] private string correctAnswer = "7";      //This is a test but the correct answers are stored here and will change when u do random


    //So like the logic for randomising numbers will be here 



    //This is a test function to check this will communicate with the GM make sure this is last in the script
    public bool CheckSelection(string selectedValue) //slected value is from the GM //Ill leave the multiple numbers and how many is needed to for u 
    {
        Debug.Log("Reward Manager received: " + selectedValue);

        if (selectedValue == correctAnswer)  //CorrectAnswer variable will be the one that changes
        {
            Debug.Log("Correct!");

            return true;
        }

        Debug.Log("Wrong!");

        return false;
    }
}