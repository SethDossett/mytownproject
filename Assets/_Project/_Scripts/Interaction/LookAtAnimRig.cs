using UnityEngine.Animations.Rigging;
using UnityEngine;
using MyTownProject.NPC;

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
        private readonly Collider[] _colliders = new Collider[3];

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
        }
        private void OnDisable()
        {
        }
        void ChangedState()
        {
            //If Player is Looked at
            ChangeWeight(1);
        }

        private void Update()
        {
            _numsFound = Physics.OverlapSphereNonAlloc(_fieldOfView.position, _fofVRadius, _colliders, _playerLayer);

            if(_numsFound > 0)
            {
                if (!_targetSet)
                    LookAtPlayer(_colliders[0].transform);
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