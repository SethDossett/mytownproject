using UnityEngine;
public class ClimbableObj_Modifier : MonoBehaviour //Could be Interface if Objects start to have other scripts on them.
{
    [field: SerializeField] public bool Climbable { get; private set; }
}
