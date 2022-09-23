using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace KinematicCharacterController.Examples{
    public class PlayerClimb : MonoBehaviour
    {
        [Header("References")]
        TheCharacterController CC;
        NewControls inputActions;
        CapsuleCollider _capsule;

        [Header("Raycast Checks")]
        [SerializeField] LayerMask _groundLayer;
        [SerializeField] Vector3 _downCastOffset;
        Vector3 _forwardCastOffset;
        Vector3 _overPassCastOffset;

        [Header("Positions")]
        [SerializeField] Vector3 _endOffset;
        [SerializeField] Vector3 _hangOffset;
        Vector3 _downOrigin;
        Vector3 _endPosition;

        RaycastHit _downHitInfo;
        RaycastHit _forwardHitInfo;
        RaycastHit _overPassHitInfo;

        [Header("Values")]
        public bool _isClimbing;
        [SerializeField] float _overPassHeight;
        float _climbHeight;
        Vector3 _forwardDirectionXZ;
        Vector3 _forwardNormalXZ;
        [SerializeField] float _groudAngleMax;
        [SerializeField] float _wallAngleMax;
        float _groudAngle;
        float _wallAngle;
        

        [Header("Heights")]
        [SerializeField] float _hangHeight = 1.5f;
        [SerializeField] float _climbUpHeight = 0.9f;
        [SerializeField] float _vaultHeight = 0.4f;
        [SerializeField] float _stepUpHeight = 0.25f;


        [Header("Animations")]
        Animator _animator;
        Vector3 _matchTargetPosition;
        Quaternion _matchTargetRotation;
        Quaternion _forwardNormalXZRotation;
        MatchTargetWeightMask _weightMask = new MatchTargetWeightMask(Vector3.one, 1);
        int anim_Hang= Animator.StringToHash("Hang");
        int anim_JumpToHang= Animator.StringToHash("JumpToHang");
        int anim_ClimbUp = Animator.StringToHash("ClimbUp");
        int anim_Vault = Animator.StringToHash("Vault");
        int anim_StepUp = Animator.StringToHash("StepUp");
        int anim_FreeHangDrop = Animator.StringToHash("FreeHangDrop");
        int anim_HangClimbUp = Animator.StringToHash("HangClimbUp");



        private void OnEnable() {
            inputActions.GamePlay.Enable();
        }
        private void OnDisable() {
            inputActions.GamePlay.Enable();
        }
        void Awake(){
            inputActions = new NewControls();
        }
        void Start(){
            _animator = GetComponent<Animator>();
            CC = GetComponent<TheCharacterController>();
            _capsule = GetComponent<CapsuleCollider>();
        }
        void Update()
        {
            if(_isClimbing) return;

            float value = inputActions.GamePlay.Move.ReadValue<Vector2>().magnitude;
            if(value > 0){
                if(CanClimb(out _downHitInfo, out _forwardHitInfo, out _endPosition)){
                    
                    float climbHeight = _downHitInfo.point.y - transform.position.y;
                    Vector3 forwardNormalXZ = Vector3.ProjectOnPlane(_forwardHitInfo.normal, Vector3.up);
                    _forwardNormalXZRotation = Quaternion.LookRotation(-forwardNormalXZ, Vector3.up);

                    if(climbHeight > _hangHeight){
                        _isClimbing = true;
                        _matchTargetPosition = _forwardHitInfo.point + _forwardNormalXZRotation * _hangOffset;
                        _matchTargetRotation = _forwardNormalXZRotation;
                        //CC.TransitionToState(CharacterState.Jumping);
                        CC.Motor.ForceUnground();
                        CC.Motor.SetTransientPosition(_matchTargetPosition,true, 5);
                        _animator.CrossFadeInFixedTime(anim_JumpToHang, 0, 0);
                        //_animator.MatchTarget(_matchTargetPosition, _matchTargetRotation, AvatarTarget.Root, _weightMask, 0.14f, 0.25f);
                    }
                    else if(climbHeight > _climbUpHeight){
                        _isClimbing = true;
                        _matchTargetPosition = _endPosition;
                        _matchTargetRotation = _forwardNormalXZRotation;
                        //CC.TransitionToState(CharacterState.Jumping);
                        CC.Motor.ForceUnground();
                        CC.Motor.SetTransientPosition(_matchTargetPosition,true, 5);
                        _animator.CrossFadeInFixedTime(anim_ClimbUp, 0, 0);
                        //_animator.MatchTarget(_matchTargetPosition, _matchTargetRotation, AvatarTarget.Root, _weightMask, 0f, 0.23f);
                    }
                    else if(climbHeight > _vaultHeight){
                        _isClimbing = true;
                        _matchTargetPosition = _endPosition;
                        _matchTargetRotation = _forwardNormalXZRotation;
                        //CC.TransitionToState(CharacterState.Jumping);
                        CC.Motor.ForceUnground();
                        CC.Motor.SetTransientPosition(_matchTargetPosition,true, 5);
                        _animator.CrossFadeInFixedTime(anim_Vault, 0, 0);
                        //_animator.MatchTarget(_matchTargetPosition, _matchTargetRotation, AvatarTarget.Root, _weightMask, 0.05f, 0.16f);
                    }
                    else if(climbHeight > _stepUpHeight){
                        _isClimbing = true;
                        _matchTargetPosition = _endPosition;
                        _matchTargetRotation = _forwardNormalXZRotation;
                        //CC.TransitionToState(CharacterState.Jumping);
                        CC.Motor.ForceUnground();
                        CC.Motor.SetTransientPosition(_matchTargetPosition,true, 5);
                        _animator.CrossFadeInFixedTime(anim_StepUp, 0, 0);
                        //_animator.MatchTarget(_matchTargetPosition, _matchTargetRotation, AvatarTarget.Root, _weightMask, 0f, 0.12f);
                    }
                    else{
                        _isClimbing = false;
                    }
                    
                }
                

            }
            
            
        }
        void OnAnimatorMove() {
            if(_animator.isMatchingTarget)
                _animator.ApplyBuiltinRootMotion();
        }

        bool DownCast(){
            return Physics.Raycast(_downOrigin, Vector3.down, out _downHitInfo, _downCastOffset.y - 0.4f, _groundLayer);
        }
        bool ForwardCast(){
            return Physics.Raycast(_forwardCastOffset, transform.forward, out _forwardHitInfo, 5f, _groundLayer);
        }
        bool OverPassCast(){
             return Physics.Raycast(_overPassCastOffset, transform.forward, out _overPassHitInfo, 5f, _groundLayer);
        }

        public bool CanClimb(out RaycastHit downRaycastHit, out RaycastHit forwardRaycastHit, out Vector3 endPosition){
            endPosition = Vector3.zero;
            downRaycastHit = new RaycastHit();
            forwardRaycastHit = new RaycastHit();

            _downOrigin = transform.TransformPoint(_downCastOffset);
            if(!DownCast()) return false;
            _forwardCastOffset = new Vector3(transform.position.x, _downHitInfo.point.y, transform.position.z);
            _overPassCastOffset = new Vector3(transform.position.x, transform.position.y + _overPassHeight, transform.position.z);

            _forwardDirectionXZ = Vector3.ProjectOnPlane(transform.forward,Vector3.up);

            if(!ForwardCast()) return false;
            _climbHeight = _downHitInfo.point.y - transform.position.y;    
            
            if(OverPassCast() || _climbHeight < _overPassHeight){
                
                _forwardNormalXZ = Vector3.ProjectOnPlane(_forwardHitInfo.normal, Vector3.up);
                _groudAngle = Vector3.Angle(_downHitInfo.normal,Vector3.up);
                _wallAngle = Vector3.Angle(-_forwardNormalXZ,_forwardDirectionXZ);

                if(_wallAngle > _wallAngleMax) return false;
                if(_groudAngle > _groudAngleMax) return false;
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
                    endPosition = _endPosition;
                    downRaycastHit = _downHitInfo;
                    forwardRaycastHit = _forwardHitInfo;
                    print("CanClimb");
                    return true;
                }

            }
            return false;
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
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.TransformPoint(_downCastOffset), Vector3.down * (_downCastOffset.y - 0.4f));
            Gizmos.DrawRay(_forwardCastOffset, transform.forward * 5f);
            Gizmos.DrawRay(_overPassCastOffset, transform.forward * 5f);

        }
    }

}
