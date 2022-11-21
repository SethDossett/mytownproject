using MyTownProject.Events;
using MyTownProject.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace KinematicCharacterController.Examples
{
    public class ExamplePlayer : MonoBehaviour
    {
        [SerializeField] TheCharacterController CC;
        public ExampleCharacterCamera CharacterCamera;
        [SerializeField] MainEventChannelSO MainEventChannelSO;
        [SerializeField] TransformEventSO playerReference;
        Transform _player;
        Camera mainCamera;

        private const string MouseXInput = "Mouse X";
        private const string MouseYInput = "Mouse Y";
        private const string MouseScrollInput = "Mouse ScrollWheel";
        private const string HorizontalInput = "Horizontal";
        private const string VerticalInput = "Vertical";

        private PlayerInput playerInput;
        private NewControls inputActions;
        private InputAction move, cameraMove, jump, crouch, zoom, interact, start, leftTrigger;

        private bool canPressStart = true;
        private bool isPaused = false;

        private void OnEnable()
        {
            GameStateManager.OnGameStateChanged += CheckGameState;

            move = inputActions.GamePlay.Move;
            cameraMove = inputActions.GamePlay.Camera;
            interact = inputActions.GamePlay.Interact;
            crouch = inputActions.GamePlay.Crouch;
            //zoom = inputActions.GamePlay.Zoom;
            start = inputActions.GamePlay.Start;
            leftTrigger = inputActions.GamePlay.LeftTrigger;
        }
        private void OnDisable()
        {
            GameStateManager.OnGameStateChanged -= CheckGameState;

        }
        void GetPlayerReference(Transform player)
        {
            _player = player;
            CC = _player.GetComponent<TheCharacterController>();
            print($"Got Ref {gameObject.name}");
        }

        private void Awake()
        {
            print($"awake {gameObject.name}");
            playerReference.OnRaiseEvent += GetPlayerReference;
            inputActions = InputManager.inputActions;
            playerInput = GetComponent<PlayerInput>();
            mainCamera = Camera.main;
        }
        void CheckGameState(GameState state)
        {
            if (state == GameState.GAME_PLAYING)
            {
                print("Crouch input = " + crouch.ReadValue<float>());
                if (crouch.ReadValue<float>() <= 0.1f)
                    CC._shouldBeCrouching = false;
            }
            else
            {

            }
        }
        

        private void OnDestroy()
        {
            playerReference.OnRaiseEvent -= GetPlayerReference;
        }
        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;

            // Tell camera to follow transform
            //CharacterCamera.SetFollowTransform(CC.CameraFollowPoint);

            // Ignore the character's collider(s) for camera obstruction checks
            //CharacterCamera.IgnoredColliders.Clear();
            //CharacterCamera.IgnoredColliders.AddRange(CC.GetComponentsInChildren<Collider>());
        }

        private void Update()
        {
            if (canPressStart)
            {
                if (start.WasPressedThisFrame()) // could move this to Pause Menu
                {
                    MainEventChannelSO.RaiseEventPaused();
                }
            }
            HandleCharacterInput();
        }

        private void LateUpdate()
        {
            // Handle rotating the camera along with physics movers
            if (CharacterCamera.RotateWithPhysicsMover && CC.Motor.AttachedRigidbody != null)
            {
                CharacterCamera.PlanarDirection = CC.Motor.AttachedRigidbody.GetComponent<PhysicsMover>().RotationDeltaFromInterpolation * CharacterCamera.PlanarDirection;
                CharacterCamera.PlanarDirection = Vector3.ProjectOnPlane(CharacterCamera.PlanarDirection, CC.Motor.CharacterUp).normalized;
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
            Vector3 lookInputVector = new Vector3(camDir.x, camDir.y, 0f);

            // Prevent moving the camera while the cursor isn't locked
            if (Cursor.lockState != CursorLockMode.Locked)
            {
                lookInputVector = Vector3.zero;
            }

            // Input for zooming the camera (disabled in WebGL because it can cause problems)
            //float scrollInput = -Input.GetAxis(MouseScrollInput);
            //float scrollInput = -inputActions.GamePlay.Zoom.ReadValue<float>();
            float scrollInput = 0f;
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
            characterInputs.MoveDirection = move.ReadValue<Vector2>();
            //characterInputs.CameraRotation = CharacterCamera.Transform.rotation;
            characterInputs.CameraRotation = mainCamera.transform.rotation;
            //characterInputs.JumpDown = jump.WasPressedThisFrame();
            characterInputs.CrouchDown = crouch.WasPressedThisFrame();
            characterInputs.CrouchUp = crouch.WasReleasedThisFrame();
            characterInputs.Interact = interact.WasPerformedThisFrame();
            characterInputs.LeftTriggerDown = leftTrigger.WasPressedThisFrame();
            characterInputs.LeftTriggerDown = leftTrigger.WasReleasedThisFrame();

            // Apply inputs to character
            CC.SetInputs(ref characterInputs);
        }
    }
}