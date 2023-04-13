using MyTownProject.Events;
using UnityEngine;
using Cinemachine;
using System.Collections;
using KinematicCharacterController.Examples;

namespace MyTownProject.Cameras
{
    public class TargetingCamController : MonoBehaviour
    {
        [SerializeField] GeneralEventSO UntargetEvent;
        [SerializeField] TransformEventSO PlayerReference;

        CinemachineFreeLook cam;
        Transform LookAtPoint;

        private void OnEnable()
        {
            UntargetEvent.OnRaiseEvent += Untarget;
        }
        private void OnDisable()
        {
            UntargetEvent.OnRaiseEvent -= Untarget;
        }
        void Awake()
        {
            PlayerReference.OnRaiseEvent += GetPlayerRef;
            cam = GetComponent<CinemachineFreeLook>();
            
        }
        void GetPlayerRef(Transform player)
        {
            LookAtPoint = player.GetComponent<TheCharacterController>()._LookAtPoint;
            cam.m_Follow = LookAtPoint;
        }
        public void OnCameraLive()
        {
            RX(1f, 2f, 1);
            RY(1f, 2f, 1);
        }
        void Untarget()
        {
            DisableRecenter();
        }

        //RX() RY() doesnt work if recenterTime is not > 0.
        void RX(float f1, float f2, float f3) => StartCoroutine(RecenterXAxis(f1, f2, f3));
        IEnumerator RecenterXAxis(float waitTime, float recenteringTime, float disableRecenter = 1)
        {
            EnableRecenter(waitTime, recenteringTime, false);
            // a value greater than 0 will disable recentering
            if (disableRecenter > 0)
            {
                yield return new WaitForSecondsRealtime(waitTime + recenteringTime * 3);
                yield return new WaitForEndOfFrame();
                DisableRecenter();
            }
            yield break;
        }
        void RY(float f1, float f2, float f3) => StartCoroutine(RecenterYAxis(f1, f2, f3));
        IEnumerator RecenterYAxis(float waitTime, float recenteringTime, float disableRecenter = 1)
        {
            EnableRecenter(waitTime, recenteringTime, true);
            // a value greater than 0 will disable recentering
            if (disableRecenter > 0)
            {
                yield return new WaitForSecondsRealtime(waitTime + recenteringTime * 3);
                yield return new WaitForEndOfFrame();
                DisableRecenter();
            }

            yield break;
        }

        void EnableRecenter(float waitTime, float recenteringTime, bool recenterY)
        {
            //Recenters Y Axis
            if (recenterY)
            {
                cam.m_YAxisRecentering.m_WaitTime = waitTime;
                cam.m_YAxisRecentering.m_RecenteringTime = recenteringTime;
                cam.m_YAxisRecentering.m_enabled = true;
            }
            //Recenters X Axis
            else
            {
                cam.m_RecenterToTargetHeading.m_WaitTime = waitTime;
                cam.m_RecenterToTargetHeading.m_RecenteringTime = recenteringTime;
                cam.m_RecenterToTargetHeading.m_enabled = true;
            }
        }
        void DisableRecenter()
        {
            cam.m_RecenterToTargetHeading.m_enabled = false;
            cam.m_YAxisRecentering.m_enabled = false;
        }
    }
}
