using UnityEngine;
using Cinemachine;
using MyTownProject.SO;
using MyTownProject.Enviroment;
using MyTownProject.Interaction;

namespace MyTownProject.Cameras
{
    public class DoorCameraController : MonoBehaviour
    {
        CinemachineVirtualCamera cam;
        [SerializeField] ActionSO OpenDoor;
        Transform _doorCameraPosition;

        private void OnEnable()
        {
            OpenDoor.OnOpenDoor += SetCameraPosition;
            
        }
        private void OnDisable()
        {
            OpenDoor.OnOpenDoor -= SetCameraPosition;
            
        }
        void Start(){
            cam = GetComponent<CinemachineVirtualCamera>();
        }


        void SetCameraPosition(DoorType type, GameObject door)
        {
            cam.Follow = door.GetComponent<Door>().CameraPosition;
            // Vector3 offset = door.transform.rotation * new Vector3(20f ,0, 15f);
            // Vector3 pos = door.transform.position + offset;
            // Quaternion rot = door.transform.rotation;
            // _doorCameraPosition.position = pos;
            
        }
        


        
    }
}
