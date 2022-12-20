using UnityEngine;

public class DebugOptions : MonoBehaviour
{
    public static DebugOptions Instance;

    [SerializeField] bool setFrameRate;
        [SerializeField] int targetFrameRate;
    private void Awake() {
        if(Instance != null){
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        CheckFrameRate();
    }
    void CheckFrameRate()
        {
            if (setFrameRate)
                Application.targetFrameRate = targetFrameRate;
            else
                Application.targetFrameRate = -1;
        }
}
