using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyTownProject.Events;
using Cinemachine;

namespace MyTownProject.Cameras{

    public class FreeLookController : MonoBehaviour
    {
        [SerializeField] DialogueEventsSO DialogueEvents;
        CinemachineFreeLook cam;
        [SerializeField] CinemachineTargetGroup _targetGroup;

        [Range(0.1f, 2f)][SerializeField] float _lensZoomSpeed = 0.5f;

        void OnEnable(){
            cam = GetComponent<CinemachineFreeLook>();
            DialogueEvents.onEnter += TalkingToNPC;
            DialogueEvents.onExit += BackToPlayerView;
        }
        void OnDisable(){
            DialogueEvents.onEnter -= TalkingToNPC;
            DialogueEvents.onExit -= BackToPlayerView;
        }

        void TalkingToNPC(GameObject go, TextAsset text){
            StartCoroutine(ChangeLens(25));
            
        }
        void BackToPlayerView(){
            StartCoroutine(ChangeLens(40));
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

    }
}

