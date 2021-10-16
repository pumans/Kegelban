using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; //Text, Image

public class Controls : MonoBehaviour
{
    // Start is called before the first frame update
    private GameObject Ball;
    private Rigidbody ballRigidbody;
    private Vector3 ballStartPosition;
    private bool isBallMoving;
    private GameObject Arrow;
    private GameObject ArrowTail;
    private float arrowAngle;   // угол поворота
    private GameObject Wall;

    private Text throwCount;
    private int makeThrowCount;
    private Text hitKegels;
    private Text leftKegels;
    private int leftKegelsCount;

    private GameObject Indicator;
    private Image ForceIndicator;
    // 1000 - сбивает только одну
    private const float MIN_FORCE = 1000f;
    // 2000 - визуально сильный бросок
    private const float MAX_FORCE = 2000f;

    // Menu
    private GameObject GameMenu;
    public Game game;
    
    void Start()
    {
        GameMenu = GameObject.Find("Menu");
        Menu.MenuMode = MenuMode.Start;
        Arrow = GameObject.Find("Arrow");
        ArrowTail = GameObject.Find("Arrow Tail");
        // находим шар
        Ball = GameObject.Find("Ball");     // по имени в иерархии
        ballStartPosition = Ball.transform.position;
        isBallMoving = false;
        // толкнуть шар - приложить силу к его твердому телу
        ballRigidbody = Ball.GetComponent<Rigidbody>();
        arrowAngle = 0f;
        Wall = GameObject.Find("WallRollBack");     // по имени в иерархии
        Wall.SetActive(false);

        // количество бросков
        throwCount = GameObject.Find("ThrowCount").GetComponent<Text>();
        makeThrowCount = 0;
        // количество оставшихся кегель
        leftKegels = GameObject.Find("Left").GetComponent<Text>();
        leftKegelsCount = 10;
        // количество сбитых кегель
        hitKegels = GameObject.Find("Hit").GetComponent<Text>();

        //индикатор силы удара
        ForceIndicator = GameObject.Find("ForseIndicator").GetComponent<Image>();
        Indicator = GameObject.Find("Indicator");

        game = new Game();
    }

    // Update is called once per frame
    void Update()
    {
        // Если меню активно
        if (Menu.IsActive) return;
        #region Остановка шара
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
            // собираем информацию о кеглях

            foreach(GameObject kegel in GameObject.FindGameObjectsWithTag("Kegel"))
            {
                Debug.Log(kegel.transform.position);
                //
                if (kegel.transform.position.y > 0.2 || Mathf.Abs(kegel.transform.rotation.x) > 0.01 
                    || Mathf.Abs(kegel.transform.rotation.z) > 0.01 )
                {
                    //Debug.Log(kegel.transform.position);
                    kegel.SetActive(false);
                    leftKegelsCount--;
                    leftKegels.text = leftKegelsCount.ToString();
                    hitKegels.text = (10 - leftKegelsCount).ToString();
                }
            }
            game.makeAttempt(10 - leftKegelsCount);
            if(leftKegelsCount == 0 || makeThrowCount == 2)
            {
                //SceneManager.LoadScene("SampleScene");
                foreach (GameObject kegel in GameObject.FindGameObjectsWithTag("Kegel"))
                {
                    for(int i = 1; i < 11; i++)
                    {
                        //kegel.transform.position.Set()
                    }
                    //kegel.transform.position.Set()
                }
                    if (game.isGameOver())
                {
                    Menu.MenuMode = MenuMode.GameOver; 
                } else
                {
                    Menu.MenuMode = MenuMode.Next;
                }
                Menu.myScore = game.getScore();
                Debug.Log( game.getScore().ToString() );
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
            // направление силы
            Vector3 forceDirection =  Arrow.transform.forward;
            
            // величина силы
            float forceValue = MIN_FORCE + (MAX_FORCE-MIN_FORCE) * ForceIndicator.fillAmount;
            ballRigidbody.AddForce(forceValue * forceDirection);
            ballRigidbody.velocity = forceDirection * 0.1f;
            isBallMoving = true;
            makeThrowCount++;
            throwCount.text = makeThrowCount.ToString();
        }

        #region Вращение стрелки
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
        #region Индикатор силы
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

        #region Пауза
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
     * обработчик кнопки Play меню
     */
    public void PlayClick()
    {
        GameMenu.SetActive(false);
        Menu.IsActive = false;
    }
}


public class Game
{
    private int Score;
    private List<Frame> game ;
    private int FrameCounter;
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
        return Score;
    }
    public void makeAttempt(int score)
    {
        switch (AttemptCounter)
        {
            case 0:                         // если первый бросок во фрейме
                frame = new Frame();
                FrameCounter++;
                frame.FrameNumber = FrameCounter;
                frame.attempt1 = score;
                if (score == 10)            // если сбито 10 кеглей ставим отметку страйк
                {
                    frame.isStrike = true;
                }
                game.Add(frame);            //добавляем фрейм в список
                break;
            case 1:                         // если второй бросок во фрейме
                game[FrameCounter-1].attempt2 = score;
                if( (game[FrameCounter - 1].attempt1 + game[FrameCounter - 1].attempt2) == 10) // сбивает все кегли
                {
                    game[FrameCounter - 1].isSpare = true;      // ставим отметку спаре
                }
                break;
            case 2:                         // если третий бросок в 10-м фрейме
                game[FrameCounter - 1].attempt3 = score;
                FrameCounter++;
                break;
            default:
                break;
        }
        Counter(score);                     // увеличиваем счетчик
        // увеличение счета в предыдущих фреймах, если там был страйк или спаре
        if (FrameCounter > 1)                // если сыграно больше 1-го фрейма
        {
            if ( game[FrameCounter - 2].isStrike) // если предыдущий фрейм страйк
            {
                // счет первой попытки предыдущего фрейма увеличиваем на кол-во сбитых кеглей
                game[FrameCounter - 2].attempt1 += score;
                // если сыграно больше двух фреймов
                if (FrameCounter > 2)
                {
                    if (game[FrameCounter - 3].isStrike) // если через один фрейм назад страйк
                    {
                        // счет первой попытки через один фрейм назад увеличиваем на кол-во сбитых кеглей
                        game[FrameCounter - 3].attempt1 += score;
                    }
                }
            } else if(game[FrameCounter - 2].isSpare) // если предыдущий фрейм спаре
            {
                // если попытка в текущем фрейме первая
                if(AttemptCounter == 0)
                {
                    // счет второй попытки предыдущего фрейма увеличиваем на кол-во сбитых кеглей
                    game[FrameCounter - 2].attempt2 += score;
                }
            }
        } 
    }
    // счетчик ходов
    private void Counter(int score)
    {
        if (FrameCounter < 10 && AttemptCounter == 0) // если первая попытка в фрейме (кроме последнего фрейма)
        {
            if (score != 10)            // если не страйк разрешаем вторую попытку
            {
                AttemptCounter++;
            }
            if(AttemptCounter >= 2)     // если сделали две попытки
            {
                AttemptCounter = 0;     // сбрасываем счетчик для перехода к новому фрейму
            }
        }
        else if (FrameCounter == 10)    // в последнем фрейме
        {
            if (AttemptCounter == 0)    // если в первой попытке 
            {
                if (score == 10)            // страйк, то ставим отметку 
                {
                    frame.isStrike = true; 
                }
                AttemptCounter++;
            } else if(AttemptCounter == 1) // если во второй попытке 
            {
                // был страйк или сделали спар, добавляем попытку
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