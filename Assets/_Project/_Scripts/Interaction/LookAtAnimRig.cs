using UnityEngine.Animations.Rigging;
using UnityEngine;
using MyTownProject.NPC;
using System.Collections;
using System.Collections.Generic;

namespace MyTownProject.Interaction
{
    public class LookAtAnimRig : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] NPC_ScriptableObject NPC;

        [Header("OverLap Sphere")]
        [SerializeField] Transform _fieldOfView;
        [SerializeField] float _fofVRadius;
        [SerializeField] LayerMask _playerLayer;
        [SerializeField] int _numsFound = 0;
        Collider[] _colliders = new Collider[3];

        [Header("Rig Values")]
        [SerializeField] RigBuilder _rigBuilder;
        MultiAimConstraint _multiAimConstraint;
        int _targetValue;
        bool _targetSet = false;
        float _lookSpeed = 1f;

        private void Awake()
        {
            _multiAimConstraint = GetComponent<MultiAimConstraint>();
        }
        private void OnEnable()
        {
            NPC.OnChangedState += ChangedState; //need to set event from when NPC Talks to look at player.
        }
        private void OnDisable()
        {
            NPC.OnChangedState -= ChangedState;
        }
        void ChangedState(NPC_StateHandler.NPCSTATE state)
        {
            if (state != NPC_StateHandler.NPCSTATE.TALKING) return;

            ChangeWeight(1);
        }

        private void Update()
        {
            _numsFound = Physics.OverlapSphereNonAlloc(_fieldOfView.position, _fofVRadius, _colliders, _playerLayer);

            if(_numsFound > 0)
            {
                if (!_targetSet)
                    LookAtPlayer(_colliders[0].GetComponent<Transform>());
                else return;
            }
            else
            {
                _targetSet = false;
                ChangeWeight(0);
            }
        }

        bool ShouldLookAtPlayer()
        {
            return true;
        }
        void LookAtPlayer(Transform target)
        {
            ChangeWeight(1);
            //WeightedTransform weightedTransform = new WeightedTransform(target,1);

            //_multiAimConstraint.data.sourceObjects.Clear();
            //_multiAimConstraint.data.sourceObjects.Add(weightedTransform);
            var data = _multiAimConstraint.data.sourceObjects;
            if(data[0].transform == null)
            {
                data.SetTransform(0, target);
                _multiAimConstraint.data.sourceObjects = data;
                _rigBuilder.Build();
            }
            
        }
        void ChangeWeight(int targetValue)
        {
            _multiAimConstraint.weight = Mathf.MoveTowards(_multiAimConstraint.weight, targetValue, _lookSpeed * Time.unscaledDeltaTime);
            //if (_multiAimConstraint.weight == _targetValue) _targetSet = true;
        }
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(_fieldOfView.position, _fofVRadius);
        }
    }
}