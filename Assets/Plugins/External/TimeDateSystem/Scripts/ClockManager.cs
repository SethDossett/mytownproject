using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.UI;
using TMPro;
using DPUtils.Systems.DateTime;

public class ClockManager : MonoBehaviour
{
    public RectTransform ClockFace;
    public TextMeshProUGUI Date, Time, Season, Week;

    public Image weatherSprite;
    public Sprite[] weatherSprites;

    private float startingRotation;

    public Light2D sunlight;
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
        Season.text = dateTime.Season.ToString();
        Week.text = $"WK: {dateTime.CurrentWeek}";
        weatherSprite.sprite = weatherSprites[(int)WeatherManager.currentWeather];

        float t = (float)dateTime.Hour / 24f;

        float newRotation = Mathf.Lerp(0, 360, t);
        ClockFace.localEulerAngles = new Vector3(0, 0, newRotation + startingRotation);

        float dayNightT = dayNightCurve.Evaluate(t);

        sunlight.intensity = Mathf.Lerp(dayIntensity, nightIntensity, dayNightT);

        if(WeatherManager.currentWeather == Weather.Sunny)
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
