using UnityEngine;
using Cinemachine;
using MyTownProject.SO;
using MyTownProject.Enviroment;

public class CinemachineNewPosition : CinemachineExtension
{
    Vector3 newPos;
    Quaternion newRot;

    CinemachineVirtualCamera cam;
    [SerializeField] GameObject door;
    
    void Start()
    {
        cam = GetComponent<CinemachineVirtualCamera>();
        Quaternion rot = door.transform.rotation;
    }


    Vector3 SetCameraPosition()
    {
        Vector3 offset = door.transform.rotation * new Vector3(20f, 0, 15f);
        Vector3 pos = door.transform.position + offset;
        
        return pos;
    }

    protected override void PostPipelineStageCallback(
        CinemachineVirtualCameraBase vcam,
        CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
    {
        if (stage == CinemachineCore.Stage.Body)
        {
            state.PositionCorrection = SetCameraPosition();
            state.OrientationCorrection = newRot; // could be +=?
        }
    }
}
