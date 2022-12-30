using UnityEngine;

namespace MyTownProject.Utility
{

public class DebugOptions : MonoBehaviour
{
    public static DebugOptions Instance;
    [SerializeField] DebugSettingsSO _settings;
    FPSCounter _fpsCounter;

    private void Awake() {
        if(Instance != null){
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        SetInitialReferences();
        CheckFrameRate();
    }
    void SetInitialReferences(){
        _fpsCounter = GetComponentInChildren<FPSCounter>();

        if(_settings.SetFPS_Counter){
            _fpsCounter.enabled = true;
        }
        else{
            _fpsCounter.enabled = false;
        }
    }
    void CheckFrameRate()
        {
            if (_settings.SetFrameRate)
                Application.targetFrameRate = _settings.TargetFrameRate;
            else
                Application.targetFrameRate = -1;
        }
}
}
