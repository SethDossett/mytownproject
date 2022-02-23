using MyTownProject.Core;
using UnityEngine;

namespace MyTownProject.Events
{
    public class TimedEventMaster : MonoBehaviour
    {
        public delegate void TimedEventHandler();
        public event TimedEventHandler sixThirtyEvent;
        public event TimedEventHandler sevenEvent;
        public event TimedEventHandler sevenThirtyEvent;
        public event TimedEventHandler eightEvent;


        void OnEnable()
        {
            TimeManager.OnMinuteChanged += CheckTimeForEvent;
        }
        void OnDisable()
        {
            TimeManager.OnMinuteChanged -= CheckTimeForEvent;
        }

        private void CheckTimeForEvent()
        {
            int hour = TimeManager.Hour;
            int minute = TimeManager.Minute;
            int day = TimeManager.Day;

            if (hour == 6 && minute == 30)
            {
                sixThirtyEvent?.Invoke();
                Debug.Log("630");
            }
            if (hour == 6 && minute == 60)
            {
                sevenEvent?.Invoke();
                //GameManager.instance.UpdateState(GameManager.GameState.CUTSCENE);
                Debug.Log("700");
            }
            if (hour == 7 && minute == 30)
            {
                sevenThirtyEvent?.Invoke();
                Debug.Log("730");
            }
            if (hour == 7 && minute == 60)
            {
                eightEvent?.Invoke();
                Debug.Log("800");
            }

        }
    }
}