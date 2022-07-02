using UnityEngine;
using UnityEngine.UI;

public class InteractPrompt : MonoBehaviour
{
    Camera _cam;
    Transform _transform;
    public Image _imagePrompt;
    void Start()
    {
        _cam = Camera.main; 
        _transform = transform;
    }
    void Update()
    {
        
    }
    void LateUpdate()
    {
        Quaternion rot = _cam.transform.rotation;
        _transform.LookAt(_transform.position + rot * Vector3.forward, rot * Vector3.up);
    }
}
