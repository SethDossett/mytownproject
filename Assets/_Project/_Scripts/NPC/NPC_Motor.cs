using MyTownProject.Events;
using UnityEngine;
using System.Collections;

namespace MyTownProject.NPC
{
    public class NPC_Motor : MonoBehaviour
    {
        [SerializeField] NPC_ScriptableObject NPC;
        [SerializeField] DialogueEventsSO dialogueEvents;
        [SerializeField] Transform _player;
        Rigidbody rb;
        [Range(20f, 300f)][SerializeField] float _rotSpeed = 100f;
        bool _talking = false;


        
        private void OnEnable()
        {
            NPC.OnChangedState += HandleTalkingRotation;
        }
        private void OnDisable()
        {
            NPC.OnChangedState -= HandleTalkingRotation;
        }
        private void Start()
        {
            rb = GetComponent<Rigidbody>();
            rb.position = NPC.currentPosition;
            rb.rotation = NPC.currentRotation.normalized;
        }
        
        private void Update()
        {
            DoMove(NPC.currentState);

            if (_talking)
                RotateToPlayer();
        }

        private void DoMove(NPC_StateHandler.NPCSTATE state)
        {
            
            if (state != NPC_StateHandler.NPCSTATE.WALKING)
                return;
                
            HandleMove();
        }

        private void HandleMove()
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

        private void HandleTalkingRotation(NPC_StateHandler.NPCSTATE state)
        {
            if (state != NPC_StateHandler.NPCSTATE.TALKING)
            {
                _talking = false;
                return;
            }

            print("DEBUGGG");
            _talking = true;
            
            
            
        }
        private void RotateToPlayer()
        {
            //can set to only rotate body if angle is outside set amount.
            // this is so if so choose we can rotate just head if not outside certain angle.
            // or even just eyes then head then body.

            /*Vector3 npcPos = transform.position;
            Vector3 delta = new Vector3(npcPos.x - _player.position.x, 0.0f, npcPos.z - _player.position.z);

            Quaternion rotation = Quaternion.LookRotation(delta);*/

            
            Quaternion rotation = Quaternion.LookRotation(_player.position - transform.position);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, 200 * Time.unscaledDeltaTime);


        }
    }
}