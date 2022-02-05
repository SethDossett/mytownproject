using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


namespace CrowdManagement
{

    [RequireComponent(typeof(AudioSource))]
    public class SoundController : MonoBehaviour
    {
        public List<AudioClip> clips;
        public bool randomClip = false;
        public bool loop = false;
        public bool randomVolume = false;
        public Vector2 delayOnEnable;
        [Header("X = min, Y = max")]
        public Vector2 volume = Vector2.one;
        public bool randomPitch = false;
        [Header("X = min, Y = max")]
        public Vector2 pitch = Vector2.one;
        public bool changeClip = false;
        public Vector2 clipChangeTimeRange = new Vector2(0.0f, 5.0f);

        private int currentClipIndex = 0;
        private AudioSource m_audio;
        private float defaultVolume;
        private float invokeTime = 0.0f;

        // Use this for initialization
        void Awake()
        {
            m_audio = GetComponent<AudioSource>();

            m_audio.loop = loop;

            if (clips != null && clips.Count > 0)
                m_audio.clip = clips[UnityEngine.Random.Range(0, clips.Count)];

            if (randomVolume)
                m_audio.volume = UnityEngine.Random.Range(volume.x, volume.y);

            if (randomPitch)
                m_audio.pitch = UnityEngine.Random.Range(pitch.x, pitch.y);

            defaultVolume = m_audio.volume;
        }

        private void OnEnable()
        {
            if (randomClip)
                Invoke("PlayRandomSound", UnityEngine.Random.Range(delayOnEnable.x, delayOnEnable.y));
            else
                Invoke("PlaySound", UnityEngine.Random.Range(delayOnEnable.x, delayOnEnable.y));
        }

        public void PlaySound()
        {
            if (!gameObject.activeSelf || 
            	!gameObject.activeInHierarchy ||
                m_audio.clip == null)
                return;

            m_audio.volume = defaultVolume;

            if (m_audio.enabled)
                m_audio.Play();

            if (changeClip)
            {
                invokeTime = UnityEngine.Random.Range(clipChangeTimeRange.x, clipChangeTimeRange.y) + m_audio.clip.length;
                Invoke("ChangeClip", invokeTime);
            }
        }

        public void PlayRandomSound()
        {
            if (clips == null || clips.Count <= 0)
                return;

            int rndNum = UnityEngine.Random.Range(0, clips.Count);
            m_audio.clip = clips[rndNum];

            PlaySound();
        }

        void ChangeClip()
        {
            if (clips == null || clips.Count <= 0)
                return;

            if (randomClip)
            {
                m_audio.clip = clips[UnityEngine.Random.Range(0, clips.Count)];
            }
            else
            {
                if (currentClipIndex == clips.Count && !loop)
                    return;

                m_audio.clip = clips[currentClipIndex % clips.Count];
                currentClipIndex++;
            }

            if (randomVolume)
            {
                m_audio.volume = UnityEngine.Random.Range(volume.x, volume.y);
                defaultVolume = m_audio.volume;
            }

            if (randomPitch)
                m_audio.pitch = UnityEngine.Random.Range(pitch.x, pitch.y);

            PlaySound();
        }
    }
}
