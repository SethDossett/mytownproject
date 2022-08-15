using System.Security.Cryptography;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyTownProject.Events;
using Cinemachine;

namespace MyTownProject.Cameras{

    public class FreeLookController : MonoBehaviour
    {
        public Transform _player;
        [SerializeField] FloatEventSO RecenterCamX;
        [SerializeField] FloatEventSO RecenterCamY;
        [SerializeField] DialogueEventsSO DialogueEvents;
        [SerializeField] GeneralEventSO UntargetEvent;
        CinemachineFreeLook cam;
        [SerializeField] CinemachineTargetGroup _targetGroup;

        [Range(0.1f, 2f)][SerializeField] float _lensZoomInSpeed = 0.2f;
        [Range(0.1f, 5f)][SerializeField] float _lensZoomOutSpeed = 0.6f;
        [SerializeField] AnimationCurve _ZoomIncurve;
        [SerializeField] AnimationCurve _ZoomOutcurve;

        
        void OnEnable(){
            cam = GetComponent<CinemachineFreeLook>();
            DialogueEvents.onEnter += TalkingToNPC;
            DialogueEvents.onExit += BackToPlayerView;
            UntargetEvent.OnRaiseEvent += Untarget;
            RecenterCamX.OnRaiseEvent2 += RX;
            RecenterCamY.OnRaiseEvent2 += RY;
        }
        void OnDisable(){
            DialogueEvents.onEnter -= TalkingToNPC;
            DialogueEvents.onExit -= BackToPlayerView;
            UntargetEvent.OnRaiseEvent -= Untarget;
            RecenterCamX.OnRaiseEvent2 -= RX;
            RecenterCamY.OnRaiseEvent2 -= RY;
        }

        void TalkingToNPC(GameObject go, TextAsset text){
            StartCoroutine(ChangeLens(25, _lensZoomInSpeed, _ZoomIncurve));
            
        }
        void BackToPlayerView(){
            StartCoroutine(ChangeLens(40, _lensZoomOutSpeed, _ZoomOutcurve));
        }
        void Untarget(){
            RX(0,0);
            RY(0,0);
        }
        IEnumerator ChangeLens(float value, float lerpDuration, AnimationCurve curve){
            float startValue = cam.m_Lens.FieldOfView;
            float endValue = value;

            float timeElapsed = 0;
            while(timeElapsed < lerpDuration){
                cam.m_Lens.FieldOfView = Mathf.Lerp(startValue, endValue, curve.Evaluate(timeElapsed/ lerpDuration));
                timeElapsed += Time.unscaledDeltaTime;
                yield return null;
            }
            cam.m_Lens.FieldOfView = endValue;
            
        }

        void RX(float f1, float f2) => StartCoroutine(RecenterXAxis(f1,f2));
        IEnumerator RecenterXAxis(float waitTime, float recenteringTime){
            cam.m_RecenterToTargetHeading.m_WaitTime = waitTime;
            cam.m_RecenterToTargetHeading.m_RecenteringTime = recenteringTime;
            cam.m_RecenterToTargetHeading.m_enabled = true;
            yield return new WaitForSeconds(waitTime + recenteringTime * 2);
            yield return new WaitForEndOfFrame();
            //yield return new WaitUntil(()=> Mathf.Abs(cam.m_XAxis.Value) < 0.1f);  
            cam.m_RecenterToTargetHeading.m_enabled = false;
            yield break;
               
            
        }
        void RY(float f1, float f2) => StartCoroutine(RecenterYAxis(f1,f2));
        IEnumerator RecenterYAxis(float waitTime, float recenteringTime){
            cam.m_YAxisRecentering.m_WaitTime = waitTime;
            cam.m_YAxisRecentering.m_RecenteringTime = recenteringTime;
            cam.m_YAxisRecentering.m_enabled = true;
            yield return new WaitForSecondsRealtime(waitTime + recenteringTime);
            yield return new WaitForEndOfFrame();
            cam.m_YAxisRecentering.m_enabled = false;
            yield break;
        }
        void Update(){
            // see what world angle camera is facing
            // what is angle of player
            //cam.m_XAxis.Value = 5f;
            print(Mathf.Abs(cam.m_XAxis.Value));
            
            if(UnityEngine.InputSystem.Keyboard.current.rKey.wasPressedThisFrame){
                RX(0,0.7f);
            }
        }
    }
}

