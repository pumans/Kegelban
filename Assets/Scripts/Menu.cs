using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public static MenuMode MenuMode { get; set; }
    public static int myScore;
    private Text Title;
    private Text Score;
    public static bool IsActive { get; set; }

    void Start()
    {
        Title = GameObject.Find("Title").GetComponent<Text>();
        Score = GameObject.Find("Score").GetComponent<Text>();
        IsActive = true;
        myScore = 0;
    }
    void Update()
    {
        
    }
    void LateUpdate()
    {
        switch (MenuMode)
        {
            case MenuMode.Start:
                Title.text = "Начало игры";
                break;
            case MenuMode.Pause:
                Title.text = "Пауза";
                break;
            case MenuMode.Next:
                Title.text = "Следующий фрейм";
                break;
            case MenuMode.GameOver:
                Title.text = "Конец игры";
                break;
        }
        Score.text = myScore.ToString();
    }
}

public enum MenuMode
{
    Start,
    Pause,
    Next,
    GameOver
}