using UnityEngine;

public class GetButtonSelection : MonoBehaviour
{
    [SerializeField] GameObject _button;
    public GameObject Button {get{return _button;} set {value = _button;}}
    
}
