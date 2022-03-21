using MyTownProject.Core;
using UnityEngine;

namespace MyTownProject.Events
{
    public class TimedEventMaster : MonoBehaviour
    {
        public delegate void TimedEventHandler();
        public event TimedEventHandler sixEvent;
        public event TimedEventHandler sixThirtyEvent;
        public event TimedEventHandler sevenEvent;
        public event TimedEventHandler sevenThirtyEvent;
        public event TimedEventHandler eightEvent;


        void OnEnable()
        {
            TimeManager.OnDateTimeChanged += CheckTimeForEvent;
        }
        void OnDisable()
        {
            TimeManager.OnDateTimeChanged -= CheckTimeForEvent;
        }

        private void CheckTimeForEvent(DateTime dateTime)
        {
            int hour = dateTime.Hour;
            int minute = dateTime.Minutes;
            Days day = dateTime.Day;


            if(hour == 6 && minute == 0)
            {
                sixEvent?.Invoke();
                Debug.Log("6:00");
            }
            if (hour == 6 && minute == 30)
            {
                sixThirtyEvent?.Invoke();
                Debug.Log("6:30");
            }
            if (hour == 6 && minute == 60)
            {
                sevenEvent?.Invoke();
                Debug.Log("7:00");
            }
            if (hour == 7 && minute == 30)
            {
                sevenThirtyEvent?.Invoke();
                Debug.Log("7:30");
            }
            if (hour == 7 && minute == 60)
            {
                eightEvent?.Invoke();
                Debug.Log("8:00");
            }

        }
    }
}