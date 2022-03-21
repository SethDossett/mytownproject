using UnityEngine;
using DPUtils.Systems.DateTime;

public class WeatherHandler : MonoBehaviour
{
    public static WeatherDanPos currentWeather = WeatherDanPos.Sunny;

    public ParticleSystem rainParticles;

    private void Awake()
    {
        rainParticles.Stop(false, ParticleSystemStopBehavior.StopEmitting);
    }

    private void OnEnable()
    {
        DanPosTimeManager.OnDateTimeChanged += GetRandomWeather;
    }

    private void OnDisable()
    {
        DanPosTimeManager.OnDateTimeChanged -= GetRandomWeather;
    }

    private void GetRandomWeather(DateTimeDanPos dateTime)
    {
        if (dateTime.Hour == 0 && dateTime.Minutes == 0)
        {
            currentWeather = (WeatherDanPos)Random.Range(0, (int)WeatherDanPos.MAX_WEATHER_AMOUNT + 1);

            if (currentWeather == WeatherDanPos.Raining)
            {
                rainParticles.Play();
            }
            else
            {
                rainParticles.Stop(false, ParticleSystemStopBehavior.StopEmitting);
            }
        }
    }
}

public enum WeatherDanPos
{
    Sunny = 0,
    Raining = 1,
    MAX_WEATHER_AMOUNT = Raining
}
