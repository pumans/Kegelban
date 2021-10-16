using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; //Text, Image
using System.Xml.Serialization;
using System.IO;

public class Controls : MonoBehaviour
{
    // Start is called before the first frame update
    private GameObject Ball;
    private Rigidbody ballRigidbody;
    private Vector3 ballStartPosition;
    private bool isBallMoving;
    private GameObject Arrow;
    private GameObject ArrowTail;
    private float arrowAngle;   // ���� ��������
    private GameObject Wall;

    private Text throwCount;
    private int makeThrowCount;
    private Text hitKegels;
    private Text leftKegels;
    private int leftKegelsCount;

    private GameObject Indicator;
    private Image ForceIndicator;
    // 1000 - ������� ������ ����
    private const float MIN_FORCE = 1000f;
    // 2000 - ��������� ������� ������
    private const float MAX_FORCE = 2000f;

    // Menu
    private GameObject GameMenu;
    public Game game;
    private const string SAVED_GAME = "game.xml"; 
    
    void Start()
    {
        GameMenu = GameObject.Find("Menu");
        Menu.MenuMode = MenuMode.Start;
        Arrow = GameObject.Find("Arrow");
        ArrowTail = GameObject.Find("Arrow Tail");
        // ������� ���
        Ball = GameObject.Find("Ball");     // �� ����� � ��������
        ballStartPosition = Ball.transform.position;
        isBallMoving = false;
        // �������� ��� - ��������� ���� � ��� �������� ����
        ballRigidbody = Ball.GetComponent<Rigidbody>();
        arrowAngle = 0f;
        Wall = GameObject.Find("WallRollBack");     // �� ����� � ��������
        Wall.SetActive(false);

        // ���������� �������
        throwCount = GameObject.Find("ThrowCount").GetComponent<Text>();
        makeThrowCount = 0;
        // ���������� ���������� ������
        leftKegels = GameObject.Find("Left").GetComponent<Text>();
        leftKegelsCount = 10;
        // ���������� ������ ������
        hitKegels = GameObject.Find("Hit").GetComponent<Text>();

        //��������� ���� �����
        ForceIndicator = GameObject.Find("ForseIndicator").GetComponent<Image>();
        Indicator = GameObject.Find("Indicator");

        LoadGame();
        Debug.Log(game.getScore());
        Menu.myScore = game.getScore();
    }

    // Update is called once per frame
    void Update()
    {
        // ���� ���� �������
        if (Menu.IsActive) return;
        #region ��������� ����
        if(Ball.transform.position.z > 18)
        {
            Wall.SetActive(true);
        }
        if (ballRigidbody.velocity.magnitude < 0.1f && isBallMoving)
        {
            //ballRigidbody.position.z > 19;
            isBallMoving = false;
            Arrow.SetActive(true);
            Indicator.SetActive(true);
            Wall.SetActive(false);
            Ball.transform.position = ballStartPosition;
            ballRigidbody.velocity = Vector3.zero;
            ballRigidbody.angularVelocity = Vector3.zero;
            // �������� ���������� � ������
            int broke = 0;
            foreach(GameObject kegel in GameObject.FindGameObjectsWithTag("Kegel"))
            {
                //Debug.Log(kegel.transform.position);
                //
                
                if (kegel.transform.position.y > 0.2 || Mathf.Abs(kegel.transform.rotation.x) > 0.01 
                    || Mathf.Abs(kegel.transform.rotation.z) > 0.01 )
                {
                    //Debug.Log(kegel.transform.position);
                    kegel.SetActive(false);
                    leftKegelsCount--;
                    leftKegels.text = leftKegelsCount.ToString();
                    hitKegels.text = (10 - leftKegelsCount).ToString();
                    broke++;
                }
            }
            //Debug.Log(leftKegelsCount);
            //Debug.Log(10-leftKegelsCount);
            game.makeAttempt(broke);
            
            if(leftKegelsCount == 0 || makeThrowCount == 2)
            {
                // ��������� � xml ��������
                using (StreamWriter writer = new StreamWriter(SAVED_GAME))
                {
                    XmlSerializer serializer = new XmlSerializer(
                        game.GetType());
                    serializer.Serialize(writer, game);
                }
                SceneManager.LoadScene("SampleScene");                
                if (game.isGameOver())
                {
                    Menu.MenuMode = MenuMode.GameOver; 
                } else
                {
                    Menu.MenuMode = MenuMode.Next;
                }
                Menu.myScore = game.getScore();
                GameMenu.SetActive(false);
                Menu.MenuMode = MenuMode.Next;
                Menu.IsActive = false;
            }
        }
        #endregion
        if (Input.GetKeyDown(KeyCode.Space) && ! isBallMoving)
        {
            Arrow.SetActive(false);
            Indicator.SetActive(false);
            // ����������� ����
            Vector3 forceDirection =  Arrow.transform.forward;
            
            // �������� ����
            float forceValue = MIN_FORCE + (MAX_FORCE-MIN_FORCE) * ForceIndicator.fillAmount;
            ballRigidbody.AddForce(forceValue * forceDirection);
            ballRigidbody.velocity = forceDirection * 0.1f;
            isBallMoving = true;
            makeThrowCount++;
            throwCount.text = makeThrowCount.ToString();
        }

        #region �������� �������
        if (Input.GetKeyDown(KeyCode.LeftArrow) && arrowAngle > -30f)
        {
            if(arrowAngle > -30f)
            {
                Arrow.transform.RotateAround(
                    ArrowTail.transform.position,
                    Vector3.up,
                    -1
                );
                arrowAngle += -1;
            }
        }
        if (Input.GetKeyDown( KeyCode.RightArrow) && arrowAngle < 30f)
        {
            Arrow.transform.RotateAround(
                ArrowTail.transform.position,
                Vector3.up,
                1
            );
            arrowAngle += 1;
        }
        #endregion
        #region ��������� ����
        if (!isBallMoving)
        {
        if (Input.GetKey(KeyCode.UpArrow))
        {
                float val = ForceIndicator.fillAmount + Time.deltaTime / 2;
                
                //   ForceIndicator.fillAmount += 0.01f;
                if(val <=1)
                {
                    ForceIndicator.fillAmount = val;
                }
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
                float val = ForceIndicator.fillAmount - Time.deltaTime / 2;

                //   ForceIndicator.fillAmount -= 0.01f;
                if (val >= .1f)
                {
                    ForceIndicator.fillAmount = val;
                }
            }
        }

        #endregion

        #region �����
        if (Input.GetKeyDown(KeyCode.Pause) || Input.GetKey(KeyCode.P))
        {
            GameMenu.SetActive(true);
            Menu.MenuMode = MenuMode.Pause;
            Menu.myScore = game.getScore();
            Menu.IsActive = true;
        }
        #endregion
    }
    /**
     * ���������� ������ Play ����
     */
    public void PlayClick()
    {
        GameMenu.SetActive(false);
        Menu.IsActive = false;
    }
    public void LoadGame()
    {
        if (File.Exists(SAVED_GAME))
        {
            using (StreamReader reader = new StreamReader(SAVED_GAME))
            {
                XmlSerializer serializer = new XmlSerializer( typeof(Game) );
                game = (Game)serializer.Deserialize(reader);
            }
            //Debug.Log(game.getScore().ToString()); 
        }
        else
        {
            game = new Game();
        }
        
    }
}


public class Game
{
    private int Score;
    [SerializeField]
    public List<Frame> game ;
    public int FrameCounter;
    private int AttemptCounter;
    private Frame frame;
    public Game()
    {
        FrameCounter = AttemptCounter = 0;
        game = new List<Frame>(10);
    }
    public bool isGameOver()
    {
        if (FrameCounter < 11)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    public int getScore()
    {
        Score = 0;
        foreach (Frame frame in game)
        {
            Score += (frame.attempt1 + frame.attempt2 + frame.attempt3);
        }
        Debug.Log(Score);
        return Score;
    }
    public void makeAttempt(int score)
    {
        switch (AttemptCounter)
        {
            case 0:                         // ���� ������ ������ �� ������
                frame = new Frame();
                FrameCounter++;
                frame.FrameNumber = FrameCounter;
                frame.attempt1 = score;
                if (score == 10)            // ���� ����� 10 ������ ������ ������� ������
                {
                    frame.isStrike = true;
                }
                game.Add(frame);            //��������� ����� � ������
                break;
            case 1:                         // ���� ������ ������ �� ������
                game[FrameCounter-1].attempt2 = score;
                if( (game[FrameCounter - 1].attempt1 + game[FrameCounter - 1].attempt2) == 10) // ������� ��� �����
                {
                    game[FrameCounter - 1].isSpare = true;      // ������ ������� �����
                }
                break;
            case 2:                         // ���� ������ ������ � 10-� ������
                game[FrameCounter - 1].attempt3 = score;
                FrameCounter++;
                break;
            default:
                break;
        }
        Debug.Log(game[FrameCounter - 1].attempt1);
        Debug.Log(game[FrameCounter - 1].attempt2);
        Counter(score);                     // ����������� �������
        // ���������� ����� � ���������� �������, ���� ��� ��� ������ ��� �����
        if (FrameCounter > 1)                // ���� ������� ������ 1-�� ������
        {
            if ( game[FrameCounter - 2].isStrike) // ���� ���������� ����� ������
            {
                // ���� ������ ������� ����������� ������ ����������� �� ���-�� ������ ������
                game[FrameCounter - 2].attempt1 += score;
                // ���� ������� ������ ���� �������
                if (FrameCounter > 2)
                {
                    if (game[FrameCounter - 3].isStrike) // ���� ����� ���� ����� ����� ������
                    {
                        // ���� ������ ������� ����� ���� ����� ����� ����������� �� ���-�� ������ ������
                        game[FrameCounter - 3].attempt1 += score;
                    }
                }
            } else if(game[FrameCounter - 2].isSpare) // ���� ���������� ����� �����
            {
                // ���� ������� � ������� ������ ������
                if(AttemptCounter == 0)
                {
                    // ���� ������ ������� ����������� ������ ����������� �� ���-�� ������ ������
                    game[FrameCounter - 2].attempt2 += score;
                }
            }
        } 
    }
    // ������� �����
    private void Counter(int score)
    {
        if (FrameCounter < 10 && AttemptCounter == 0) // ���� ������ ������� � ������ (����� ���������� ������)
        {
            if (score != 10)            // ���� �� ������ ��������� ������ �������
            {
                AttemptCounter++;
            }
            if(AttemptCounter >= 2)     // ���� ������� ��� �������
            {
                AttemptCounter = 0;     // ���������� ������� ��� �������� � ������ ������
            }
        }
        else if (FrameCounter == 10)    // � ��������� ������
        {
            if (AttemptCounter == 0)    // ���� � ������ ������� 
            {
                if (score == 10)            // ������, �� ������ ������� 
                {
                    frame.isStrike = true; 
                }
                AttemptCounter++;
            } else if(AttemptCounter == 1) // ���� �� ������ ������� 
            {
                // ��� ������ ��� ������� ����, ��������� �������
                if (game[FrameCounter - 1].isStrike || game[FrameCounter - 1].isSpare) 
                {
                    AttemptCounter++;
                }
                else
                {
                    FrameCounter++;
                }
            }

        }
    }
}
public class Frame
{
    public Frame()
    {
        isStrike = isSpare = false;
        FrameNumber = attempt1 = attempt2 = attempt3 = 0;
    }
    public int FrameNumber { get; set; }
    public int attempt1 { get; set; }
    public int attempt2 { get; set; }
    public int attempt3 { get; set; }
    public bool isStrike { get; set; }
    public bool isSpare { get; set; }

}