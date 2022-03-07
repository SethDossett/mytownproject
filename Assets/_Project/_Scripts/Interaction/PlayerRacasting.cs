using UnityEngine.InputSystem;
using UnityEngine;
using MyTownProject.Core;

namespace MyTownProject.Interaction
{
    public class PlayerRacasting : MonoBehaviour
    {
        [Header("References")]
        private IInteractable currentTarget;
        private NewControls _inputActions;
        private InputAction _interact;
        private GameStateManager.GameState _game_Playing_State;
        private RaycastHit _hitinfo;
        private Transform _transform;

        [Header("Values")]
        [SerializeField] private float _interactRayLength = 5f;
        [SerializeField] private Vector3 _offset;
        private bool canRaycast = false;



        private void OnEnable()
        {
            GameStateManager.OnGameStateChanged += CheckState;
            _inputActions = new NewControls();
            _interact = _inputActions.GamePlay.Interact;
            _interact.Enable();
        }
        private void OnDisable()
        {
            GameStateManager.OnGameStateChanged -= CheckState;
            _interact.Disable();
        }
        private void Start()
        {
            _game_Playing_State = GameStateManager.GameState.GAME_PLAYING;
            _transform = transform;
        }
        void CheckState(GameStateManager.GameState state)
        {
            if(state == _game_Playing_State)
                canRaycast = true;
            else
                canRaycast = false;
        }
        private void Update()
        {
            if (!canRaycast)
                return;

            CheckForInteractable();

            if (_interact.WasPerformedThisFrame())
            {
                if (currentTarget != null)
                {
                    currentTarget.OnInteract();
                }
            }
        }
        private void CheckForInteractable()
        {
            Ray ray = new Ray(_transform.position + _offset, _transform.forward);
            Debug.DrawRay(ray.origin, ray.direction * _interactRayLength);
            Debug.DrawRay(ray.origin, ray.direction * 2f, Color.green);
            if (Physics.Raycast(ray, out _hitinfo, _interactRayLength))
            {
                ;
                IInteractable interactable = _hitinfo.collider.GetComponent<IInteractable>();
                if (interactable == null)
                {
                    if (currentTarget != null)
                    {
                        currentTarget.OnLoseFocus();
                        currentTarget = null;
                        return;
                    }
                    return;
                }

                if (!interactable.CanBeInteractedWith)
                {
                    if (currentTarget != null)
                    {
                        currentTarget.OnLoseFocus();
                        currentTarget = null;
                        return;
                    }
                    return;
                }
                if (_hitinfo.distance <= interactable.MaxRange)
                {
                    if (interactable == currentTarget)
                    {
                        return;
                    }
                    else if (currentTarget != null)
                    {
                        currentTarget.OnLoseFocus();
                        currentTarget = interactable;
                        currentTarget.OnFocus(_hitinfo.collider.gameObject.name);
                        return;
                    }
                    else
                    {
                        currentTarget = interactable;
                        currentTarget.OnFocus(_hitinfo.collider.gameObject.name);
                        return;
                    }
                }
                else
                {
                    if (currentTarget != null)
                    {
                        currentTarget.OnLoseFocus();
                        currentTarget = null;
                        return;
                    }
                }

            }
            else
            {
                if (currentTarget != null)
                {
                    currentTarget.OnLoseFocus();
                    currentTarget = null;
                    return;
                }
            }
        }
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position + _offset + transform.forward * _hitinfo.distance, 1.5f);

        }

    }
}