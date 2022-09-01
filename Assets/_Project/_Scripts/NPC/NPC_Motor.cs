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
        Transform _npcTransform = null;
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
            _npcTransform = transform; // doesnt need to be cashed
        }
        
        private void Update()
        {
            if (_talking) RotateToFacePlayer(); else CheckToMove(NPC.currentState);

            //transform.rotation = NPC.currentRotation;
        }

        private void RotateToFacePlayer()
        {
            Quaternion rotation = Quaternion.LookRotation(_player.position - _npcTransform.position, Vector3.up);
            rotation.Normalize();
            rotation.x = 0;
            rotation.z = 0;

            NPC.currentRotation = Quaternion.RotateTowards(NPC.currentRotation, rotation, _rotSpeed * Time.unscaledDeltaTime);
            transform.rotation = NPC.currentRotation;
        }

        private void CheckToMove(NPC_StateHandler.NPCSTATE state)
        {
            if (state == NPC_StateHandler.NPCSTATE.TALKING || state == NPC_StateHandler.NPCSTATE.WORKING) return;
            else if(state == NPC_StateHandler.NPCSTATE.STANDING){
                if (!NPC.returnToStandingRotation) return;
                if (NPC.currentRotation == NPC.shouldBeStandingRotation) return;
                NPC.currentRotation = Quaternion.RotateTowards(NPC.currentRotation, NPC.shouldBeStandingRotation, (_rotSpeed)  * Time.unscaledDeltaTime);
                transform.rotation = NPC.currentRotation;
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
                rb.rotation = NPC.currentRotation;
            }
            
        }
        private bool RotatedToTarget()
        {
            if(Vector3.Dot(_player.forward, _npcTransform.forward) <= -0.7f)
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
            
            

            //Debug.Log(_npcTransform.forward);
            //Debug.Log(_player.forward);
            //Debug.Log(Vector3.Dot(_player.forward, _npcTransform.forward));
            //if(state == NPC_StateHandler.NPCSTATE.STANDING){
            //    //StartCoroutine(RotateToStartPosition());
            //    
            //}
            //
            //if (RotatedToTarget()) return;
            //
            //if (state == NPC_StateHandler.NPCSTATE.TALKING)
            //{
            //    RotateToPlayer();
            //}


        }
        private void RotateToPlayer()
        {
            _talking = true;
            _prevRotation = transform.rotation;
            //Lets put in coroutine to move player until dot product in 1.

            //can set to only rotate body if angle is outside set amount.
            // this is so if so choose we can rotate just head if not outside certain angle.
            // or even just eyes then head then body.

            /*Vector3 npcPos = transform.position;
            Vector3 delta = new Vector3(npcPos.x - _player.position.x, 0.0f, npcPos.z - _player.position.z);

            Quaternion rotation = Quaternion.LookRotation(delta);*/
            print("BIGSHOT");
            
            
           
            //transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, 200 * Time.unscaledDeltaTime);
            StartCoroutine(Rotate());

        }
        IEnumerator Rotate()
        {
            
            Quaternion rotation = Quaternion.LookRotation(_player.position - _npcTransform.position, Vector3.up);
            rotation.Normalize();
            rotation.x = 0;
            rotation.z = 0;

            while (Vector3.Dot(_player.forward, _npcTransform.forward) >= -0.98f)
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