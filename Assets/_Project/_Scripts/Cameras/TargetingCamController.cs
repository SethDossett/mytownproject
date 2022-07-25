using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class TargetingCamController : MonoBehaviour
{
    CinemachineVirtualCamera cam;
    CinemachineOrbitalTransposer transposer; 
    [SerializeField] Transform player;

    void Start()
    {
        cam = GetComponent<CinemachineVirtualCamera>();
        transposer = cam.GetCinemachineComponent<CinemachineOrbitalTransposer>();
    
    }

    // Update is called once per frame
}
