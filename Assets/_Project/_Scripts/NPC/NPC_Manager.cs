using UnityEngine;
using MyTownProject.Events;
using UnityEngine.SceneManagement;

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

        }
        private void OnDisable()
        {

        }
        private void Awake()
        {
            _collider = GetComponent<CapsuleCollider>();
        }

        private void Start()
        {
            currentScene = SceneManager.GetActiveScene();
            //transform.position = NPC.currentPosition;
            CheckScene();
        }
    
        private void CheckScene()//set ScriptExecutuonOrder  hide npc to before player is teleported.
        {
            if ((int)NPC.currentScene != currentScene.buildIndex)
            {
                HideNPC();
            }
            else
            {
                UnHideNPC();
            }
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

        public void PlayFootStepSound(){
            
        }


    }
}