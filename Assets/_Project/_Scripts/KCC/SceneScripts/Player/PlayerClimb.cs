using System.Collections;
using UnityEngine;
using MyTownProject.Core;

namespace KinematicCharacterController.Examples
{
    public class PlayerClimb : MonoBehaviour
    {
        [Header("References")]
        TheCharacterController CC;
        NewControls inputActions;
        CapsuleCollider _capsule;
        CharacterState CurrentCharacterState;
        ClimbableObj_Modifier _climbableObject;

        [Header("Raycast Checks")]
        [SerializeField] LayerMask _groundLayer;
        //If _downcastOffset.y = 2.5f, his max climb up will be 2.5f.
        [SerializeField] Vector3 _downCastOffset;
        Vector3 _downLedgeCastRayOffset = new Vector3(0, 1.8f, 0.6f);
        Vector3 _forwardCastOffset;
        Vector3 _overPassCastOffset;
        float _downCastRayLength = 1.5f;

        [Header("Positions")]
        [SerializeField] Vector3 _endOffset;
        [SerializeField] Vector3 _hangOffset;
        Vector3 _downOrigin;
        Vector3 _endPosition;

        RaycastHit _downHitInfo;
        RaycastHit _forwardHitInfo;
        RaycastHit _overPassHitInfo;

        [Header("Values")]
        [SerializeField] float _maxClimbHeight = 2.5f;
        [SerializeField] float _maxLedgeGrabHeight = 2f;
        [SerializeField] float _overPassHeight;
        float _climbHeight;
        public float _downRaycastHitDis;
        Vector3 _forwardDirectionXZ;
        Vector3 _forwardNormalXZ;
        [SerializeField] float _groudAngleMax;
        [SerializeField] float _wallAngleMax;
        float _groudAngle;
        float _wallAngle;
        [SerializeField][Range(0f, 0.6f)] float _climbBufferTime = 0.3f;
        float timer = 0;
        float _climbingTimer = 0;


        [Header("Checks")]
        public bool _isClimbing;
        bool _onGround;
        bool _crawling;

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
        int anim_Hang = Animator.StringToHash("Hang");
        int anim_JumpToHang = Animator.StringToHash("JumpToHang");
        int anim_ClimbUp = Animator.StringToHash("ClimbUp");
        int anim_Vault = Animator.StringToHash("Vault");
        int anim_StepUp = Animator.StringToHash("StepUp");
        int anim_GrabLedge = Animator.StringToHash("GrabLedge");

        private void OnEnable()
        {
            TheCharacterController.OnPlayerStateChanged += PlayerStateChange;
        }
        private void OnDisable()
        {
            TheCharacterController.OnPlayerStateChanged -= PlayerStateChange;
        }
        void Awake()
        {
            inputActions = InputManager.inputActions;
        }
        void Start()
        {
            _animator = GetComponent<Animator>();
            CC = GetComponent<TheCharacterController>();
            _capsule = GetComponent<CapsuleCollider>();
            _onGround = true;
        }
        void PlayerStateChange(CharacterState state)
        {
            CurrentCharacterState = state;
            if (state == CharacterState.Default || state == CharacterState.CutsceneControl)
            {
                _onGround = true;
                _isClimbing = false;
                _crawling = false;
            }
            else if (state == CharacterState.Jumping)
            {
                _onGround = false;
                _isClimbing = false;
                _crawling = false;
            }
            else if (state == CharacterState.Crawling)
            {
                _onGround = true;
                _isClimbing = false;
                _crawling = true;
            }
            else
            {
                _isClimbing = true;
                _crawling = false;
            }
        }
        void Update()
        {
            if (_isClimbing) return;

            if (_onGround) GroundCheck(); else DetectLedge();

        }
        void GroundCheck()
        {
            if (!_crawling)
            {
                Vector3 input = CC._moveInputVector;
                float dot = Vector3.Dot(input, transform.forward.normalized);
                if (dot >= 0.8f)
                {
                    if (CanClimb(out _downHitInfo, out _forwardHitInfo, out _endPosition))
                    {
                        InitiateClimb();
                        print("Initiate");
                    }
                    else timer = 0;
                }

            }

        }

        bool DownCast()
        {
            // was downcastoffset.y - 0.4f, so it did not go lower than step height,
            // but now i want low enough that i can see if i can hang.
            return Physics.Raycast(_downOrigin, Vector3.down, out _downHitInfo, _downCastOffset.y + _downCastRayLength, _groundLayer);
        }
        bool ForwardCast()
        {
            return Physics.Raycast(_forwardCastOffset, transform.forward, out _forwardHitInfo, 4f, _groundLayer);
        }
        bool OverPassCast()
        {
            return Physics.Raycast(_overPassCastOffset, transform.forward, out _overPassHitInfo, 4f, _groundLayer);
        }

        public bool CanClimb(out RaycastHit downRaycastHit, out RaycastHit forwardRaycastHit, out Vector3 endPosition)
        {
            endPosition = Vector3.zero;
            downRaycastHit = new RaycastHit();
            forwardRaycastHit = new RaycastHit();

            _downOrigin = transform.TransformPoint(_downCastOffset);
            if (!DownCast())
            {
                _climbableObject = null;
                CC.CanHang = true;
                return false;
            }
            //Check to See if object can be climbed
            _climbableObject = _downHitInfo.collider.GetComponent<ClimbableObj_Modifier>();
            if (_climbableObject != null && !_climbableObject.Climbable) return false;

            CC.CanHang = false;
            _downRaycastHitDis = _downHitInfo.point.y - transform.position.y;
            if (_downRaycastHitDis <= 0.4f) return false;
            _forwardCastOffset = new Vector3(transform.position.x, _downHitInfo.point.y, transform.position.z);
            _overPassCastOffset = new Vector3(transform.position.x, transform.position.y + _overPassHeight, transform.position.z);

            _forwardDirectionXZ = Vector3.ProjectOnPlane(transform.forward, Vector3.up);
            if (!ForwardCast()) return false;
            _climbHeight = _downHitInfo.point.y - transform.position.y;
            if (_climbHeight <= 0.4f || _climbHeight > _maxClimbHeight) return false; // so that we dont try to climb if object is less than step height
            if (OverPassCast() || _climbHeight < _overPassHeight)
            {

                _forwardNormalXZ = Vector3.ProjectOnPlane(_forwardHitInfo.normal, Vector3.up);
                _groudAngle = Vector3.Angle(_downHitInfo.normal, Vector3.up);
                _wallAngle = Vector3.Angle(-_forwardNormalXZ, _forwardDirectionXZ);

                if (_wallAngle > _wallAngleMax) return false;
                if (_groudAngle > _groudAngleMax) return false;
                Vector3 vectSurface = Vector3.ProjectOnPlane(_forwardDirectionXZ, _downHitInfo.normal);
                _endPosition = _downHitInfo.point + Quaternion.LookRotation(vectSurface, Vector3.up) * _endOffset;
                //De-Penetration
                Collider colliderB = _downHitInfo.collider;
                bool _penetrationOverlap = Physics.ComputePenetration(
                    _capsule, _endPosition, transform.rotation, colliderB, colliderB.transform.position,
                    colliderB.transform.rotation, out Vector3 penetrationDir, out float penetrationDis);
                if (_penetrationOverlap)
                    _endPosition += penetrationDir * penetrationDis;

                //Up Sweep
                float inflate = -0.05f;
                float upSweepDistance = (_downHitInfo.point.y - transform.position.y);
                Vector3 upSweepDirection = transform.up;
                Vector3 upSweepOrigin = transform.position;
                bool upSweepHit = CharacterSweep(upSweepOrigin, transform.rotation, upSweepDirection, upSweepDistance, _groundLayer, inflate);
                if (upSweepHit) print("Upsweep");
                //Forward Sweep
                Vector3 forwardSweepOrigin = transform.position + upSweepDirection * upSweepDistance;
                Vector3 forwardSweepVector = _endPosition - forwardSweepOrigin;
                bool forwardSweepHit = CharacterSweep(forwardSweepOrigin, transform.rotation, forwardSweepVector.normalized, forwardSweepVector.magnitude, _groundLayer, inflate);

                if (!upSweepHit && !forwardSweepHit)
                {
                    timer += Time.deltaTime;
                    if (timer >= _climbBufferTime)
                    {
                        timer = 0;
                        endPosition = _endPosition;
                        downRaycastHit = _downHitInfo;
                        forwardRaycastHit = _forwardHitInfo;
                        print("CanClimb");
                        return true;
                    }
                }
                else timer = 0;

            }
            return false;
        }
        bool CharacterSweep(Vector3 pos, Quaternion rot, Vector3 dir, float dis, LayerMask layerMask, float inflate)
        {
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
        [SerializeField] float amount; // set distance and dispose variable
        void InitiateClimb()
        {
            CC.TransitionToState(CharacterState.Climbing);
            _isClimbing = true;
            float climbHeight = _downHitInfo.point.y - transform.position.y;
            _matchTargetRotation = FindLedgeNormal(_forwardHitInfo);
            if (climbHeight > _hangHeight)
            {
                //_matchTargetPosition = _forwardHitInfo.point + _forwardNormalXZRotation * _hangOffset;
                _matchTargetPosition = transform.TransformPoint(0, climbHeight - 1.21f, amount);
                StartCoroutine(DoClimb(0.4f, 0.6f, _matchTargetPosition, _matchTargetRotation, 2.5f, true));
                _animator.CrossFadeInFixedTime(anim_JumpToHang, .25f, 0);
                CC._isHanging = true;
                //_animator.MatchTarget(_matchTargetPosition, _matchTargetRotation, AvatarTarget.Root, _weightMask, 0.14f, 0.25f);
            }
            else if (climbHeight > _climbUpHeight)
            {
                _matchTargetPosition = _endPosition;
                StartCoroutine(DoClimb(0.15f, 0.65f, _matchTargetPosition, _matchTargetRotation, 2.5f, true));
                _animator.CrossFadeInFixedTime(anim_ClimbUp, 0, 0);
                //_animator.MatchTarget(_matchTargetPosition, _matchTargetRotation, AvatarTarget.Root, _weightMask, 0f, 0.23f);
            }
            else if (climbHeight > _vaultHeight)
            {
                _matchTargetPosition = _endPosition;
                StartCoroutine(DoClimb(0.1f, 0.6f, _matchTargetPosition, _matchTargetRotation, 3.5f));
                _animator.CrossFadeInFixedTime(anim_Vault, 0, 0);
                //_animator.MatchTarget(_matchTargetPosition, _matchTargetRotation, AvatarTarget.Root, _weightMask, 0.05f, 0.16f);
            }
            else if (climbHeight > _stepUpHeight)
            {
                _matchTargetPosition = _endPosition;
                StartCoroutine(DoClimb(0.1f, 0.4f, _matchTargetPosition, _matchTargetRotation, 4));
                _animator.CrossFadeInFixedTime(anim_StepUp, 0.1f, 0);
                //_animator.MatchTarget(_matchTargetPosition, _matchTargetRotation, AvatarTarget.Root, _weightMask, 0f, 0.12f);
            }
            else
            {
                _isClimbing = false;
            }
        }


        IEnumerator DoClimb(float waitTime, float loopTime, Vector3 goalPos, Quaternion goalRot, float speed, bool controlCapsule = false)
        {
            //Disable Controls with gettingOnOffObstacle
            CC._gettingOnOffObstacle = true;
            _climbingTimer = 0;
            //How much time before we start moving Character
            yield return new WaitForSeconds(waitTime);
            //If true we will disable collider while animation is playing
            if (controlCapsule) CC.CapsuleEnable(false);
            // Move Character, Loop time is how long animation takes
            while (_climbingTimer < loopTime)
            {
                _climbingTimer += Time.deltaTime;
                CC.Motor.SetTransientPosition(goalPos, true, speed);
                //CC.Motor.SetPosition(goalPos);
                CC.Motor.SetRotation(goalRot);
                yield return null;
            }
            //Enable Controls with gettingOnOffObstacle
            CC._gettingOnOffObstacle = false;
            // If true we will enable collider after animation, unless isHanging
            if (controlCapsule)
            {
                if (!CC._isHanging)
                    CC.CapsuleEnable(true);
            }
            _climbingTimer = 0;
            if (!CC._isHanging)
            {
                _isClimbing = false;
                CC.TransitionToState(CharacterState.Default);
            }
            yield break;
        }

        void DetectLedge()
        { //stops working after climbUp maybe doclimb etc.
            if (!CC._isFalling) return;
            //downcast if there is a ledge infront of player
            //Could Lower on Y, so how doesnt grab ledge when it is over his head/arm length, so it looks more natural.
            // This will bring his max grab height down. At Down Cast Y = 1.8, he can grab 2.5f above Ground sometimes.
            _downOrigin = transform.TransformPoint(_downLedgeCastRayOffset);
            if (!DownCast()) return;

            //Send forwardcast to see what angle player is facing
            _forwardCastOffset = new Vector3(transform.position.x, _downHitInfo.point.y - 0.01f, transform.position.z);
            if (!ForwardCast()) return;
            _forwardDirectionXZ = Vector3.ProjectOnPlane(transform.forward, Vector3.up);
            _forwardNormalXZ = Vector3.ProjectOnPlane(_forwardHitInfo.normal, Vector3.up);
            _wallAngle = Vector3.Angle(-_forwardNormalXZ, _forwardDirectionXZ);
            if (_wallAngle > _wallAngleMax + 5) return;

            //check distance from players feet to ledge
            _downRaycastHitDis = _downHitInfo.point.y - transform.position.y;
            if (_downRaycastHitDis > _maxLedgeGrabHeight) return;
            if (_downRaycastHitDis > 0.6f)
            {
                print("grab ledge");
                Quaternion ledgeNormalRot = FindLedgeNormal(_forwardHitInfo);
                GrabLedge(ledgeNormalRot);
                return;
            }

            return;
        }
        void GrabLedge(Quaternion wallRotation)
        {
            CC.TransitionToState(CharacterState.Climbing);
            _isClimbing = true;
            _matchTargetPosition = transform.TransformPoint(0, _downRaycastHitDis - 1.21f, amount);
            CC.CapsuleEnable(false);
            StartCoroutine(DoClimb(0, 0.5f, _matchTargetPosition, wallRotation, 3f));
            _animator.CrossFadeInFixedTime(anim_GrabLedge, .25f, 0);
            CC._isHanging = true;
        }

        public void ClimbBackUp() => StartCoroutine(DoClimbBackUp()); // climbing back up rotation is different(DropToHang, InitiateClimb, Grab Ledge.
        IEnumerator DoClimbBackUp() // whenever we climb back up we can shoot ray to see normal of wall and rotate there.
        {
            print("Booya");
            //Disable Controls
            CC._gettingOnOffObstacle = true;
            CC._isHanging = false;
            Vector3 goalPos = transform.TransformPoint(0, 1.3f, 0.3f);

            _forwardCastOffset = new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z);
            if (!ForwardCast()) Debug.LogError("Forward Cast is not hitting wall");
            Quaternion goalRot = FindLedgeNormal(_forwardHitInfo);
            float timer = 0;
            while (timer < 0.75f)
            {
                timer += Time.deltaTime;
                CC.Motor.SetTransientPosition(goalPos, true, 1.8f);
                CC.Motor.SetRotation(goalRot);
                yield return null;
            }
            print("done2");
            CC.CapsuleEnable(true);
            //Moveinputvector needs to be 0 at this point.
            CC.TransitionToState(CharacterState.Default);
            _isClimbing = false;
            CC._gettingOnOffObstacle = false;
            yield break;
        }

        Quaternion FindLedgeNormal(RaycastHit hit)
        {
            Vector3 forwardNormalXZ = Vector3.ProjectOnPlane(hit.normal, Vector3.up);
            _forwardNormalXZRotation = Quaternion.LookRotation(-forwardNormalXZ, Vector3.up);

            return _forwardNormalXZRotation;
        }
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.TransformPoint(_downCastOffset), Vector3.down * (_downCastOffset.y + _downCastRayLength));
            Gizmos.DrawRay(_forwardCastOffset, transform.forward * 4f);
            Gizmos.DrawRay(_overPassCastOffset, transform.forward * 4f);

        }
    }

}
