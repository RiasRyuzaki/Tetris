using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    // Update is called once per frame
    void Update()
    {
        
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
}
