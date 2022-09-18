using UnityEngine;
using MyTownProject.Events;
using UnityEngine.SceneManagement;
using System;

namespace MyTownProject.NPC
{
    public class NPC_Manager : MonoBehaviour
    {
        [SerializeField] NPC_ScriptableObject NPC;
        [SerializeField] DialogueEventsSO dialogueEvents;
        Scene currentScene;
        [SerializeField] GameObject _meshes;
        CapsuleCollider _collider;
        public GameObject _head;
        public bool IsVisible;

        private void OnEnable()
        {
            NPC_DestinationHandler.UpdateScene += NextAction;
        }
        private void OnDisable()
        {
            NPC_DestinationHandler.UpdateScene -= NextAction;
        }
        private void Awake()
        {
            _collider = GetComponent<CapsuleCollider>();
        }

        private void Start()
        {
            currentScene = SceneManager.GetActiveScene();
            //transform.position = NPC.currentPosition;
        }

        private void Update()
        {
            CheckScene();

        }
        void DetectForPlayer()
        {
            //float rad = 2f;
            //Collider[] cols = new Collider[3];
            //int numFound;
            //numFound = Physics.OverlapSphereNonAlloc(transform.position, rad, cols, _playerLayer);
            //
            //if(numFound > 0)
            //{
            //    Transform player = cols[0].GetComponent<Transform>();
            //}

        }
        private void CheckScene()//set ScriptExecutuonOrder  hide npc to before player is teleported.
        {
            if (NPC.currentScene != currentScene.buildIndex)
            {
                HideNPC();
            }
            else
            {
                UnHideNPC();
            }
        }

        void NextAction(DestinationPathsSO path) // check when scene starts and when npc action takes place.
        {
            Debug.Log(currentScene.buildIndex);
            Debug.Log(NPC.currentScene);
            
           
        }

        void HideNPC()
        {
            if(_meshes.activeInHierarchy == true)
                _meshes.SetActive(false);
            if(_collider.enabled)
                _collider.enabled = false;
        }

        void UnHideNPC()
        {
            if (_meshes.activeInHierarchy == false)
                _meshes.SetActive(true);
            if (!_collider.enabled)
                _collider.enabled = true;
        }


    }
}