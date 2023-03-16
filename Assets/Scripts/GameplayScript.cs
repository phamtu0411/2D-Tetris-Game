using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameplayScript : MonoBehaviour
{
    public static int gridWidth = 10;
    public static int gridHeight = 20;

    public static Transform[,] grid = new Transform[gridWidth, gridHeight];

    //score += when clear line
    public int scoreOneLine = 1;
    public int scoreTwoLine = 2;
    public int scoreThreeLine = 3;
    public int scoreFourLine = 4;

    private int numberOfRowsThisTurn = 0;
    private int currentScore = 0;
    public TMP_Text scoreTXT;

    //sound
    public AudioClip clearLineSound;
    private AudioSource sound;

    //preview next tetromino
    private GameObject previewNextTetromino;
    private GameObject nextTetromino;

    private bool gameStarted = false;
    private Vector2 previewNextTetrominoPosition = new Vector2(11.5f, 15f);

    //ghost tetromino
    GameObject ghostTetromino;


    void Start()
    {
        SpawnTetromino();
        sound = GetComponent<AudioSource>();
    }

    void Update()
    {
        UpdateScore();
        UpdateUI();
    }

    //instantiate tetromino
    public void SpawnTetromino()
    {
        if (!gameStarted)
        {
            gameStarted = true;
            nextTetromino = (GameObject)Instantiate(Resources.Load(TetrominoRandom(), typeof(GameObject)), new Vector2(5f, 20f), Quaternion.identity);
            previewNextTetromino = (GameObject)Instantiate(Resources.Load(TetrominoRandom(), typeof(GameObject)), previewNextTetrominoPosition, Quaternion.identity);
            previewNextTetromino.GetComponent<TetrominoScript>().enabled = false;

            //SpawnGhostTetromino();
        }
        else
        {
            previewNextTetromino.transform.localPosition = new Vector2(5f, 20f);
            nextTetromino = previewNextTetromino;
            nextTetromino.GetComponent<TetrominoScript>().enabled = true;

            previewNextTetromino = (GameObject)Instantiate(Resources.Load(TetrominoRandom(), typeof(GameObject)), previewNextTetrominoPosition, Quaternion.identity);
            previewNextTetromino.GetComponent<TetrominoScript>().enabled = false;

            //SpawnGhostTetromino();
        }
    }

    //check inside grid
    public bool IsInsideGrid(Vector2 pos)
    {
        return ((int)pos.x >= 0 && (int)pos.x < gridWidth && (int)pos.y >= 0);
    }

    //round
    public Vector2 Round(Vector2 pos)
    {
        return new Vector2(Mathf.Round(pos.x), Mathf.Round(pos.y));
    }

    //tetromino name on resources
    string TetrominoRandom()
    {
        int random = Random.Range(1, 8);
        string name = "Prefabs/Tetromino L";
        switch (random)
        {
            case 1:
                name = "Prefabs/Tetromino J";
                break;
            case 2:
                name = "Prefabs/Tetromino I";
                break;
            case 3:
                name = "Prefabs/Tetromino S";
                break;
            case 4:
                name = "Prefabs/Tetromino Z";
                break;
            case 5:
                name = "Prefabs/Tetromino O";
                break;
            case 6:
                name = "Prefabs/Tetromino T";
                break;
            case 7:
                name = "Prefabs/Tetromino L";
                break;
        }
        return name;
    }

    //take to a new location based on tetromino current location.
    public void UpdateGrid(TetrominoScript tetromino)
    {
        for (int y = 0; y < gridHeight; ++y)
        {
            for (int x = 0; x < gridWidth; ++x)
            {
                //check if the current cell in the grid has a tetromino
                if (grid[x, y] != null)
                {
                    //delete the current tetromino from the previous position
                    if (grid[x, y].parent == tetromino.transform)
                    {
                        grid[x, y] = null;
                    }
                }
            }
        }

        foreach (Transform mino in tetromino.transform)
        {
            //rounding
            Vector2 pos = Round(mino.position);

            //check pos y is in gridHeight of grid
            if (pos.y < gridHeight)
            {
                //update the corresponding cell in the grid
                grid[(int)pos.x, (int)pos.y] = mino;
            }
        }
    }

    public Transform GetTransformGridPosition(Vector2 pos)
    {
        //check pos y of input position >= gridheight
        if (pos.y > gridHeight - 1)
        {
            return null;
        }
        //get the value of the corresponding cell in the grid
        //by using the pos x, y of the input location
        else
        {
            return grid[(int)pos.x, (int)pos.y];
        }
    }

    //check row is full tetromino
    public bool IsFullTetrominoPerRow(int y)
    {
        for (int x = 0; x < gridWidth; ++x)
        {
            //check the corresponding cell in the "grid" == null
            if (grid[x, y] == null)
            {
                return false;
            }
        }
        //found a full row => increase all available rows
        numberOfRowsThisTurn++;

        return true;
    }

    //delete tetromino
    public void DestroyTetromino(int y)
    {
        for (int x = 0; x < gridWidth; ++x)
        {
            //delete
            Destroy(grid[x, y].gameObject);
            //set the corresponding cells in the "grid" = null.
            grid[x, y] = null;
        }
    }

    //move row to below row
    public void MoveRowDown(int y)
    {
        for (int x = 0; x < gridWidth; ++x)
        {
            //check the corresponding cell in the "grid" != null
            if (grid[x, y] != null)
            {
                //set tetromino in that cell to the one below cell
                grid[x, y - 1] = grid[x, y];
                //set current cell = null
                grid[x, y] = null;
                //move tetromino to new location
                grid[x, y - 1].position += new Vector3(0, -1, 0);
            }
        }
    }

    //move all rows down
    public void MoveAllRowsDown(int y)
    {
        for (int i = y; i < gridHeight; ++i)
        {
            MoveRowDown(i);
        }
    }

    //delete all tetrominoes on row  
    public void DestroyRow()
    {
        for (int y = 0; y < gridHeight; ++y)
        {
            if (IsFullTetrominoPerRow(y))
            {
                DestroyTetromino(y);
                //move all rows above the canceled row down
                MoveAllRowsDown(y + 1);
                --y;
            }
        }
    }

    //check tetromino in the top row of the grid
    public bool IsAboveGrid(TetrominoScript tetromino)
    {
        for (int x = 0; x < gridWidth; ++x)
        {
            foreach (Transform mino in tetromino.transform)
            {
                Vector2 pos = Round(mino.position);
                //pos in the top row of the grid
                if (pos.y > gridHeight - 1)
                {
                    return true;
                }
            }
        }
        return false;
    }

    //next scene
    public void GameOver()
    {
        SceneManager.LoadScene("GameOver");
    }

    public void UpdateUI()
    {
        scoreTXT.text = currentScore.ToString();
    }

    public void UpdateScore()
    {
        if (numberOfRowsThisTurn > 0)
        {
            if (numberOfRowsThisTurn == 1)
            {
                ClearOneLine();
            }
            else if (numberOfRowsThisTurn == 2)
            {
                ClearTwoLine();
            }
            else if (numberOfRowsThisTurn == 3)
            {
                ClearThreeLine();
            }
            else if (numberOfRowsThisTurn == 4)
            {
                ClearFourLine();
            }
            numberOfRowsThisTurn = 0;
            PlayClearLineSound();
        }
    }

    public void ClearOneLine()
    {
        currentScore += scoreOneLine;
    }
    public void ClearTwoLine()
    {
        currentScore += scoreTwoLine;
    }
    public void ClearThreeLine()
    {
        currentScore += scoreThreeLine;
    }
    public void ClearFourLine()
    {
        currentScore += scoreFourLine;
    }

    void PlayClearLineSound()
    {
        sound.PlayOneShot(clearLineSound);
    }

    /*public void SpawnGhostTetromino()
    {
        if (GameObject.FindGameObjectWithTag("CurrentGhostTetromino") != null)
        {
            Destroy(GameObject.FindGameObjectWithTag("CurrentGhostTetromino"));
        }

        ghostTetromino = (GameObject)Instantiate(nextTetromino, nextTetromino.transform.position, Quaternion.identity);
        Destroy(ghostTetromino.GetComponent<TetrominoScript>());
        ghostTetromino.AddComponent<GhostTetromino>();
    }*/
}