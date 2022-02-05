using UnityEngine;
using TMPro;
public class ClockUI : MonoBehaviour
{
    public TextMeshProUGUI timeText, dateText, debugText;
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
    }
}
