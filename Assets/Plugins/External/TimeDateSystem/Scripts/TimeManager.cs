using UnityEngine;
using UnityEngine.Events;

namespace DPUtils.Systems.DateTime
{
    public class TimeManager : MonoBehaviour
    {
        [Header("Date & Time Settings")]
        [Range(1, 28)]
        public int dateInMonth;
        [Range(1, 4)]
        public int season;
        [Range(1, 99)]
        public int year;
        [Range(0, 24)]
        public int hour;
        [Range(0, 6)]
        public int minutes;

        public static DateTime DateTime;

        [Header("Tick Settings")]
        public int TickMinutesIncreased = 10;
        public float TimeBetweenTicks = 1;
        private float currentTimeBetweenTicks = 0;

        public static UnityAction<DateTime> OnDateTimeChanged;
        public static UnityAction<DateTime> OnNewDay;

        private void Awake()
        {
            DateTime = new DateTime(dateInMonth, season - 1, year, hour, minutes * 10);
        }

        private void Start()
        {
            OnDateTimeChanged?.Invoke(DateTime);
        }
        private void FixedUpdate()
        {
            currentTimeBetweenTicks += Time.fixedDeltaTime;

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
        [SerializeField] private int year;

        [SerializeField] private int hour;
        [SerializeField] private int minutes;

        [SerializeField] private Season season;

        private int totalNumDays;
        private int totalNumWeeks;
        #endregion

        #region Properties
        public Days Day => day;
        public int Date => date;
        public int Hour => hour;
        public int Minutes => minutes;
        public Season Season => season;
        public int Year => year;
        public int TotalNumDays => totalNumDays;
        public int TotalNumWeeks => totalNumWeeks;
        public int CurrentWeek => totalNumWeeks % 16 == 0 ? 16 : totalNumWeeks % 16;
        #endregion

        #region Constructors

        public DateTime(int date, int season, int year, int hour, int minutes)
        {
            this.day = (Days)(date % 7);
            if (day == 0) day = (Days)7;
            this.date = date;
            this.season = (Season)season;
            this.year = year;

            this.hour = hour;
            this.minutes = minutes;


            totalNumDays = date + (28 * (int)this.season) + (112 * (year - 1));

            totalNumWeeks = 1 + totalNumDays / 7;
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

            if (day > (Days)7)
            {
                day = (Days)1;
                totalNumWeeks++;
            }

            date++;

            if (date % 29 == 0)
            {
                AdvanceSeason();
                date = 1;
            }

            totalNumDays++;

        }

        private void AdvanceSeason()
        {
            if (Season == Season.Winter)
            {
                season = Season.Spring;
                AdvanceYear();
            }
            else season++;
        }

        private void AdvanceYear()
        {
            date = 1;
            year++;
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

        public bool IsWeekend()
        {
            return day > Days.Fri ? true : false;
        }

        public bool IsParticularDay(Days _day)
        {
            return day == _day;
        }
        #endregion

        #region Key Dates

        public DateTime NewYearsDay(int year)
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
        }

        #endregion

        #region Start Of Season

        public DateTime StartOfSeason(int season, int year)
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
        }

        #endregion

        #region To Strings

        public override string ToString()
        {
            return $"Date: {DateToString()} Season: {season} Time: {TimeToString()} " +
                $"\nTotal Days: {totalNumDays} | Total Weeks: {totalNumWeeks}";
        }
        public string DateToString()
        {
            var Day = day;
                return $"{Day} {Date} {Year.ToString("D2")}";
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
        Mon = 1,
        Tue = 2,
        Wed = 3,
        Thu = 4,
        Fri = 5,
        Sat = 6,
        Sun = 7
    }

    [System.Serializable]
    public enum Season
    {
        Spring = 0,
        Summer = 1,
        Autumn = 2,
        Winter = 3
    }
}


/*[System.Serializable]
public enum Months
{
    NULL = 0,
    Jan = 1,
    Feb = 2,
    Mar = 3,
    Apr = 4,
    May = 5,
    Jun = 6,
    Jul = 7,
    Aug = 8,
    Sep = 9,
    Oct = 10,
    Nov = 11,
    Dec = 12
}*/


/*public struct DateTime
{
    public Days Day;
    public Months Month;
    public int Year;

    public int Hour;
    public int Minutes;

    public int Week;

    public Season Season;

    private int weeksInYear;
    private int weeksInMonth;
    public int WeeksInYear => weeksInYear;
    public int WeeksInMonth => weeksInMonth;

    private int weeksPerSeason => weeksInYear / 4;

    public DateTime(int day, int month, int year)
    {
        Day = (Days)day;
        Month = (Months)month;
        Year = year;

        Hour = 0;
        Minutes = 0;

        Week = 1;

        Season = (Season)0;

        weeksInYear = 16;
        weeksInMonth = 1;
    }

    //
    // Summary:
    //     Shorthand for writing Vector3(1, 1, 1).
    public DateTime(int day, int month, int year, int hour, int minutes)
    {
        Day = (Days)day;
        Month = (Months)month;
        Year = year;

        Hour = hour;
        Minutes = minutes;

        Week = 1;

        Season = (Season)0;

        weeksInYear = 16;
        weeksInMonth = 1;
    }

    public DateTime(int day, int month, int year, int hour, int minutes, int week, int season, int _weeksInYear, int _weeksInMonth)
    {
        Day = (Days)day;
        Month = (Months)month;
        Year = year;

        Hour = hour;
        Minutes = minutes;

        Week = week;

        Season = (Season)season;

        weeksInYear = _weeksInYear;
        weeksInMonth = weeksInYear / 12;
    }

    public string DateToString()
    {
        return $"{Day} {Month} {Year.ToString("D2")}";
    }

    public string TimeToString()
    {
        int adjustedHour = 0;

        if (Hour == 0)
        {
            adjustedHour = 12;
        }
        else if (Hour == 24)
        {
            adjustedHour = 12;
        }
        else if (Hour >= 13)
        {
            adjustedHour = Hour - 12;
        }
        else
        {
            adjustedHour = Hour;
        }

        string AmPm = Hour == 0 || Hour < 12 ? "AM" : "PM";

        return $"{adjustedHour.ToString("D2")}:{Minutes.ToString("D2")} {AmPm}";
    }

    public void AdvanceMinutes(int SecondsToAdvanceBy)
    {
        if (Minutes + SecondsToAdvanceBy > 50)
        {
            Minutes = 0;
            AdvanceHour();
        }
        else
        {
            Minutes += SecondsToAdvanceBy;
        }
    }

    private void AdvanceHour()
    {
        if ((Hour + 1) == 24)
        {
            Hour = 0;
            AdvanceDay();
        }
        else
        {
            Hour++;
        }
    }

    private void AdvanceDay()
    {
        if (Day + 1 > (Days)7)
        {
            Day = (Days)1;
            AdvanceWeek();
        }
        else
        {
            Day++;
        }
    }

    private void AdvanceWeek()
    {
        Week++;

        if (Week % weeksPerSeason == 0)
        {
            if (Season == Season.Winter)
                Season = Season.Spring;
            else Season++;
        }

        if (Week % weeksInMonth == 0)
        {
            AdvanceMonth();
        }
    }

    private void AdvanceMonth()
    {
        if (Month + 1 > (Months)12)
        {
            Month = (Months)1;
            AdvanceYear();
        }
        else
        {
            Month++;
        }
    }

    private void AdvanceYear()
    {
        Year++;
        Week = 0;
    }
}*/