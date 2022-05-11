using UnityEngine;

namespace MyTownProject.SO
{
    [CreateAssetMenu(menuName = "ScriptableObject/Scene")]
    public class SceneSO : ScriptableObject
    {
        public Vector3 playerLocation;
        public Quaternion playerRotation;
        public int sceneIndex;
    }
}