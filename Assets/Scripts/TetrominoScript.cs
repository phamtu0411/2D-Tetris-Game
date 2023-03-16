using UnityEngine;

public class TetrominoScript : MonoBehaviour
{
    float fall = 0;
    public float fallSpeed = 1;

    //rotate
    public bool allowRotation = true;
    public bool limitRotation = false;

    //sound
    public AudioClip moveSound;
    public AudioClip rotateSound;
    public AudioClip landSound;

    private AudioSource sound;

    //optimizing controller
    private float continuousVerticalSpeed = 0.05f; //tetromino speed will move when held down
    private float continuousHorizontallSpeed = 0.1f; //tetromino speed will move when held down
    private float downWaitMaxBTN = 0.2f; //time to wait before tetromino move immediate

    private float verticalTimer = 0;
    private float horizontalTimer = 0;
    private float downWaitTimerHorizontal = 0; //follow time last move immediate
    private float downWaitTimerVertical = 0;

    private bool movedImmediateHorizontal = false;
    private bool movedImmediateVertical = false;

    /*//touch movement
    public float moveSpeed = 1f; // movement speed of tetromino
    private float touchStartPosition; // start position of touch
    private Vector3 tetrominoStartPosition; // start position of tetromino*/

    public bool slammed = false;

    void Start()
    {
        sound = GetComponent<AudioSource>();
    }

    void Update()
    {
        Move();
    }

    void Move()
    {
        /*if (Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Began)
            {
                touchStartPosition = t.position.x; // save the start position of touch
                tetrominoStartPosition = transform.position; // save the start position of tetromino
            }
            else if (t.phase == TouchPhase.Moved)
            {
                float touchDelta = t.position.x - touchStartPosition; // calculate the difference in touch position
                Vector3 tetrominoPosition = tetrominoStartPosition + new Vector3(touchDelta, 0f, 0f) * moveSpeed; // calculate the new position of tetromino
                transform.position = tetrominoPosition; // update the position of tetromino
            }
        }*/

        //reset these variables,
        //the previous movement will be deleted before the player makes a new movement
        if (Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.LeftArrow))
        {
            //check move immediate 
            movedImmediateHorizontal = false;
            //These variables follow time before started moving
            horizontalTimer = 0;
            downWaitTimerHorizontal = 0;
        }

        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            //check move immediate 
            movedImmediateVertical = false;
            //These variables follow time before started moving
            verticalTimer = 0;
            downWaitTimerVertical = 0;
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            MoveRight();
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            MoveLeft();
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Rotate();
        }
        else if (Input.GetKey(KeyCode.DownArrow) || Time.time - fall >= fallSpeed) //last move time  >= current fallSpeed
        {
            MoveDown();
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            SlamDown();
        }
    }

    bool IsValidPosition()
    {
        foreach (Transform tetromino in transform)
        {
            Vector2 pos = FindObjectOfType<GameplayScript>().Round(tetromino.position);
            if (FindObjectOfType<GameplayScript>().IsInsideGrid(pos) == false)
            {
                return false;
            }

            if (FindObjectOfType<GameplayScript>().GetTransformGridPosition(pos) != null && FindObjectOfType<GameplayScript>().GetTransformGridPosition(pos).parent != transform)
            {
                return false;
            }
        }
        return true;
    }

    void PlayMoveSound()
    {
        sound.PlayOneShot(moveSound);
    }

    void PlayRotateSound()
    {
        sound.PlayOneShot(rotateSound);
    }

    void PlayLandSound()
    {
        sound.PlayOneShot(landSound);
    }

    void MoveLeft()
    {
        //check moved immediate horizontal (GetKeyDown not Getkey)
        if (movedImmediateHorizontal)
        {
            //check time last move horizontal
            //player hasn't waited enough time since last move => func ends without moving tetromino
            if (downWaitTimerHorizontal < downWaitMaxBTN)
            {
                downWaitTimerHorizontal += Time.deltaTime;
                return;
            }

            //check can move horizontal at current speed
            //not enough time => func ends without moving tetromino
            if (horizontalTimer < continuousHorizontallSpeed)
            {
                horizontalTimer += Time.deltaTime;
                return;
            }
        }

        if (!movedImmediateHorizontal)
        {
            movedImmediateHorizontal = true;
        }

        horizontalTimer = 0;

        transform.position += new Vector3(-1, 0, 0);
        if (IsValidPosition())
        {
            FindObjectOfType<GameplayScript>().UpdateGrid(this);
            PlayMoveSound();
        }
        else
        {
            transform.position += new Vector3(1, 0, 0);
        }
    }

    void MoveRight()
    {
        //check moved immediate horizontal (GetKeyDown not Getkey)
        if (movedImmediateHorizontal)
        {
            //check time last move horizontal
            //player hasn't waited enough time since last move => func ends without moving tetromino
            if (downWaitTimerHorizontal < downWaitMaxBTN)
            {
                downWaitTimerHorizontal += Time.deltaTime;
                return;
            }

            //check can move horizontal at current speed
            //not enough time => func ends without moving tetromino
            if (horizontalTimer < continuousHorizontallSpeed)
            {
                horizontalTimer += Time.deltaTime;
                return;
            }
        }

        if (!movedImmediateHorizontal)
        {
            movedImmediateHorizontal = true;
        }

        horizontalTimer = 0;

        transform.position += new Vector3(1, 0, 0);
        if (IsValidPosition())
        {
            FindObjectOfType<GameplayScript>().UpdateGrid(this);
            PlayMoveSound();
        }
        else
        {
            transform.position += new Vector3(-1, 0, 0);
        }
    }

    void MoveDown()
    {
        if (slammed)
        {
            slammed = false;
            return;
        }
        //check moved immediate vertical (GetKeyDown not Getkey)
        if (movedImmediateVertical)
        {
            //check time last move vertical
            //player hasn't waited enough time since last move => func ends without moving tetromino
            if (downWaitTimerVertical < downWaitMaxBTN)
            {
                downWaitTimerVertical += Time.deltaTime;
                return;
            }

            //check can move horizontal at current speed
            //not enough time => func ends without moving tetromino
            if (verticalTimer < continuousVerticalSpeed)
            {
                verticalTimer += Time.deltaTime;
                return;
            }
        }

        if (!movedImmediateVertical)
        {
            movedImmediateVertical = true;
        }
        verticalTimer = 0;

        transform.position += new Vector3(0, -1, 0);
        if (IsValidPosition())
        {
            FindObjectOfType<GameplayScript>().UpdateGrid(this);
            PlayMoveSound();
        }
        else
        {
            transform.position += new Vector3(0, 1, 0);

            FindObjectOfType<GameplayScript>().DestroyRow();

            if (FindObjectOfType<GameplayScript>().IsAboveGrid(this))
            {
                FindObjectOfType<GameplayScript>().GameOver();
            }

            enabled = false;
            PlayLandSound();
            FindObjectOfType<GameplayScript>().SpawnTetromino();
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

            if (IsValidPosition())
            {
                FindObjectOfType<GameplayScript>().UpdateGrid(this);
                PlayRotateSound();
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

    public void SlamDown()
    {
        slammed = true;
        while (IsValidPosition())
        {
            transform.position += new Vector3(0, -1, 0);
        }
        if (!IsValidPosition())
        {
            transform.position += new Vector3(0, 1, 0);
            FindObjectOfType<GameplayScript>().UpdateGrid(this);
        }
    }
}