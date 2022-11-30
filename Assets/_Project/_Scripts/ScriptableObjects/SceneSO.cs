using UnityEngine;
using System.Collections.Generic;
using MyTownProject.Interaction;

namespace MyTownProject.SO
{
    [CreateAssetMenu(menuName = "ScriptableObject/Scene")]
    public class SceneSO : ScriptableObject
    {
        public string SceneName;
        public Vector3 playerLocation;
        public Quaternion playerRotation;
        public int sceneIndex;

        [Header("Not Passing Through Door")]
        public Vector3 NoDoorStartPos;
        public Quaternion NoDoorStartRot;

        [Header("Passing Through Door")]
        public bool EnteredThroughDoor;
        public DoorPatnerIndex DoorIndex;

        public List<GameObject> DoorsInScene = new List<GameObject>();
    }
}