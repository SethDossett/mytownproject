using UnityEngine;
using MyTownProject.Events;
using FMODUnity;

namespace MyTownProject.Audio
{
    public class PlayOneShot : MonoBehaviour
    {
        [SerializeField] AudioEventSO audioEventSO;
        [SerializeField] EventReference OneShot;

        //Make DropDown where you can select when you want oneshot played.
        //onenable, awake, start, custom etc.

        public void PlayOneShotClip()
        {
            audioEventSO.RaiseEvent(OneShot);
        }

        void PlayOneShotClipAndPosition(EventReference clip, Vector3 pos)
        {
            RuntimeManager.PlayOneShot(clip.Guid,pos);
        }
    }
}


