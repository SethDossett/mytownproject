using System.Linq;
using UnityEditor;
using UnityEngine;
using MyTownProject.SO;
using MyTownProject.Events;
using MyTownProject.NPC;


public class GameData : MonoBehaviour
{
    [SerializeField] SceneSO[] _allScenes;
    [SerializeField] ActionSO[] _allActions;
    [SerializeField] NPC_ScriptableObject[] _allNPCs;
    [SerializeField] GameSettingsSO[] _allGameSettings;
    [SerializeField] GeneralEventSO[] _allGeneralEvents;
    [SerializeField] SceneAsset[] _allSceneAssets;
    public static SceneSO[] AllScenes => Instance._allScenes;
    public static ActionSO[] AllActions => Instance._allActions;
    public static NPC_ScriptableObject[] AllNPCs => Instance._allNPCs;
    public static GameSettingsSO[] AllGameSettings => Instance._allGameSettings;
    public static GeneralEventSO[] AllGeneralEvents => Instance._allGeneralEvents;
    public static SceneAsset[] AllSceneAssets => Instance._allSceneAssets;

    void Awake() => _instance = this;
    static GameData _instance;


    public static GameData Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<GameData>();
            return _instance;
        }
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        _allScenes = GetAllInstancesSO<SceneSO>();
        _allActions = GetAllInstancesSO<ActionSO>();
        _allNPCs = GetAllInstancesSO<NPC_ScriptableObject>();
        _allGameSettings = GetAllInstancesSO<GameSettingsSO>();
        _allGeneralEvents = GetAllInstancesSO<GeneralEventSO>();

        _allSceneAssets = GetAllObjects<SceneAsset>();
    }


    public static T[] GetAllObjects<T>(string searchPath = null) where T : UnityEngine.Object
    {
        string[]
            guids = AssetDatabase.FindAssets("t:" +
                                             typeof(T).Name); //FindAssets uses tags check documentation for more info
        if (searchPath != null)
            guids = AssetDatabase.FindAssets("t:" + typeof(T).Name, new[] {searchPath});

        T[] a = new T[guids.Length];
        for (int i = 0; i < guids.Length; i++) //probably could get optimized 
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            a[i] = AssetDatabase.LoadAssetAtPath<T>(path);
        }

        return a;
    }


    public static T[] GetAllInstancesSO<T>(string searchPath = null) where T : UnityEngine.ScriptableObject
    {
        string[]
            guids = AssetDatabase.FindAssets("t:" +
                                             typeof(T).Name); //FindAssets uses tags check documentation for more info
        if (searchPath != null)
            guids = AssetDatabase.FindAssets("t:" + typeof(T).Name, new[] {searchPath});

        T[] a = new T[guids.Length];
        for (int i = 0; i < guids.Length; i++) //probably could get optimized 
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            a[i] = AssetDatabase.LoadAssetAtPath<T>(path);
        }

        return a;
    }
#endif
}
