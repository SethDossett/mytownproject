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

        public void CrawlAudio() => _playOneShot.RaiseEvent(crawl);

        public void FootStepAudio(){
            if(CC.Motor.GroundingStatus.IsStableOnGround)
                _playOneShot.RaiseEvent2(footsteps, transform.position);
        } 
        //one play if player is grounded and if animation layer is on.
    }
}