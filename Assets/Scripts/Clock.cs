using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Clock : MonoBehaviour
{
    private Text displayClock;
    public static string StringValue { get {
        Value += Time.deltaTime;
        int total = (int)Value;
        int sec = total % 60;
        int min = total / 60;

        return (min < 10 ? "0" : "") + min + ":" + (sec < 10 ? "0" : "") + sec;
        } }
    public static float Value { get; private set; }
    void Start()
    {
        Value = 0;
        displayClock = GetComponent<Text>();
    }

    void Update()
    {
        if(Menu.IsActive == false)
        {
            Value += Time.deltaTime;
            displayClock.text = StringValue;
        } 
    }
}
