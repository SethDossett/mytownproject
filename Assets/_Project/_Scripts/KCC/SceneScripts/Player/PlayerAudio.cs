using MyTownProject.Events;
using UnityEngine;
using FMODUnity;

namespace KinematicCharacterController.Examples
{
    public class PlayerAudio : MonoBehaviour
    {
        TheCharacterController CC;

        void Awake(){
            CC = GetComponent<TheCharacterController>();
        }

        [SerializeField] AudioEventSO _playOneShot;

        
        [SerializeField] EventReference crawl;
        [SerializeField] EventReference footsteps;
        [SerializeField] EventReference land;
        [SerializeField] EventReference climbLadder;
        [SerializeField] EventReference ladderSigh;

        //FMOD.Studio.EventInstance footStepsInstance;

        public void CrawlAudio(){
            if(CC.CurrentRootName == P_StateNames.Crawling)
                _playOneShot.RaiseEvent(crawl);
        }

        public void FootStepAudio(){
            if (CC.Motor.GroundingStatus.IsStableOnGround)
            {
                //footStepsInstance = RuntimeManager.CreateInstance(footsteps);
                //footStepsInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform));
                //footStepsInstance.setParameterByName("Surface", (float)CC.CurrentGroundType);
                //footStepsInstance.start();
                //footStepsInstance.release();
                _playOneShot.RaiseEvent2(footsteps, transform.position);
            }
        } 

        public void LandAudio(){
            _playOneShot.RaiseEvent2(land, transform.position);
        }

        public void ClimbLadderAudio()
        {
            _playOneShot.RaiseEvent2(climbLadder, transform.position);
        }

        public void SighGettingOnOffLadder()
        {
            _playOneShot.RaiseEvent2(ladderSigh, transform.position);
        }
        //one play if player is grounded and if animation layer is on.
    }
}