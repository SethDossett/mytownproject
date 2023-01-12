using UnityEngine;
using UnityEngine.Events;

namespace MyTownProject.Core
{
    public class TimeManager : MonoBehaviour
    {
        /*public static Action OnMinuteChanged, OnHourChanged, OnDayChanged;

        public static int Minute { get; private set; }
        public static int Hour { get; private set; }
        public static int Day { get; private set; }

        [SerializeField] private float timeToTick = 0.5f;
        private float timer;

        void Start()
        {
            //Application.targetFrameRate = 60;
            Minute = 0;
            Hour = 6;
            Day = 1;
            timer = timeToTick;
        }

        private void FixedUpdate()
        {
            timer -= Time.fixedDeltaTime;
        }

        void Update()
        {


            if (timer <= 0)
            {
                Minute++;
                OnMinuteChanged?.Invoke();
                if (Minute >= 60)
                {
                    Minute = 0;
                    Hour++;
                    OnHourChanged?.Invoke();
                    if (Hour >= 24)
                    {
                        Hour = 0;
                        Day++;
                        OnDayChanged?.Invoke();
                    }

                }
                timer = timeToTick;
            }
        }*/


        [Header("Date & Time Settings")]
        [Range(1, 3)]
        public int currentDay;
        [Range(1, 4)]
        public int season;
        [Range(1, 99)]
        public int year;
        [Range(0, 24)]
        public int hour;
        [Range(0, 60)]
        public int minutes;

        public static DateTime DateTime;
        public static float GlobalTime;

        [Header("Tick Settings")]
        public int TickMinutesIncreased = 1;
        public float TimeBetweenTicks = 1;
        private float currentTimeBetweenTicks = 0;

        public static UnityAction<DateTime> OnDateTimeChanged;
        public static UnityAction<DateTime> OnNewDay;

        private void Awake()
        {
            DateTime = new DateTime(currentDay, hour, minutes);

            //Global Time is going to be set to whatever the saved time was,
            // if no saved file Global Time = 0.
            GlobalTime = 0;
        }

        private void Start()
        {
            OnDateTimeChanged?.Invoke(DateTime);
        }
        private void FixedUpdate()
        {
            currentTimeBetweenTicks += Time.fixedDeltaTime;
            GlobalTime += Time.fixedDeltaTime;
        }
        private void Update()
        {
            if (currentTimeBetweenTicks >= TimeBetweenTicks)
            {
                currentTimeBetweenTicks = 0;
                Tick();
            }
        }

        void Tick()
        {
            AdvanceTime();
        }

        void AdvanceTime()
        {

            DateTime.AdvanceMinutes(TickMinutesIncreased);

            OnDateTimeChanged?.Invoke(DateTime);

        }

    }


    [System.Serializable]
    public struct DateTime
    {
        #region Fields
        private Days day;
        [SerializeField] private int date;
        [SerializeField] private int hour;
        [SerializeField] private int minutes;

        #endregion

        #region Properties
        public Days Day { get { return day; } set { day = value; } }
        public int Date => date;
        public int Hour => hour;
        public int Minutes => minutes;
        #endregion

        #region Constructors

        public DateTime(int date, int hour, int minutes)
        {
            this.day = (Days)(date % 3);
            if (this.day == 0) day = (Days)3;
            this.date = date;
            this.hour = hour;
            this.minutes = minutes;
        }

        #endregion

        #region Time Advancement

        public void AdvanceMinutes(int SecondsToAdvanceBy)
        {
            if (minutes + SecondsToAdvanceBy >= 60)
            {
                minutes = (minutes + SecondsToAdvanceBy) % 60;
                AdvanceHour();
            }
            else
            {
                minutes += SecondsToAdvanceBy;
            }
        }

        private void AdvanceHour()
        {
            if ((hour + 1) == 24)
            {
                hour = 0;
                AdvanceDay();
            }
            else
            {
                hour++;
            }
        }

        private void AdvanceDay()
        {
            day++;

            if (day > (Days)3)
            {
                day = (Days)1; // moon crash lol
            }

        }



        #endregion

        #region Bool Checks
        public bool IsNight()
        {
            return hour > 18 || hour < 6;
        }

        public bool IsMorning()
        {
            return hour >= 6 && hour <= 12;
        }

        public bool IsAfternoon()
        {
            return hour > 12 && hour < 18;
        }

        public bool IsParticularDay(Days _day)
        {
            return day == _day;
        }
        #endregion

        #region Key Dates

        /*public DateTime NewYearsDay(int year)
        {
            if (year == 0) year = 1;
            return new DateTime(1, 0, year, 6, 0);
        }

        public DateTime SummerSolstice(int year)
        {
            if (year == 0) year = 1;
            return new DateTime(28, 1, year, 6, 0);
        }
        public DateTime PumpkinHarvest(int year)
        {
            if (year == 0) year = 1;
            return new DateTime(28, 2, year, 6, 0);
        }*/

        #endregion

        #region Start Of Season

        /*public DateTime StartOfSeason(int season, int year)
        {
            season = Mathf.Clamp(season, 0, 3);
            if (year == 0) year = 1;

            return new DateTime(1, season, year, 6, 0);
        }

        public DateTime StartOfSpring(int year)
        {
            return StartOfSeason(0, year);
        }

        public DateTime StartOfSummer(int year)
        {
            return StartOfSeason(1, year);
        }

        public DateTime StartOfAutumn(int year)
        {
            return StartOfSeason(2, year);
        }

        public DateTime StartOfWinter(int year)
        {
            return StartOfSeason(3, year);
        }*/

        #endregion

        #region To Strings

        public override string ToString()
        {
            return $"Date: {day} Time: {TimeToString()} ";
        }

        public string DateToString()
        {
            var Day = day;
            return $"{Day}";
        }

        public string TimeToString()
        {
            int adjustedHour = 0;

            if (hour == 0)
            {
                adjustedHour = 12;
            }
            else if (hour >= 13)
            {
                adjustedHour = hour - 12;
            }
            else
            {
                adjustedHour = hour;
            }

            string AmPm = hour < 12 ? "AM" : "PM";

            return $"{adjustedHour.ToString("D2")}:{minutes.ToString("D2")} {AmPm}";
        }

        #endregion
    }

    [System.Serializable]
    public enum Days
    {
        NULL = 0,
        FIRST = 1,
        SECOND = 2,
        THIRD = 3,

    }
}