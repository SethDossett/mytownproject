using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Experimental.Rendering.Universal;
using TMPro;
using MyTownProject.Core;
using MyTownProject.Enviroment;

namespace MyTownProject.UI
{
    public class ClockUI : MonoBehaviour
    {
        /*public TextMeshProUGUI timeText, dateText, debugText;
        string _suffix;
        string _am = "AM";
        string _pm = "PM";
        int _minute, _hour, _day;


        private void OnEnable()
        {
            TimeManager.OnMinuteChanged += UpdateTime;
            TimeManager.OnHourChanged += UpdateTime;
            TimeManager.OnDayChanged += UpdateTime;
        }
        private void OnDisable()
        {
            TimeManager.OnMinuteChanged -= UpdateTime;
            TimeManager.OnHourChanged -= UpdateTime;
            TimeManager.OnDayChanged -= UpdateTime;
        }
        private void UpdateTime()
        {
            _minute = TimeManager.Minute;
            _hour = TimeManager.Hour;
            _day = TimeManager.Day;

            debugText.text = $"{_hour:0}: {_minute:00}";

            _suffix = TimeManager.Hour <= 12 ? _am : _pm;
            _hour = _hour % 12 == 0 ? 12 : _hour % 12;

            timeText.text = $"{_hour:0}: {_minute:00}" + $" {_suffix}";
            dateText.text = $"{_day}";
        }*/

        public RectTransform ClockFace;
        public TextMeshProUGUI Date, Time, Season, Week;

        public Image weatherSprite;
        public Sprite[] weatherSprites;

        private float startingRotation;

        public Light sunlight;
        public float nightIntensity;
        public float dayIntensity;

        public Color dayColor;
        public Color rainyDayColor;
        public Color nightColor;

        public ParticleSystem fireflies;

        public AnimationCurve dayNightCurve;

        private void Awake()
        {
            startingRotation = ClockFace.localEulerAngles.z;
            fireflies.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }

        private void OnEnable()
        {
            TimeManager.OnDateTimeChanged += UpdateDateTime;
        }

        private void OnDisable()
        {
            TimeManager.OnDateTimeChanged -= UpdateDateTime;
        }

        private void UpdateDateTime(DateTime dateTime)
        {
            Date.text = dateTime.DateToString();
            Time.text = dateTime.TimeToString();
            //Season.text = dateTime.Season.ToString();
            //Week.text = $"WK: {dateTime.CurrentWeek}";
            weatherSprite.sprite = weatherSprites[(int)WeatherManager.currentWeather];

            float t = (float)dateTime.Hour / 24f;

            float newRotation = Mathf.Lerp(0, 360, t);
            ClockFace.localEulerAngles = new Vector3(0, 0, newRotation + startingRotation);

            float dayNightT = dayNightCurve.Evaluate(t);

            sunlight.intensity = Mathf.Lerp(dayIntensity, nightIntensity, dayNightT);

            if (WeatherManager.currentWeather == Weather.Sunny)
                sunlight.color = Color.Lerp(dayColor, nightColor, dayNightT);
            else sunlight.color = Color.Lerp(rainyDayColor, nightColor, dayNightT);

            if (dateTime.IsNight() && fireflies.isStopped)
            {
                fireflies.Play();
            }

            if (!dateTime.IsNight() && fireflies.isPlaying)
            {
                fireflies.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            }
        }







    }
}