using UnityEngine;
using MyTownProject.Core;

namespace MyTownProject.Enviroment
{
    public class WeatherManager : MonoBehaviour
    {
        public static Weather currentWeather = Weather.Sunny;

        public ParticleSystem rainParticles;

        private void Awake()
        {
            rainParticles.Stop(false, ParticleSystemStopBehavior.StopEmitting);
        }

        private void OnEnable()
        {
            TimeManager.OnDateTimeChanged += GetRandomWeather;
        }

        private void OnDisable()
        {
            TimeManager.OnDateTimeChanged -= GetRandomWeather;
        }

        private void GetRandomWeather(DateTime dateTime)
        {
            if (dateTime.Hour == 0 && dateTime.Minutes == 0)
            {
                currentWeather = (Weather)Random.Range(0, (int)Weather.MAX_WEATHER_AMOUNT + 1);

                if (currentWeather == Weather.Raining)
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

    public enum Weather
    {
        Sunny = 0,
        Raining = 1,
        MAX_WEATHER_AMOUNT = Raining
    }

}
