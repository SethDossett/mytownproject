using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class TargetingCamController : MonoBehaviour
{
    CinemachineFreeLook cam;
    CinemachineTransposer transposer; 
    Camera mainCam;
    [SerializeField] Transform player;

    void Start()
    {
        mainCam = Camera.main;
        //cam = GetComponent<CinemachineFreeLook>();
        //transposer = cam.GetCinemachineComponent<CinemachineTransposer>();
        
    }

    private void LateUpdate() {
       //mainCam.transform.position = player.position;
    }
}
