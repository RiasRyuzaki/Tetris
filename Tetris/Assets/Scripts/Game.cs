using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
    public static int gridWidth = 15;
    public static int gridHeight = 24;

    public static Transform[,] grid = new Transform[gridWidth, gridHeight];


    // Start is called before the first frame update
    void Start()
    {
        SpawnNextBlock();
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
        GameObject nextBlock = (GameObject)Instantiate(Resources.Load(GetRandomBlock(), typeof(GameObject)), new Vector2(5.0f, 23.0f), Quaternion.identity);
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
        SceneManager.LoadScene("GameOver");
    }
}
