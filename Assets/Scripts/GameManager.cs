using TMPro;
using UnityEngine;

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
    private InputSystem inputManager;
    public TextMeshProUGUI timerText;

    [SerializeField]
    private float gameTime = 60f; // Total game time in seconds

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
        inputManager = new InputSystem();
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
                if (inputManager.Player.Attack.triggered)
                {
                   StartGame();
                    Time.timeScale = 1f; // Set the timescale to 1 when the game starts
                }

                // Player Inputs and selects a tile the Games timescale is 0 
                break;

                case GameState.Playing:
                Timer();
                Debug.Log("Game is in progress.");
                Debug.Log("Time remaining: " + gameTime + " seconds");
                //Player selects a tile and the Games timescale is 1 and timer now ticks down
                break;

            case GameState.Paused:
                //Player selects esc the timescale is 0 and the game is paused
                break;
            case GameState.Win:
                //Player gets the rewards and wins the game and the win screen is displayed timescale is 0
                break;
            case GameState.Lose:
                Debug.Log("Game Over! You lose.");
                
                //Player runs out of time or number of turns and the lose screen is displayed timescale is 0
                break;

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

    public void StartGame()
    {
        currentState = GameState.Playing;

        Time.timeScale = 1f;
    }
    public void Win()
    {

        currentState = GameState.Win;

        Time.timeScale = 0f;

        winScreen.SetActive(true);
    }

   public void Lose()
    {
        currentState = GameState.Lose;

       Time.timeScale = 0f;

       loseScreen.SetActive(true);
    }
}
