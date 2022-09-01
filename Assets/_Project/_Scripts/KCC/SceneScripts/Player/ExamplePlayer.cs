using MyTownProject.Events;
using UnityEngine;
using UnityEngine.InputSystem;

namespace KinematicCharacterController.Examples
{
    public class ExamplePlayer : MonoBehaviour
    {
        public TheCharacterController Character;
        public ExampleCharacterCamera CharacterCamera;
        public MainEventChannelSO MainEventChannelSO;

        [SerializeField] GeneralEventSO _enableControls;
        [SerializeField] GeneralEventSO _disableControls;

        private const string MouseXInput = "Mouse X";
        private const string MouseYInput = "Mouse Y";
        private const string MouseScrollInput = "Mouse ScrollWheel";
        private const string HorizontalInput = "Horizontal";
        private const string VerticalInput = "Vertical";

        private PlayerInput playerInput;
        private NewControls inputActions;
        private InputAction move;
        private InputAction cameraMove;
        private InputAction jump;
        private InputAction crouch;
        private InputAction zoom;
        private InputAction interact;
        private InputAction start;

        private bool canPressStart = true;
        private bool isPaused = false;

        private void OnEnable()
        {
            inputActions.GamePlay.Enable();
            _disableControls.OnRaiseEvent += DisableControls;
            _enableControls.OnRaiseEvent += EnableControls;
            //move = InputManager.inputActions.GamePlay.Move;
           //cameraMove = inputActions.GamePlay.Camera;
           //interact = inputActions.GamePlay.Interact;
           //crouch = inputActions.GamePlay.Crouch;
           //zoom = inputActions.GamePlay.Zoom;
           //start = inputActions.GamePlay.Start;
            //move.Enable();
            //cameraMove.Enable();
            //jump.Enable();
            //crouch.Enable();
            //zoom.Enable();
            //interact.Enable();
            //start.Enable();
        }
        private void OnDisable()
        {
            inputActions.GamePlay.Disable();
            _disableControls.OnRaiseEvent -= DisableControls;
            _enableControls.OnRaiseEvent -= EnableControls;
           /* move.Disable();
            cameraMove.Disable();
            //jump.Disable();
            crouch.Disable();
            zoom.Disable(); 
            interact.Disable();
            start.Disable();*/
        }
        private void Awake()
        {
            inputActions = new NewControls();
            playerInput = GetComponent<PlayerInput>();
        }
        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;

            // Tell camera to follow transform
            CharacterCamera.SetFollowTransform(Character.CameraFollowPoint);

            // Ignore the character's collider(s) for camera obstruction checks
            CharacterCamera.IgnoredColliders.Clear();
            CharacterCamera.IgnoredColliders.AddRange(Character.GetComponentsInChildren<Collider>());
        }

        private void Update()
        {
            if (canPressStart)
            {
                if (inputActions.GamePlay.Start.WasPressedThisFrame())
                {
                    MainEventChannelSO.RaiseEventPaused();
                }
            }
            HandleCharacterInput();
        }

        private void LateUpdate()
        {
            // Handle rotating the camera along with physics movers
            if (CharacterCamera.RotateWithPhysicsMover && Character.Motor.AttachedRigidbody != null)
            {
                CharacterCamera.PlanarDirection = Character.Motor.AttachedRigidbody.GetComponent<PhysicsMover>().RotationDeltaFromInterpolation * CharacterCamera.PlanarDirection;
                CharacterCamera.PlanarDirection = Vector3.ProjectOnPlane(CharacterCamera.PlanarDirection, Character.Motor.CharacterUp).normalized;
            }

            HandleCameraInput();
        }

        private void HandleCameraInput()
        {
            // Create the look input vector for the camera
            //float mouseLookAxisUp = Input.GetAxisRaw(MouseYInput);
            //float mouseLookAxisRight = Input.GetAxisRaw(MouseXInput);
            //Vector3 lookInputVector = new Vector3(mouseLookAxisRight, mouseLookAxisUp, 0f);
            Vector2 camDir = inputActions.GamePlay.Camera.ReadValue<Vector2>();
            Vector3 lookInputVector = new Vector3(camDir.x,camDir.y, 0f);

            // Prevent moving the camera while the cursor isn't locked
            if (Cursor.lockState != CursorLockMode.Locked)
            {
                lookInputVector = Vector3.zero;
            }

            // Input for zooming the camera (disabled in WebGL because it can cause problems)
            //float scrollInput = -Input.GetAxis(MouseScrollInput);
            float scrollInput = -inputActions.GamePlay.Zoom.ReadValue<float>();   
#if UNITY_WEBGL
        scrollInput = 0f;
#endif

            // Apply inputs to the camera
            CharacterCamera.UpdateWithInput(Time.deltaTime, scrollInput, lookInputVector);

            // Handle toggling zoom level
           /* if (Input.GetMouseButtonDown(1))
            {
                CharacterCamera.TargetDistance = (CharacterCamera.TargetDistance == 0f) ? CharacterCamera.DefaultDistance : 0f;
            }*/
        }

        private void HandleCharacterInput()
        {
            PlayerCharacterInputs characterInputs = new PlayerCharacterInputs();

            // Build the CharacterInputs struct
            characterInputs.MoveDirection = inputActions.GamePlay.Move.ReadValue<Vector2>();
            characterInputs.CameraRotation = CharacterCamera.Transform.rotation;
            //characterInputs.JumpDown = jump.WasPressedThisFrame();
            characterInputs.CrouchDown = inputActions.GamePlay.Crouch.WasPressedThisFrame();
            characterInputs.CrouchUp = inputActions.GamePlay.Crouch.WasReleasedThisFrame();
            characterInputs.Interact = inputActions.GamePlay.Interact.WasPerformedThisFrame();
            characterInputs.LeftTriggerDown = inputActions.GamePlay.LeftTrigger.WasPressedThisFrame();
            characterInputs.LeftTriggerDown = inputActions.GamePlay.LeftTrigger.WasReleasedThisFrame();

            // Apply inputs to character
            Character.SetInputs(ref characterInputs);
        }
        void EnableControls(){
            inputActions.Enable();
        }
        void DisableControls(){
            inputActions.Disable();
        }
    }
}