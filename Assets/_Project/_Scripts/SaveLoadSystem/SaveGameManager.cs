using UnityEngine;
using UnityEngine.Events;
using System.IO;

namespace MyTownProject.SaveLoadSystem
{
    public static class SaveGameManager
    {
        public static SaveData CurrentSaveData = new SaveData(); 

        public static UnityAction OnLoadGameStart;
        public static UnityAction OnLoadGameFinish;

        public const string SaveDirectory = "/SaveData/";
        public const string FileName = "SaveGame.sav";
        public static bool SaveGame() {


            var dir = Application.persistentDataPath + SaveDirectory;

            if(!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            string json = JsonUtility.ToJson(CurrentSaveData, true);
            File.WriteAllText(dir + FileName, json);

            GUIUtility.systemCopyBuffer = dir;

            return true;
        }

        public static void LoadGame() {

            OnLoadGameStart?.Invoke();
            string fullPath = Application.persistentDataPath + SaveDirectory + FileName;
            SaveData tempData = new SaveData();

            if(File.Exists(fullPath)) 
            {
                string json = File.ReadAllText(fullPath);
                tempData = JsonUtility.FromJson<SaveData>(json);
            }
            else
            {
                Debug.LogError("Save file does not exist!");
            }

            CurrentSaveData = tempData;
            OnLoadGameFinish?.Invoke();
        }


    }
}