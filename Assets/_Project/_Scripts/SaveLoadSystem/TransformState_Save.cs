using MyTownProject.NPC;
using UnityEngine;

namespace MyTownProject.SaveLoadSystem
{
    public class TransformState_Save : State_Save
    {
        [SerializeField] NPC_ScriptableObject NPC;

        public struct NPCTransformValues
        {
            public Vector3 position;
            public Quaternion rotation;
        }

        public NPCTransformValues saveState = new NPCTransformValues();

        public override string SaveState()
        {
            saveState.position = NPC.currentPosition;
            saveState.rotation = NPC.currentRotation;


            return JsonUtility.ToJson(saveState);
        }

        public override void LoadState(string loadedJSON)
        {
            saveState = JsonUtility.FromJson<NPCTransformValues>(loadedJSON);

            NPC.currentPosition = saveState.position;
            NPC.currentRotation = saveState.rotation;
            transform.position = saveState.position;
            transform.rotation = saveState.rotation;
        }

        public override bool ShouldSave()
        {
            if(saveState.position == NPC.currentPosition && saveState.rotation == NPC.currentRotation)
                return false;


            return true;
        }
    }
}