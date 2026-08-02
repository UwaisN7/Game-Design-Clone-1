using JetBrains.Annotations;
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
        Lose,
        FailedRound
    }
    public GameState currentState;
    public GameObject winScreen;
    public GameObject loseScreen;
    public GameObject roundLoseScreen;
    private PlayerInput inputManager;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI roundCountText;
    public TextMeshProUGUI livesText;

    public GameObject pauseScreen;
    [SerializeField]
    private RewardManager rewardManager;
    public CyberpunkGridManager gridManager;
    //public bool gamePaused;

    public string selectedValue;

    [SerializeField]
    public float gameTime; // Total game time in seconds
    public static float initialGameTime = 60f; // Initial game time for reference

    public readonly int startingRound = 1;
    public int currentRound;
    private bool roundUIUpdated = false; //bool to check if the round UI has been updated

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
        if (roundLoseScreen != null)
        {
            loseScreen.SetActive(false);
        }
        if (timerText != null)
        {
            timerText.text = "Time: " + gameTime.ToString("F0") + "s";
        }
        inputManager = new PlayerInput();

        if(currentRound == 1)
        {
            initialGameTime = 60f;
            gameTime = initialGameTime; // Reset game time to initial value at the start of the game
            
        }
        else
        {
            gameTime = initialGameTime; // initialGametime is recalculated in the NextRound() function
        }
        currentRound = startingRound;//Just to make sure the round starts at 1 and not 0
        roundUIUpdated = false;//Make sure the round UI is updated at the start of the game

    }

    void OnEnable()
    {
        inputManager.Enable();
    }

    void OnDisable()
    {
        inputManager.Disable();
    }

    private void Start()
    {
        rewardManager.playerLives = 3; // Reset player lives to 3 at the start of the game
    }
    void Update()
    {
        switch (currentState)
        {
            case GameState.StartOfGame:
                livesText.text = "Lives: " + rewardManager.playerLives.ToString();
                if (inputManager.Player.Click.triggered)
                {
                    Time.timeScale = 1f; // Set the timescale to 1 when the game starts 
                    currentState = GameState.Playing;
                }
                if (roundUIUpdated == false)// Check if the round UI has been updated, if not update it
                {
                    UpdateRoundUI();
                    timerText.text = "Time: " + gameTime.ToString("F0");
                }
                break;


            case GameState.Playing:
                Time.timeScale = 1f;
                Timer();
                if (inputManager.Player.Click.triggered)
                {
                    PlayerSelected(int.Parse(selectedValue));
                }
                //This is a test to see if the reward manager is working and communicating with the GM
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
                rewardManager.ResetProgress();//reset the progress + upgrades in the RewardManager when the player loses

                //Player runs out of time or number of turns and the lose screen is displayed timescale is 0
                break;
             case GameState.FailedRound:
                Debug.Log("Round Failed! You lose a life.");
                Time.timeScale = 0f;
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
    public void UpdateRoundUI()
    {
        roundCountText.text = "Round: " + currentRound.ToString();
        roundUIUpdated = true;
    }


    public void Win()
    {
        currentState = GameState.Win;
        rewardManager.UpgradeSelector(); // Call the UpgradeRandomiser function in the RewardManager when the player wins
    }

    public void Lose()
    {
        if(rewardManager.playerLives == 1)
        {
            LoseRun();
        }
        else
        {
            roundLoseScreen.SetActive(true);
            currentState = GameState.FailedRound;
            gameTime = initialGameTime;
            rewardManager.playerLives -= 1; // Decrease player lives by 1
            Debug.Log("Player lives remaining: " + rewardManager.playerLives);
            livesText.text = "Lives: " + rewardManager.playerLives.ToString();
        }
            
    }
    public void LoseRun()
    {
        loseScreen.SetActive(true);
        currentState = GameState.Lose;
        gameTime = initialGameTime; // Reset the game time to the initial value
        currentRound = startingRound; // Reset the current round to the starting round (1)
        rewardManager.ResetProgress(); // Reset the progress + upgrades in the RewardManager
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
        roundUIUpdated = false;
        winScreen.SetActive(false);

        currentRound += 1;
        initialGameTime -= (gameTime * 0.1f);
        gameTime = initialGameTime;

        gridManager.StartGame();
        currentState= GameState.StartOfGame;

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