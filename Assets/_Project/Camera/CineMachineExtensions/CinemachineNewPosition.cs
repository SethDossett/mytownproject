using UnityEngine;
using Cinemachine;

public class CinemachineNewPosition : CinemachineExtension
{
    Vector3 newPos;
    Quaternion newRot;
    [SerializeField] Transform target;
    [SerializeField] Transform player;
    [SerializeField] Camera mainCam;
    
    
    private void Update()
    {
        
        Vector3 dir = target.position - mainCam.transform.position;
        dir.Normalize();
        dir.y = 0;

        Quaternion targetRotation = Quaternion.LookRotation(dir);
        //player.rotation = targetRotation;


        //targetRotation = Quaternion.LookRotation(dir);
        Vector3 eulerAngle = targetRotation.eulerAngles;
        eulerAngle.y = 0;
        //mainCam.transform.localEulerAngles = eulerAngle;
        newRot = targetRotation;
    }
 
    protected override void PostPipelineStageCallback(
        CinemachineVirtualCameraBase vcam,
        CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
    {
        if (stage == CinemachineCore.Stage.Body)
        {
            //state.PositionCorrection = mainCam.transform.position;
            state.OrientationCorrection = newRot; // could be +=?
        }
    }
}
