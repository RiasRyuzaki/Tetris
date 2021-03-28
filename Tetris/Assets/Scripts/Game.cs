using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    public static int gridWidth = 15;
    public static int gridHeight = 24;

    public static Transform[,] grid = new Transform[gridWidth, gridHeight];

    public int clearOneLine = 40;
    public int clearTwoLine = 100;
    public int clearThreeLine = 300;
    public int clearFourLine = 1200;

    public int currentLevel = 0;
    private int numLinesCleared = 0;
    public float fallSpeed = 1.0f;

    public AudioClip clearedLineSound;

    public Text hudScore;
    public Text hudLevel;
    public Text hudLines;

    private int numberOfRowsThisTurn = 0;
    private AudioSource audioSource;
    public static int currentScore = 0;

    private GameObject previewBlock;
    private GameObject nextBlock;

    private bool gameStarted = false;
    private int startingHighScore;
    private int startingHighScore2;
    private int startingHighScore3;
    private Vector2 previewBlockPosition = new Vector2(-6.5f, 20);

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        currentScore = 0;
        hudScore.text = "0";
        hudLines.text = "0";
        SpawnNextBlock();
        audioSource = GetComponent<AudioSource>();
        startingHighScore = PlayerPrefs.GetInt("highscore");
        startingHighScore2 = PlayerPrefs.GetInt("highscore2");
        startingHighScore3 = PlayerPrefs.GetInt("highscore3");
    }

    void Update()
    {
        UpdateScore();
        UpdateUI();
        UpdateLevel();
        UpdateSpeed();
    }

    void UpdateLevel()
    {
        currentLevel = numLinesCleared / 10;
    }

    void UpdateSpeed()
    {
        fallSpeed = 1.0f - ((float)currentLevel * 0.1f);
    }

    public void UpdateUI()
    {
        hudScore.text = currentScore.ToString();
        hudLevel.text = currentLevel.ToString();
        hudLines.text = numLinesCleared.ToString();
    }

    public void UpdateScore()
    {
        if (numberOfRowsThisTurn >0)
        {
            if (numberOfRowsThisTurn ==1)
            {
                ClearedOneLine();
            }
            else if (numberOfRowsThisTurn == 2)
            {
                ClearedTwoLine();
            }
            else if (numberOfRowsThisTurn == 3)
            {
                ClearedThreeLine();
            }
            else if (numberOfRowsThisTurn == 4)
            {
                ClearedFourLine();
            }
            numberOfRowsThisTurn = 0;
            PlayLineClearedSound();

        }
        
    }

    public void ClearedOneLine()
    {
        currentScore += clearOneLine + (currentLevel *20);
        numLinesCleared++;
    }

    public void ClearedTwoLine()
    {
        currentScore += clearTwoLine + (currentLevel *25);
        numLinesCleared += 2;
    }

    public void ClearedThreeLine()
    {
        currentScore += clearThreeLine + (currentLevel *30);
        numLinesCleared += 3;
    }

    public void ClearedFourLine()
    {
        currentScore += clearFourLine +(currentLevel *40);
        numLinesCleared += 4;
    }
    
    public void PlayLineClearedSound()
    {
        audioSource.PlayOneShot(clearedLineSound);
    }

    public void UpdateHighScore()
    {
        if (currentScore > startingHighScore)
        {
            PlayerPrefs.SetInt("highscore3", startingHighScore2);
            PlayerPrefs.SetInt("highscore2", startingHighScore);
            PlayerPrefs.SetInt("highscore", currentScore);
        }
        else if (currentScore > startingHighScore2)
        {
            PlayerPrefs.SetInt("highscore3", startingHighScore2);
            PlayerPrefs.SetInt("highscore2", currentScore);
        }
        else if (currentScore > startingHighScore3)
        {
            PlayerPrefs.SetInt("highscore3", currentScore);
        }
    }

    public bool CheckIsAboveGrid(Blocks block)
    {
        for (int x=0; x<gridWidth; ++x)
        {
            foreach (Transform bk in block.transform)
            {
                Vector2 pos = Round(bk.position);

                if (pos.y > gridHeight -1)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public bool IsFullRowAt(int y)
    {
        for (int x=0; x<gridWidth; ++x) 
        {
            if (grid[x,y] == null)
            {
                return false;
            }
        }
        numberOfRowsThisTurn++;
        return true;
    }

    public void DeleteBlockAt(int y)
    {
        for (int x = 0; x<gridWidth; ++x)
        {
            Destroy(grid[x, y].gameObject);
            grid[x, y] = null;
        }
    }

    public void MoveRowDown(int y)
    {
        for (int x=0; x<gridWidth; ++x)
        {
            if(grid[x,y] != null)
            {
                grid[x, y - 1] = grid[x, y];
                grid[x, y] = null;
                grid[x, y - 1].position += new Vector3(0, -1, 0);
            }
        }
    }

    public void MoveAllRowDown(int y)
    {
        for (int i=y; i<gridHeight; ++i)
        {
            MoveRowDown(i);
        }
    }

    public void UpdateGrid(Blocks block)
    {
        for (int y = 0; y< gridHeight; ++y)
        {
            for (int x = 0; x< gridWidth; ++x)
            {
                if (grid[x, y] != null)
                {
                    if (grid[x,y].parent == block.transform)
                    {
                        grid[x, y] = null;
                    }
                }
            }
        }
        foreach (Transform bk in block.transform)
        {
            Vector2 pos = Round(bk.position);
            if (pos.y < gridHeight)
            {
                grid[(int)pos.x, (int)pos.y] = bk;
            }
        }

    }

    public Transform GetTransformAtGridPosition (Vector2 pos)
    {
        if (pos.y > gridHeight - 1)
        {
            return null;
        }
        else
        {
            return grid[(int)pos.x, (int)pos.y];
        }
    }

    public void SpawnNextBlock()
    {
        if (!gameStarted)
        {
            gameStarted = true;
            nextBlock = (GameObject)Instantiate(Resources.Load(GetRandomBlock(), typeof(GameObject)), new Vector2(5.0f, 23.0f), Quaternion.identity);
            previewBlock = (GameObject)Instantiate(Resources.Load(GetRandomBlock(), typeof(GameObject)), previewBlockPosition, Quaternion.identity);
            previewBlock.GetComponent<Blocks>().enabled = false;

        }
        else
        {
            previewBlock.transform.localPosition = new Vector2(5.0f, 23.0f);
            nextBlock = previewBlock;
            nextBlock.GetComponent<Blocks>().enabled = true;

            previewBlock = (GameObject)Instantiate(Resources.Load(GetRandomBlock(), typeof(GameObject)), previewBlockPosition, Quaternion.identity);
            previewBlock.GetComponent<Blocks>().enabled = false;
        }
        
    }

    public void DeleteRow()
    {
        for (int y=0; y<gridHeight; ++y)
        {
            if (IsFullRowAt(y))
            {
                DeleteBlockAt(y);
                MoveAllRowDown(y + 1);
                --y;
            }
        }
    }

    public bool CheckIsInsideGrid(Vector2 pos)
    {
        return ((int)pos.x >= 0 && (int)pos.x < gridWidth && (int)pos.y >= 0);
    }

    public Vector2 Round(Vector2 pos)
    {
        return new Vector2(Mathf.Round(pos.x), Mathf.Round(pos.y));
    }

    string GetRandomBlock()
    {
        int randomBlock = Random.Range(1, 8);
        string randomBlockName = "Prefabs/TBlock";
        switch (randomBlock)
        {
            case 1:
                randomBlockName = "Prefabs/JBlock";
                break;
            case 2:
                randomBlockName = "Prefabs/LBlock";
                break;
            case 3:
                randomBlockName = "Prefabs/LongBlock";
                break;
            case 4:
                randomBlockName = "Prefabs/SBlock";
                break;
            case 5:
                randomBlockName = "Prefabs/SqBlock";
                break;
            case 6:
                randomBlockName = "Prefabs/TBlock";
                break;
            case 7:
                randomBlockName = "Prefabs/ZBlock";
                break;
        }
        return randomBlockName;
        
    }

    public void GameOver()
    {
        UpdateHighScore();
        SceneManager.LoadScene("GameOver");
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
