using MyTownProject.Events;
using UnityEngine;
using System.Collections;

namespace MyTownProject.NPC
{
    public class NPC_Motor : MonoBehaviour
    {
        [SerializeField] NPC_ScriptableObject NPC;
        [SerializeField] TransformEventSO playerRef;
        [SerializeField] DialogueEventsSO dialogueEvents;
        [SerializeField] Transform _player;
        Rigidbody rb;
        [Range(20f, 300f)][SerializeField] float _rotSpeed = 100f;
        bool _talking = false;

        Quaternion _prevRotation;
        void Awake(){
            print($"awake {gameObject.name}");
            playerRef.OnRaiseEvent += SetPlayerReference;

        }
        void OnDestroy(){
            playerRef.OnRaiseEvent -= SetPlayerReference;
        
        }

        void SetPlayerReference(Transform player) => _player = player;
  
        
        private bool RotatedToTarget()
        {
            if(Vector3.Dot(_player.forward, transform.forward) <= -0.7f)
            {
                return true;
            }
            else return false;
        }
        
        private void RotateToPlayer()
        {
            _talking = true;
            _prevRotation = transform.rotation;
            StartCoroutine(Rotate());

        }
        IEnumerator Rotate()
        {
            
            Quaternion rotation = Quaternion.LookRotation(_player.position - transform.position, Vector3.up);
            rotation.Normalize();
            rotation.x = 0;
            rotation.z = 0;

            while (Vector3.Dot(_player.forward, transform.forward) >= -0.98f)
            {
                NPC.currentRotation = Quaternion.RotateTowards(NPC.currentRotation, rotation, _rotSpeed * Time.unscaledDeltaTime);
                
                yield return null;
            }
            yield break;
        }

        IEnumerator RotateToStartPosition()
        {
            Quaternion rotation = _prevRotation;
            rotation.Normalize();
            rotation.x = 0;
            rotation.z = 0;

            while (Quaternion.Angle(NPC.currentRotation, _prevRotation) >= -0.98f)
            {
                NPC.currentRotation = Quaternion.RotateTowards(NPC.currentRotation, _prevRotation, _rotSpeed * Time.unscaledDeltaTime);
                
                
                yield return null;
            }
            yield break;
        }
    }
}