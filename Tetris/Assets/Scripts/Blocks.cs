using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blocks : MonoBehaviour
{
    public float blockmove = 1f;
    float fall = 0;
    private float fallSpeed;

    public bool allowRotation = true;
    public bool limitRotation = false;

    public AudioClip moveSound;
    public AudioClip rotateSound;
    public AudioClip landSound;
    private AudioSource audioSource;
    public int individualScore = 100;
    private float individualScoreTime;

    private float continuousVertSpeed = 0.05f;
    private float continuousHorSpeed = 0.1f;
    private float buttonDownWaitMax = 0.2f;
    private float verticalTimer = 0;
    private float horizontalTimer = 0;
    private float buttonDownWaitTimerH = 0;
    private float buttonDownWaitTimerV = 0;

    private bool moveImmediateHorizontal = false;
    private bool moveImmediateVertical = false;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        fallSpeed = GameObject.Find("GameScript").GetComponent<Game>().fallSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        CheckUserInput();
        UpdateIndividualScore();
    }

    void UpdateIndividualScore()
    {
        if (individualScoreTime<1)
        {
            individualScoreTime += Time.deltaTime;
        }
        else
        {
            individualScoreTime = 0;
            individualScore = Mathf.Max(individualScore - 10, 0);
        }
    }


    void CheckUserInput()
    {
        if (Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.LeftArrow))
        {
            moveImmediateHorizontal = false;
            
            horizontalTimer = 0;
            
            buttonDownWaitTimerH = 0;
        }
        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            moveImmediateVertical = false;
            verticalTimer = 0;
            buttonDownWaitTimerV = 0;
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            MoveRight();

        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Rotate();
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            MoveLeft();
        }
        if (Input.GetKey(KeyCode.DownArrow) || Time.time - fall >= fallSpeed)
        {
            MoveDown();
        }
            
    }

    void MoveLeft()
    {
        if (moveImmediateHorizontal)
        {
            if (buttonDownWaitTimerH < buttonDownWaitMax)
            {
                buttonDownWaitTimerH += Time.deltaTime;
                return;
            }
            if (horizontalTimer < continuousHorSpeed)
            {
                horizontalTimer += Time.deltaTime;
                return;
            }
        }
        if (!moveImmediateHorizontal)
            moveImmediateHorizontal = true;
        horizontalTimer = 0;
        transform.position += new Vector3(-blockmove, 0, 0);

        if (CheckIsValidPosition())
        {
            FindObjectOfType<Game>().UpdateGrid(this);
            PlayMoveAudio();
        }
        else
        {
            transform.position += new Vector3(blockmove, 0, 0);
        }
    }
    void MoveRight()
    {
        if (moveImmediateHorizontal)
        {
            if (buttonDownWaitTimerH < buttonDownWaitMax)
            {
                buttonDownWaitTimerH += Time.deltaTime;
                return;
            }
            if (horizontalTimer < continuousHorSpeed)
            {
                horizontalTimer += Time.deltaTime;
                return;
            }
        }

        if (!moveImmediateHorizontal)
            moveImmediateHorizontal = true;
        horizontalTimer = 0;

        transform.position += new Vector3(blockmove, 0, 0);

        if (CheckIsValidPosition())
        {
            FindObjectOfType<Game>().UpdateGrid(this);
            PlayMoveAudio();
        }
        else
        {
            transform.position += new Vector3(-blockmove, 0, 0);
        }
    }
    void MoveDown()
    {
            if (moveImmediateVertical)
            {
                if (buttonDownWaitTimerV < buttonDownWaitMax)
                {
                    buttonDownWaitTimerV += Time.deltaTime;
                    return;
                }
                if (verticalTimer < continuousVertSpeed)
                {
                    verticalTimer += Time.deltaTime;
                    return;
                }
            }
            if (!moveImmediateVertical)
                moveImmediateVertical = true;
            verticalTimer = 0;

            transform.position += new Vector3(0, -blockmove, 0);
            if (CheckIsValidPosition())
            {
                FindObjectOfType<Game>().UpdateGrid(this);

                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    PlayMoveAudio();
                }
            }
            else
            {
                transform.position += new Vector3(0, blockmove, 0);

                FindObjectOfType<Game>().DeleteRow();

                if (FindObjectOfType<Game>().CheckIsAboveGrid(this))
                {
                    FindObjectOfType<Game>().GameOver();
                }

                enabled = false;

                FindObjectOfType<Game>().SpawnNextBlock();
                PlayLandAudio();
                Game.currentScore += individualScore;
            }
            fall = Time.time;
    }
    void Rotate()
    {
        if (allowRotation)
        {
            if (limitRotation)
            {
                if (transform.rotation.eulerAngles.z >= 90)
                {
                    transform.Rotate(0, 0, -90);
                }
                else
                {
                    transform.Rotate(0, 0, 90);
                }
            }
            else
            {
                transform.Rotate(0, 0, 90);
            }
            if (CheckIsValidPosition())
            {
                FindObjectOfType<Game>().UpdateGrid(this);
                PlayRotateAudio();
            }
            else
            {
                if (limitRotation)
                {
                    if (transform.rotation.eulerAngles.z >= 90)
                    {
                        transform.Rotate(0, 0, -90);
                    }
                    else
                    {
                        transform.Rotate(0, 0, 90);
                    }
                }
                else
                {
                    transform.Rotate(0, 0, -90);
                }
            }

        }
    }

    void PlayMoveAudio()
    {
        audioSource.PlayOneShot(moveSound);
    }

    void PlayRotateAudio()
    {
        audioSource.PlayOneShot(rotateSound);
    }

    void PlayLandAudio()
    {
        audioSource.PlayOneShot(landSound);
    }

    bool CheckIsValidPosition()
    {
        foreach(Transform block in transform)
        {
            Vector2 pos = FindObjectOfType<Game>().Round(block.position);

            if (FindObjectOfType<Game>().CheckIsInsideGrid (pos) == false)
            {
                return false;
            }
            if (FindObjectOfType<Game>().GetTransformAtGridPosition(pos) != null && FindObjectOfType<Game>().GetTransformAtGridPosition(pos).parent != transform)
            {
                return false;
            }
        }

        return true;
    }
}
