using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyTownProject.SO
{
    [CreateAssetMenu(menuName = "ScriptableObject/Path")]
    public class PathSO : ScriptableObject
    {
        public List<RecordedValues> Records;
    }
    
    [System.Serializable]
    public struct RecordedValues
    {
        public float TimeStep;
        public Vector3 Positions;
        public Vector3 Rotations;

    }
}
