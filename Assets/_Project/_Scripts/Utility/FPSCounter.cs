using TMPro;
using UnityEngine;


namespace MyTownProject.Utility{
public class FPSCounter : MonoBehaviour
{
    int _frameRate = 0;
    float _frameCount = 0;
    float _timer = 0;
    float _refreshRate = 1f;
    TextMeshProUGUI _text;
    [SerializeField] DebugSettingsSO _settings;

    private void OnEnable() 
    {
        _refreshRate = _settings.FPS_RefreshRate;
        _text = GetComponent<TextMeshProUGUI>();
    }
    
    private void Update()
    {
        _timer += Time.unscaledDeltaTime;

        _frameCount++;

        if (_timer >= _refreshRate)
        {
            _frameRate = (int)(_frameCount / _timer);
            _text.text = _frameRate.ToString() + " FPS";

            _timer -= _refreshRate;
            _frameCount = 0;
        }


    }
}
}