using MyTownProject.Events;
using UnityEngine;
using System.Collections;
using System;

namespace MyTownProject.NPC
{
    public class NPC_Motor : MonoBehaviour
    {
        [SerializeField] NPC_ScriptableObject NPC;
        [SerializeField] TransformEventSO playerRef;
        [SerializeField] DialogueEventsSO dialogueEvents;
        Transform _player;
        Rigidbody rb;
        [Range(20f, 300f)][SerializeField] float _rotSpeed = 100f;
        bool _talking = false;

        Quaternion _prevRotation;
        
        private void OnEnable()
        {
            playerRef.OnRaiseEvent += SetPlayerReference;
            NPC.OnChangedState += CheckNPCState;
        }
        private void OnDisable()
        {
            playerRef.OnRaiseEvent -= SetPlayerReference;
            NPC.OnChangedState -= CheckNPCState;
        }
        void SetPlayerReference(Transform player) => _player = player;
        private void Start()
        {
            rb = GetComponent<Rigidbody>();
            rb.position = NPC.currentPosition;
            rb.rotation = NPC.currentRotation.normalized;
        }
        
        private void Update()
        {
            if (_talking) RotateToFacePlayer(); else CheckToMove(NPC.currentState);

        }

        private void RotateToFacePlayer()
        {
            Quaternion rotation = Quaternion.LookRotation(_player.position - transform.position, Vector3.up);
            rotation.Normalize();
            rotation.x = 0;
            rotation.z = 0;
            //if angle is less than certain amount then just turn head, then rotate rest of body. also set up bool check for just eyes, eyes range, head, range, rotation range.
            NPC.currentRotation = Quaternion.RotateTowards(NPC.currentRotation, rotation, _rotSpeed * Time.unscaledDeltaTime);
            transform.rotation = NPC.currentRotation.normalized;
        }

        private void CheckToMove(NPC_StateHandler.NPCSTATE state)
        {
            if (state == NPC_StateHandler.NPCSTATE.TALKING || state == NPC_StateHandler.NPCSTATE.WORKING) return;
            else if(state == NPC_StateHandler.NPCSTATE.STANDING){
                if (!NPC.returnToStandingRotation) return;
                if (NPC.currentRotation == NPC.shouldBeStandingRotation) return;
                NPC.currentRotation = Quaternion.RotateTowards(NPC.currentRotation, NPC.shouldBeStandingRotation, (_rotSpeed)  * Time.unscaledDeltaTime);
                rb.rotation = NPC.currentRotation.normalized;
            }
            else if (state == NPC_StateHandler.NPCSTATE.WALKING)
                DoMove();
        }

        private void DoMove()
        {
            if (!NPC.moveTowardsDestination)
                return;

            rb.MovePosition(NPC.currentPosition);

            
            if (rb.velocity.magnitude > float.Epsilon)
            {
                Quaternion rotation = Quaternion.LookRotation(rb.velocity, Vector3.up);
                rotation.Normalize();
                rotation.x = 0;
                rotation.z = 0;
                //rb.MoveRotation(rotation);
                NPC.currentRotation = Quaternion.RotateTowards(NPC.currentRotation, rotation, _rotSpeed * Time.fixedDeltaTime);
                rb.rotation = NPC.currentRotation.normalized;
            }
            
        }
        private bool RotatedToTarget()
        {
            if(Vector3.Dot(_player.forward, transform.forward) <= -0.7f)
            {
                return true;
            }
            else return false;
        }
        private void CheckNPCState(NPC_StateHandler.NPCSTATE state)
        {
            if(state != NPC_StateHandler.NPCSTATE.TALKING)
                _talking = false;
            else
            {
                NPC.shouldBeStandingRotation = transform.rotation;
                _talking = true;
            }
            


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