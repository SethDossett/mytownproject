using MyTownProject.Events;
using UnityEngine;
using FMODUnity;

namespace KinematicCharacterController.Examples
{
    public class PlayerAudio : MonoBehaviour
    {
        [SerializeField] AudioEventSO _playOneShot;

        [SerializeField] EventReference crawl;

        public void CrawlAudio() => _playOneShot.RaiseEvent(crawl);
        
    }
}