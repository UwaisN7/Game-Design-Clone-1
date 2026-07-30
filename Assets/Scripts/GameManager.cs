using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        StartOfGame,
        Playing,
        Paused,
        Win,
        Lose
    }
    public GameState currentState;
    public GameObject winScreen;
    public GameObject loseScreen;
    private PlayerInput inputManager;
    public TextMeshProUGUI timerText;

    public GameObject pauseScreen;
    [SerializeField]
    private RewardManager rewardManager;
    //public bool gamePaused;

    public string selectedValue;

    [SerializeField]
    public float gameTime = 60f; // Total game time in seconds
    public static float initialGameTime = 60f; // Initial game time for reference

    public static int round = 1;

    void Awake()
    {
        if (winScreen != null)
        {
            winScreen.SetActive(false);
        }
        if (loseScreen != null)
        {
            loseScreen.SetActive(false);
        }
        if (timerText != null)
        {
            timerText.text = "Time: " + gameTime.ToString("F0") + "s";
        }
        inputManager = new PlayerInput();

        gameTime = initialGameTime; // Reset game time to initial value at the start of the game

    }

    void OnEnable()
    {
        inputManager.Enable();
    }

    void OnDisable()
    {
        inputManager.Disable();
    }


    void Update()
    {
        switch (currentState)
        {
            case GameState.StartOfGame:
                if (inputManager.Player.Click.triggered)
                {
                    Time.timeScale = 1f; // Set the timescale to 1 when the game starts
                    //Debug.Log("Game has started.");//Yeah this is a placeholder  before we get the grid 
                    currentState = GameState.Playing;
                }

                // Player Inputs and selects a tile the Games timescale is 0 
                break;

            case GameState.Playing:
                Timer();

                if (inputManager.Player.Click.triggered)
                {
                    PlayerSelected(int.Parse(selectedValue));
                }

                //This is a test to see if the reward manager is working and communicating with the GM
                //Debug.Log("Game is in progress.");
                //Debug.Log("Time remaining: " + gameTime + " seconds");

                //Player selects a tile and the Games timescale is 1 and timer now ticks down
                break;

            case GameState.Paused:
                //Player selects esc the timescale is 0 and the game is paused
                Time.timeScale = 0f;
                pauseScreen.SetActive(true);
                break;
            case GameState.Win:
                //Player gets the rewards and wins the game and the win screen is displayed timescale is 0
                Time.timeScale = 0f;
                winScreen.SetActive(true);
                break;
            case GameState.Lose:
                Debug.Log("Game Over! You lose.");
                Time.timeScale = 0f;
                loseScreen.SetActive(true);
                //reset progress should be called here
                rewardManager.ResetProgress();

                //Player runs out of time or number of turns and the lose screen is displayed timescale is 0
                break;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (currentState == GameState.Playing)
            {
                PauseGame();
                currentState = GameState.Paused;
            }
            else
            {
                ResumeGame();
                currentState = GameState.Playing;
            }
        }
    }

    public void Timer()
    {

        if (gameTime > 0)
        {
            gameTime -= Time.deltaTime;
            timerText.text = "Time: " + gameTime.ToString("F0");
        }
        else
        {
            Lose();
        }
    }


    public void Win()
    {
        currentState = GameState.Win;
    }

    public void Lose()
    {
        currentState = GameState.Lose;
    }

    // UI Functions
    public void QuitGame()
    {
        currentState = GameState.Lose;
        Application.Quit();
        Debug.Log("Quit function working");
        rewardManager.ResetProgress();

    }

    public void QuitToMainMenu()
    {
        currentState = GameState.Lose;
        SceneManager.LoadScene("MainMenu");
    }
    public void StartGame()
    {
        currentState = GameState.Playing;
        SceneManager.LoadScene("ActualGameScene"); //When player clicks the "Start Game" button the *game* scene will open.
        Time.timeScale = 1f;
    }
    public void PauseGame()
    {
        currentState = GameState.Paused;
    }

    public void ResumeGame()
    {
        currentState = GameState.Playing;
        Time.timeScale = 1f;
        pauseScreen.SetActive(false);
    }

    public void NextRound()
    {
        round ++;
        Debug.Log("Round " + round + " started");
        gameTime -= (gameTime * 0.1f);
        Debug.Log("Time reduced by " + (gameTime * 0.1f) + " seconds");
    }

    


    //Grid generator and reward system will be added here in the future

    public void PlayerSelected(int selectedValue)
    {
        bool finished =
            rewardManager.CheckSelection(selectedValue);

        if (finished)
        {
            Win();
        }
    }
}