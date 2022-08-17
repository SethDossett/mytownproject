using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace MyTownProject.Cameras{
public class TargetingCamController : MonoBehaviour
{
    CinemachineVirtualCamera cam;
    CinemachineFramingTransposer transposer; 
    Camera mainCam;
    [SerializeField] Transform player;
    Transform target;

    void Start()
    {
        mainCam = Camera.main;
        cam = GetComponent<CinemachineVirtualCamera>();
        target = cam.m_LookAt;
        transposer = cam.GetCinemachineComponent<CinemachineFramingTransposer>();
        
    }

    private void LateUpdate() {
       //mainCam.transform.position = player.position;
       //transform.position = new Vector3(player.position.x, 1, player.position.z);

        //Vector3 dir = target.position - player.position;
        //dir.Normalize();
        //dir.y = 0;

        Vector3 dir = target.position - mainCam.transform.position;
        dir.Normalize();

        Quaternion targetRotation = Quaternion.LookRotation(dir);
        //player.rotation = targetRotation;


        //targetRotation = Quaternion.LookRotation(dir);
        Vector3 eulerAngle = targetRotation.eulerAngles;
        eulerAngle.y = 0;
        mainCam.transform.localEulerAngles = eulerAngle;

    }
}
}
