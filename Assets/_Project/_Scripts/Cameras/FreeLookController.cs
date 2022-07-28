using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyTownProject.Events;
using Cinemachine;

namespace MyTownProject.Cameras{

    public class FreeLookController : MonoBehaviour
    {
        [SerializeField] DialogueEventsSO DialogueEvents;
        [SerializeField] GeneralEventSO UntargetEvent;
        CinemachineFreeLook cam;
        [SerializeField] CinemachineTargetGroup _targetGroup;

        [Range(0.1f, 2f)][SerializeField] float _lensZoomSpeed = 0.5f;

        void OnEnable(){
            cam = GetComponent<CinemachineFreeLook>();
            DialogueEvents.onEnter += TalkingToNPC;
            DialogueEvents.onExit += BackToPlayerView;
            UntargetEvent.OnRaiseEvent += Untarget;
        }
        void OnDisable(){
            DialogueEvents.onEnter -= TalkingToNPC;
            DialogueEvents.onExit -= BackToPlayerView;
            UntargetEvent.OnRaiseEvent -= Untarget;
        }

        void TalkingToNPC(GameObject go, TextAsset text){
            StartCoroutine(ChangeLens(25));
            
        }
        void BackToPlayerView(){
            StartCoroutine(ChangeLens(40));
        }
        void Untarget(){
            print("jay");
            StartCoroutine(RecenterYAxis(0,0));
            StartCoroutine(RecenterXAxis(0,0));
        }

        IEnumerator ChangeLens(float value){
            float lerpDuration = _lensZoomSpeed;
            float startValue = cam.m_Lens.FieldOfView;
            float endValue = value;

            float timeElapsed = 0;
            while(timeElapsed < lerpDuration){
                cam.m_Lens.FieldOfView = Mathf.Lerp(startValue, endValue, timeElapsed/ lerpDuration);
                timeElapsed += Time.unscaledDeltaTime;
                yield return null;
            }
            cam.m_Lens.FieldOfView = endValue;
            
        }

        IEnumerator RecenterXAxis(float waitTime, float recenteringTime){
            cam.m_RecenterToTargetHeading.m_WaitTime = waitTime;
            cam.m_RecenterToTargetHeading.m_RecenteringTime = recenteringTime;
            cam.m_RecenterToTargetHeading.m_enabled = true;
            yield return new WaitForSecondsRealtime(waitTime + recenteringTime);
            yield return new WaitForEndOfFrame();
            cam.m_RecenterToTargetHeading.m_enabled = false;
            yield break;
        }
        IEnumerator RecenterYAxis(float waitTime, float recenteringTime){
            cam.m_YAxisRecentering.m_WaitTime = waitTime;
            cam.m_YAxisRecentering.m_RecenteringTime = recenteringTime;
            cam.m_YAxisRecentering.m_enabled = true;
            yield return new WaitForSecondsRealtime(waitTime + recenteringTime);
            yield return new WaitForEndOfFrame();
            cam.m_YAxisRecentering.m_enabled = false;
            yield break;
        }

    }
}

