using UnityEngine;
using MyTownProject.Events;
using FMODUnity;

namespace MyTownProject.Audio
{
    public class PlayOneShot : MonoBehaviour
    {
        [SerializeField] AudioEventSO audioEventSO;

        private void OnEnable()
        {
            audioEventSO.OnRaiseEvent += PlayOneShotClip;
            audioEventSO.OnRaiseEvent2 += PlayOneShotClipAndPosition;
        }
        private void OnDisable()
        {
            audioEventSO.OnRaiseEvent -= PlayOneShotClip;
            audioEventSO.OnRaiseEvent2 -= PlayOneShotClipAndPosition;
        }

        void PlayOneShotClip(EventReference clip)
        {
            RuntimeManager.PlayOneShot(clip.Guid);
            print("C IPPPP");
        }

        void PlayOneShotClipAndPosition(EventReference clip, Vector3 pos)
        {
            RuntimeManager.PlayOneShot(clip.Guid,pos);
        }
    }
}


