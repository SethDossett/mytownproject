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
        public float _downRaycastHitDis;
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

            //Vector2 value = inputActions.GamePlay.Move.ReadValue<Vector2>().magnitude;
            Vector3 input = CC._moveInputVector;
            float dot = Vector3.Dot(input, transform.forward.normalized);
            if(dot >= 0.9f){
                if(CanClimb(out _downHitInfo, out _forwardHitInfo, out _endPosition)){
                    InitiateClimb();
                }
            }
        }
        void OnAnimatorMove() {
            if(_animator.isMatchingTarget)
                _animator.ApplyBuiltinRootMotion();
        }
        //Look for ledge Raycasts
        //bool LedgeCast(){}
        bool DownCast(){
            // was downcastoffset.y - 0.4f, so it did not go lower than step height,
            // but now i want low enough that i can see if i can hang.
            return Physics.Raycast(_downOrigin, Vector3.down, out _downHitInfo, _downCastOffset.y + 1.5f, _groundLayer);
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
            if(!DownCast()){
                CC.CanHang = true;
                return false;
            }
            CC.CanHang = false; 
            _downRaycastHitDis = _downHitInfo.point.y - transform.position.y;
            if(_downRaycastHitDis <= 0.4f) return false;
            _forwardCastOffset = new Vector3(transform.position.x, _downHitInfo.point.y, transform.position.z);
            _overPassCastOffset = new Vector3(transform.position.x, transform.position.y + _overPassHeight, transform.position.z);

            _forwardDirectionXZ = Vector3.ProjectOnPlane(transform.forward,Vector3.up);
            if(!ForwardCast()) return false;
            _climbHeight = _downHitInfo.point.y - transform.position.y;    
            if(_climbHeight <= 0.4f) return false; // so that we dont try to climb if object is less than step height
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
                Vector3 upSweepOrigin = transform.position; 
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
        [SerializeField] float amount;
        void InitiateClimb(){
            CC.TransitionToState(CharacterState.Climbing);
            _isClimbing = true;
            float climbHeight = _downHitInfo.point.y - transform.position.y;
            Vector3 forwardNormalXZ = Vector3.ProjectOnPlane(_forwardHitInfo.normal, Vector3.up);
            _forwardNormalXZRotation = Quaternion.LookRotation(-forwardNormalXZ, Vector3.up);
            if(climbHeight > _hangHeight){
                //_matchTargetPosition = _forwardHitInfo.point + _forwardNormalXZRotation * _hangOffset;
                _matchTargetPosition = transform.TransformPoint(0, climbHeight - 1.21f, amount);
                _matchTargetRotation = _forwardNormalXZRotation;
                CC.Motor.Capsule.enabled = false;
                StartCoroutine(DoClimb(1, _matchTargetPosition, transform.rotation, 5));
                _animator.CrossFadeInFixedTime(anim_JumpToHang, .25f, 0);
                CC._isHanging = true;
                //_animator.MatchTarget(_matchTargetPosition, _matchTargetRotation, AvatarTarget.Root, _weightMask, 0.14f, 0.25f);
            }
            else if(climbHeight > _climbUpHeight){
                _matchTargetPosition = _endPosition;
                _matchTargetRotation = _forwardNormalXZRotation;
                StartCoroutine(DoClimb(1, _matchTargetPosition, _matchTargetRotation, 5));
                _animator.CrossFadeInFixedTime(anim_ClimbUp, 0, 0);
                //_animator.MatchTarget(_matchTargetPosition, _matchTargetRotation, AvatarTarget.Root, _weightMask, 0f, 0.23f);
            }
            else if(climbHeight > _vaultHeight){
                _matchTargetPosition = _endPosition;
                _matchTargetRotation = _forwardNormalXZRotation;
                StartCoroutine(DoClimb(1, _matchTargetPosition, _matchTargetRotation, 5));
                _animator.CrossFadeInFixedTime(anim_Vault, 0, 0);
                //_animator.MatchTarget(_matchTargetPosition, _matchTargetRotation, AvatarTarget.Root, _weightMask, 0.05f, 0.16f);
            }
            else if(climbHeight > _stepUpHeight){
                _matchTargetPosition = _endPosition;
                _matchTargetRotation = _forwardNormalXZRotation;
                StartCoroutine(DoClimb(1, _matchTargetPosition, _matchTargetRotation, 5));
                _animator.CrossFadeInFixedTime(anim_StepUp, 0, 0);
                //_animator.MatchTarget(_matchTargetPosition, _matchTargetRotation, AvatarTarget.Root, _weightMask, 0f, 0.12f);
            }
            else{
                _isClimbing = false;
            }
        }


        IEnumerator DoClimb(float loopTime, Vector3 goalPos,Quaternion goalRot, float speed){
            CC._gettingOnOffObstacle = true;
            float timer = 0;
            while(timer < loopTime){
                timer += Time.deltaTime;
                CC.Motor.SetTransientPosition(goalPos, true, speed);
                CC.Motor.SetRotation(goalRot);
                yield return null;
            }
            CC._gettingOnOffObstacle = false;
            yield break;
        }
        bool InAirCheck(){
            return false;
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
