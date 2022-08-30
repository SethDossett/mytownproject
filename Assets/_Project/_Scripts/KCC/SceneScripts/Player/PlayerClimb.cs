using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace KinematicCharacterController.Examples{
    public class PlayerClimb : MonoBehaviour
    {
        TheCharacterController CC;
        CapsuleCollider _capsule;

        [Header("Raycast Checks")]
        [SerializeField] LayerMask _groundLayer;
        [SerializeField] Vector3 _downCastOffset;
        [SerializeField] Vector3 _forwardCastOffset;
        [SerializeField] Vector3 _overPassCastOffset;

        RaycastHit _downHitInfo;
        RaycastHit _forwardHitInfo;
        RaycastHit _overPassHitInfo;

        [SerializeField] float _overPassHeight;
        float _climbHeight;
        Vector3 _forwardDirectionXZ;
        Vector3 _forwardNormalXZ;
        Vector3 _endPosition;
        [SerializeField] Vector3 _endOffset;

        [SerializeField] float _groudAngleMax;
        [SerializeField] float _wallAngleMax;
        float _groudAngle;
        float _wallAngle;
        void Start(){
            CC = GetComponent<TheCharacterController>();
        }
        void Update()
        {
            if(!DownCast()) return;
            _forwardCastOffset = new Vector3(transform.position.x, _downHitInfo.point.y, transform.position.z);
            _overPassCastOffset = new Vector3(transform.position.x, _overPassHeight, transform.position.z);

            _forwardDirectionXZ = Vector3.ProjectOnPlane(transform.forward,Vector3.up);

            if(!ForwardCast()) return;
            _climbHeight = _downHitInfo.point.y - transform.position.y;    
            
            if(OverPassCast() || _climbHeight < _overPassHeight){
                _forwardNormalXZ = Vector3.ProjectOnPlane(_forwardHitInfo.normal, Vector3.up);
                _groudAngle = Vector3.Angle(_downHitInfo.normal,Vector3.up);
                _wallAngle = Vector3.Angle(-_forwardNormalXZ,_forwardDirectionXZ);

                if(_wallAngle > _wallAngleMax) return;
                if(_groudAngle > _groudAngleMax) return;
                Vector3 vectSurface = Vector3.ProjectOnPlane(_forwardDirectionXZ, _downHitInfo.normal);
                _endPosition = _downHitInfo.point + Quaternion.LookRotation(vectSurface, Vector3.up) * _endOffset;

                //De-Penetration
                Collider colliderB = _downHitInfo.collider;
                bool _penetrationOverlap = Physics.ComputePenetration(
                    _capsule, _endPosition, transform.rotation, colliderB, colliderB.transform.position,
                    colliderB.transform.rotation, out Vector3 penetrationDir, out float penetrationDis);
                if(_penetrationOverlap)
                    _endPosition += penetrationDir * penetrationDis;

                //Up Sweep
                float inflate = -0.05f;
                float upSweepDistance = _downHitInfo.point.y - transform.position.y;
                Vector3 upSweepDirection = transform.up;
                Vector3 upSweepOrigin = transform.position; // this may not be right spot
                bool upSweepHit = CharacterSweep(upSweepOrigin, transform.rotation, upSweepDirection, upSweepDistance, _groundLayer, inflate);

                //Forward Sweep
                Vector3 forwardSweepOrigin = transform.position + upSweepDirection * upSweepDistance;
                Vector3 forwardSweepVector = _endPosition - forwardSweepOrigin;
                bool forwardSweepHit = CharacterSweep(forwardSweepOrigin, transform.rotation, forwardSweepVector.normalized, forwardSweepVector.magnitude, _groundLayer, inflate);

                if(!upSweepHit && !forwardSweepHit)
                {
                    // Can Climb
                    print("CanClimb");
                }

            }
            
        }

        bool DownCast(){
            return Physics.Raycast(transform.position + _downCastOffset, Vector3.down, out _downHitInfo, _downCastOffset.y - 0.4f, _groundLayer);
        }
        bool ForwardCast(){
            return Physics.Raycast(transform.position + _forwardCastOffset, transform.forward, out _forwardHitInfo, 5f, _groundLayer);
        }
        bool OverPassCast(){
             return Physics.Raycast(transform.position + _overPassCastOffset, transform.forward, out _overPassHitInfo, 5f, _groundLayer);
        }

        bool CharacterSweep(Vector3 pos, Quaternion rot, Vector3 dir, float dis, LayerMask layerMask, float inflate){
            float heightScale = Mathf.Abs(transform.lossyScale.y);
            float radiusScale = Mathf.Max(Mathf.Abs(transform.lossyScale.x), Mathf.Abs(transform.lossyScale.z));

            float radius = _capsule.radius * radiusScale;
            float totalHeight = Mathf.Max(_capsule.height * heightScale, radius * 2);

            Vector3 capsuleUp = rot * Vector3.up;
            Vector3 center = pos + rot * _capsule.center;
            Vector3 top = center + capsuleUp * (totalHeight / 2 - radius);
            Vector3 bottom = center - capsuleUp * (totalHeight / 2 - radius);

            bool sweepHit = Physics.CapsuleCast(bottom, top, radius, dir, dis, layerMask);

            return sweepHit;
        }

        private void OnDrawGizmos()
        {
            
        }
    }

}
