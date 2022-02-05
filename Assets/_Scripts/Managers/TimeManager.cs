using UnityEngine;
using System;

public class TimeManager : MonoBehaviour
{
    public static Action OnMinuteChanged, OnHourChanged, OnDayChanged;

    public static int Minute { get; private set; }
    public static int Hour { get; private set; }
    public static int Day { get; private set; }

    [SerializeField] private float minuteToRealTime = 0.5f;
    private float timer;

    void Start()
    {
        Minute = 0;
        Hour = 6;
        Day = 1;
        timer = minuteToRealTime;
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            Minute++;
            OnMinuteChanged?.Invoke();
            if(Minute >= 60)
            {
                Minute = 0;
                Hour++;
                OnHourChanged?.Invoke();
                if(Hour >= 24)
                {
                    Hour = 0;
                    Day++;
                    OnDayChanged?.Invoke();
                }
                
            }
            timer = minuteToRealTime;
        }
    }
}
