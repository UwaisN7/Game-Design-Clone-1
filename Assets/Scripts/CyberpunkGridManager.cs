using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class CyberpunkGridManager : MonoBehaviour
{
    [SerializeField]
    private GameManager gameManager;
    [SerializeField]
    private RewardManager rewardManager;
    [Header("Grid Config")]
    public static int gridSize = 5;
    public static int bufferSize = 6;
    public int[] codePool = new int[] { 10, 20, 30, 40, 50 };
    private bool firstMove = true;

    [Header("UI References")]
    public Transform gridContainer;
    public CellUI cellPrefab;
    public TextMeshProUGUI targetCodesText;
    public TextMeshProUGUI bufferText;

    private CellUI[,] cellGrid;
    private List<List<int>> targetCodes = new List<List<int>>();
    private List<int> playerBuffer = new List<int>();

    private bool isHorizontal = true;
    private int activeIndex = 0;
    private bool gameActive = false;
    private bool[] solvedSequences;


    void Start() => StartGame();

    public void StartGame()
    {
        playerBuffer.Clear();
        isHorizontal = true;
        activeIndex = 0; // Starts locked to Top Row (Row 0)
      
        firstMove = true;
        bufferText.color = Color.white; // Reset color on start

        GeneratePuzzleData(out int[,] rawGrid, out targetCodes);
        solvedSequences = new bool[targetCodes.Count];
        DisplayTargetCodes();
        SpawnGridUI(rawGrid);
        UpdateGridHighlights();
        UpdateBufferUI();

        gameManager.roundLoseScreen.SetActive(false);
        gameManager.currentState = GameManager.GameState.StartOfGame;
    }

    void SpawnGridUI(int[,] rawGrid)
    {
        foreach (Transform child in gridContainer) Destroy(child.gameObject);
        cellGrid = new CellUI[gridSize, gridSize];

        for (int y = 0; y < gridSize; y++)
        {
            for (int x = 0; x < gridSize; x++)
            {
                CellUI cell = Instantiate(cellPrefab, gridContainer);
                cell.Setup(rawGrid[x, y], new Vector2Int(x, y), OnCellClicked);
                cellGrid[x, y] = cell;
            }
        }
    }

    void OnCellClicked(CellUI clickedCell)
    {
        if (gameManager.currentState != GameManager.GameState.Playing)
            return;

        clickedCell.MarkAsUsed();
        gameManager.PlayerSelected(clickedCell.Code);
     

        if (firstMove)
        {
            firstMove = false;

            isHorizontal = true;
            activeIndex = clickedCell.GridPos.y;

          
        }
        else
        {
            if (isHorizontal)
            {
                isHorizontal = false;
                activeIndex = clickedCell.GridPos.x;
            }
            else
            {
                isHorizontal = true;
                activeIndex = clickedCell.GridPos.y;
            }
        }

        // These should happen EVERY click
        UpdateGridHighlights();
        //UpdateBufferUI();
     
    }

    void UpdateGridHighlights()
    {
        {
            for (int x = 0; x < gridSize; x++)
            {
                for (int y = 0; y < gridSize; y++)
                {
                    if (cellGrid[x, y].IsUsed)
                        continue;

                    bool isSelectable;

                    if (firstMove)
                    {
                        // First move: every unused tile is selectable
                        isSelectable = true;
                    }
                    else
                    {
                        // After the first move, follow the row/column rule
                        isSelectable = isHorizontal
                            ? (y == activeIndex)
                            : (x == activeIndex);
                    }

                    cellGrid[x, y].SetHighlight(isSelectable);
                }
            }
        }
    }

    void DisplayTargetCodes() 
    {
        string output = "";
        for (int i = 0; i < targetCodes.Count; i++)
        {
            string seqString = string.Join("  ", targetCodes[i]);

            if (solvedSequences[i])
            {
                // Wraps the solved sequence in brackets and dims it grey
                output += $"<color=#555555>[ {seqString} ]</color>\n";
            }
            else
            {
                output += $"{seqString}\n";
            }
        }
        targetCodesText.text = output;
    }

    void UpdateBufferUI()
    {
        bufferText.text = string.Join("  ", playerBuffer);
    }

    void CheckForMatches() 
    {
        bool allSolved = true;

        for (int i = 0; i < targetCodes.Count; i++)
        {
            if (solvedSequences[i]) continue; // Skip if already solved

            List<int> seq = targetCodes[i];

            // Only check if the buffer has enough numbers to form the sequence
            if (playerBuffer.Count >= seq.Count)
            {
                bool isMatch = true;
                int bufferStartIndex = playerBuffer.Count - seq.Count;

                // Check if the end of the buffer perfectly matches this sequence
                for (int j = 0; j < seq.Count; j++)
                {
                    if (playerBuffer[bufferStartIndex + j] != seq[j])
                    {
                        isMatch = false;
                        break;
                    }
                }

                if (isMatch)
                {
                    solvedSequences[i] = true;

                    if (rewardManager != null)
                    {
                        rewardManager.GrantSequenceReward(i, seq.Count);
                    }
                }
            }

            if (!solvedSequences[i]) allSolved = false;
        }

        DisplayTargetCodes(); // Refresh UI to show newly solved sequences
        bool allSequencesSolved = solvedSequences.All(x => x);
        bool bufferFull = playerBuffer.Count >= bufferSize;

        bool atLeastOneSolved = solvedSequences.Any(x => x);

        // The game ONLY ends when the buffer is completely filled 
        // OR if the player  solves every sequence available.
        if (bufferFull || allSolved)
        {
            if (atLeastOneSolved)
            {
                // Player completed 1 or more sequences before running out of buffer space
                bufferText.color = Color.green;
                gameManager.Win();
            }
            else
            {
                // Buffer is full and 0 sequences were completed
                bufferText.color = Color.red;
                gameManager.Lose();
            }

            gameActive = false;
        }

    }

    void GeneratePuzzleData(out int[,] grid, out List<List<int>> sequences)
    {
        grid = new int[gridSize, gridSize];
        sequences = new List<List<int>>();
        List<Vector2Int> path = new List<Vector2Int>();
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();

        int cRow = 0;
        int cCol = Random.Range(0, gridSize);
        Vector2Int startCell = new Vector2Int(cCol, cRow);
        path.Add(startCell);
        visited.Add(startCell);

        bool vertical = true;
        for (int i = 0; i < bufferSize - 1; i++)
        {
            List<Vector2Int> valid = new List<Vector2Int>();
            if (vertical)
            {
                for (int r = 0; r < gridSize; r++)
                    if (!visited.Contains(new Vector2Int(cCol, r))) valid.Add(new Vector2Int(cCol, r));
            }
            else
            {
                for (int c = 0; c < gridSize; c++)
                    if (!visited.Contains(new Vector2Int(c, cRow))) valid.Add(new Vector2Int(c, cRow));
            }

            if (valid.Count == 0) break;
            Vector2Int nextCell = valid[Random.Range(0, valid.Count)];
            visited.Add(nextCell);
            path.Add(nextCell);

            cCol = nextCell.x;
            cRow = nextCell.y;
            vertical = !vertical;
        }

        List<int> pathValues = new List<int>();
        foreach (Vector2Int pos in path)
        {
            int val = codePool[Random.Range(0, codePool.Length)];
            pathValues.Add(val);
            grid[pos.x, pos.y] = val;
        }

        if (pathValues.Count >= 2)
            sequences.Add(pathValues.GetRange(0, 2));

        if (pathValues.Count >= 4)
            sequences.Add(pathValues.GetRange(1, 3));

        if (pathValues.Count >= 6)
            sequences.Add(pathValues.GetRange(3, 3));

        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                if (grid[x, y] == 0) grid[x, y] = codePool[Random.Range(0, codePool.Length)];
            }
        }
    }

    public void OnTimeExpired()
    {
        // Don't re-trigger if the round is already over
        if (gameManager.currentState != GameManager.GameState.Playing) return;

        // Check if player solved at least 1 sequence before time ran out
        bool atLeastOneSolved = solvedSequences != null && solvedSequences.Any(x => x);

        if (atLeastOneSolved)
        {
            bufferText.color = Color.green;
            gameManager.Win();
        }
        else
        {
            bufferText.color = Color.red;
            gameManager.Lose();
        }
    }

    public bool ValidateSelection(int value)
    {
        playerBuffer.Add(value);

        UpdateBufferUI();

        CheckForMatches();

        return solvedSequences.All(x => x);
    }
}