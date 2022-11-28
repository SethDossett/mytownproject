using System.Collections;
using KinematicCharacterController.Examples;
using UnityEngine;
using MyTownProject.Events;
using MyTownProject.Core;
using Cinemachine;

namespace MyTownProject.Cameras
{

    public class FreeLookController : MonoBehaviour
    {
        [SerializeField] FloatEventSO RecenterCamX;
        [SerializeField] FloatEventSO RecenterCamY;
        [SerializeField] GeneralEventSO DisableRecenterCam;
        [SerializeField] GeneralEventSO StartOfGame;
        [SerializeField] DialogueEventsSO DialogueEvents;
        [SerializeField] TransformEventSO TargetingEvent;
        [SerializeField] GeneralEventSO UntargetEvent;
        [SerializeField] TransformEventSO PlayerReference;
        CinemachineFreeLook cam;
        [SerializeField] CinemachineTargetGroup _targetGroup;
        TheCharacterController CC;
        CinemachineInputProvider CameraInputs;

        [Range(0.1f, 2f)][SerializeField] float _lensZoomInSpeed = 0.2f;
        [Range(0.1f, 5f)][SerializeField] float _lensZoomOutSpeed = 0.6f;
        [SerializeField] AnimationCurve _ZoomIncurve;
        [SerializeField] AnimationCurve _ZoomOutcurve;


        void OnEnable()
        {
            cam = GetComponent<CinemachineFreeLook>();
            CameraInputs = GetComponent<CinemachineInputProvider>();
            GameStateManager.OnGameStateChanged += CheckGameState;
            PlayerReference.OnRaiseEvent += GetPlayerReference;
            StartOfGame.OnRaiseEvent += InitialGameStartingCamera;
            DialogueEvents.onEnter += TalkingToNPC;
            DialogueEvents.onExit += BackToPlayerView;
            TargetingEvent.OnRaiseEvent += Target;
            UntargetEvent.OnRaiseEvent += Untarget;
            RecenterCamX.OnRaiseEvent3 += RX;
            RecenterCamY.OnRaiseEvent3 += RY;
            DisableRecenterCam.OnRaiseEvent += DisableRecenter;
        }
        void OnDisable()
        {
            GameStateManager.OnGameStateChanged -= CheckGameState;
            PlayerReference.OnRaiseEvent -= GetPlayerReference;
            StartOfGame.OnRaiseEvent -= InitialGameStartingCamera;
            DialogueEvents.onEnter -= TalkingToNPC;
            DialogueEvents.onExit -= BackToPlayerView;
            TargetingEvent.OnRaiseEvent += Target;
            UntargetEvent.OnRaiseEvent -= Untarget;
            RecenterCamX.OnRaiseEvent3 -= RX;
            RecenterCamY.OnRaiseEvent3 -= RY;
            DisableRecenterCam.OnRaiseEvent -= DisableRecenter;
        }
        void GetPlayerReference(Transform player)
        {
            CC = player.GetComponent<TheCharacterController>();
        }
        void InitialGameStartingCamera()
        {
            RX(0, 0.1f, 1);
            RY(0, 0.1f, 1);
        }
        IEnumerator EnableCameraInputs(bool enable, float waitTime = 0)
        {
            yield return new WaitForSecondsRealtime(waitTime);

            if (enable)
            {
                CameraInputs.XYAxis.action.Enable();
                print("CAMERA INPUTS ON");
            }
            else
            {
                CameraInputs.XYAxis.action.Disable();
                print("CAMERA INPUTS OFF");
            }
            yield break;
        }

        void CheckGameState(GameState state)
        {
            if (state == GameState.GAME_PLAYING)
            {
                StartCoroutine(EnableCameraInputs(true));
            }
            else
            {
                StartCoroutine(EnableCameraInputs(false));
            }
        }
        void TalkingToNPC(GameObject go, TextAsset text)
        {
            //StartCoroutine(ChangeLens(25, _lensZoomInSpeed, _ZoomIncurve));
             StartCoroutine(EnableCameraInputs(true, 0.2f)); 

        }
        void BackToPlayerView()
        {
            //StartCoroutine(ChangeLens(40, _lensZoomOutSpeed, _ZoomOutcurve));
        }

        void Target(Transform t)
        {
            //StartCoroutine(ChangeLens(60, _lensZoomOutSpeed, _ZoomOutcurve));
            //RX(0,0.1f);
            //RY(0,0.1f);
        }
        void Untarget()
        {
            //BackToPlayerView();
            //RX(0,0.1f);
            //RY(0,0.1f);
        }
        IEnumerator ChangeLens(float value, float lerpDuration, AnimationCurve curve)
        {
            float startValue = cam.m_Lens.FieldOfView;
            float endValue = value;

            float timeElapsed = 0;
            while (timeElapsed < lerpDuration)
            {
                cam.m_Lens.FieldOfView = Mathf.Lerp(startValue, endValue, curve.Evaluate(timeElapsed / lerpDuration));
                timeElapsed += Time.unscaledDeltaTime;
                yield return null;
            }
            cam.m_Lens.FieldOfView = endValue;

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

        void DisableRecenter()
        {
            cam.m_RecenterToTargetHeading.m_enabled = false;
            cam.m_YAxisRecentering.m_enabled = false;
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

        public void OnCameraLive()
        {

        }
    }
}

