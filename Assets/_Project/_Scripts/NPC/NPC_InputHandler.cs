using System.Collections.Generic;
using UnityEngine;

namespace MyTownProject.NPC
{
    public class NPC_InputHandler : MonoBehaviour
    {
        public NPC_Controller Character;
        [SerializeField] private NPC_ScriptableObject NPC;
        Transform m_transform;

        private Vector3 _moveDir;
        bool _shouldMove = false;

        private void Start()
        {
            m_transform = transform;

            // just here to set SO starting position until event is set up.
            NPC.lastDestinationPosition = m_transform.position;
            NPC.currentIndex = 0;
            _shouldMove = true;

        }
        private void Update()
        {
            UpdateDirection(NPC.direction[NPC.currentIndex]);
            CheckDistance(NPC.distance[NPC.currentIndex], NPC.lastDestinationPosition); // can put within UpdateDirection()

            NPCCharacterInputs inputs = new NPCCharacterInputs();
            inputs.MoveVector = _moveDir;
            inputs.LookVector = _moveDir;

            Character.SetInputs(ref inputs);

        }

        private bool ShouldMove()
        {
            return true;
        }

        private void UpdateDirection(Vector3 npcDir)
        {
            if (!_shouldMove)
            {
                _moveDir = Vector3.zero;
            }
            else
            {
                _moveDir = npcDir;
            }
        }
        private void CheckDistance(int distance, Vector3 lastPosition)
        {
            if (Vector3.Distance(lastPosition, m_transform.position) >= distance)
            {
                UpdateValues(lastPosition);
            }
        }

        private void UpdateValues(Vector3 last)
        {
            if (NPC.currentIndex >= NPC.distance.Length - 1)
            {
                _shouldMove = false;
                return;
            }

            NPC.lastDestinationPosition = m_transform.position;
            NPC.currentIndex++;
        }
    }
}