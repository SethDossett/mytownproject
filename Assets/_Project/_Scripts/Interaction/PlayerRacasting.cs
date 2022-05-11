using UnityEngine.InputSystem;
using UnityEngine;
using MyTownProject.Core;
using MyTownProject.Events;

namespace MyTownProject.Interaction
{
    public class PlayerRacasting : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] UIEventChannelSO uiEventChannel;
        private IInteractable _interactable;
        private NewControls _inputActions;
        private InputAction _interact;
        private GameStateManager.GameState _game_Playing_State;
        private RaycastHit _hitinfo;
        private Transform _transform;
        [SerializeField] Transform _interactionPoint;
        

        [Header("Values")]
        [SerializeField] float _interactionPointRadius;
        [SerializeField] LayerMask _interactableMask;
        [SerializeField] private float _interactRayLength = 5f;
        [SerializeField] private Vector3 _offset;
        private bool canRaycast = false;
        private readonly Collider[] _colliders = new Collider[3];
        [SerializeField] private int _numFound;



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

            //CheckForInteractable();
            Interactor();
            
        }
        private void CheckForInteractable()
        {
            Ray ray = new Ray(_transform.position + _offset, _transform.forward);
            Debug.DrawRay(ray.origin, ray.direction * _interactRayLength);
            Debug.DrawRay(ray.origin, ray.direction * 2f, Color.green);
            if (Physics.Raycast(ray, out _hitinfo, _interactRayLength))
            {
                
                _interactable = _hitinfo.collider.GetComponent<IInteractable>();
                if(_interactable == null)
                {
                    if (_interactable != null)
                    {
                        _interactable.OnLoseFocus();
                        uiEventChannel.HideTextInteract();
                        _interactable = null;
                        return;
                    }
                    return;
                }

                if (!_interactable.CanBeInteractedWith || _hitinfo.distance > _interactable.MaxRange)
                {
                    if (_interactable != null)
                    {
                        _interactable.OnLoseFocus();
                        uiEventChannel.HideTextInteract();
                        _interactable = null;
                        return;
                    }
                    return;
                }

                uiEventChannel.ShowTextInteract(_interactable.Prompt);
                //currentTarget.OnFocus(_hitinfo.collider.gameObject.name);

                if (_interact.WasPerformedThisFrame()) _interactable.OnInteract(this);

            }
            else
            {
                if (_interactable != null) _interactable = null;
                
                uiEventChannel.HideTextInteract();
            }
        }

        private void Interactor()
        {
            _numFound = Physics.OverlapSphereNonAlloc(_interactionPoint.position, _interactionPointRadius, _colliders, _interactableMask);

            
            if (_numFound > 0)
            {
                _interactable = _colliders[0].GetComponentInParent<IInteractable>();

                uiEventChannel.ShowTextInteract(_interactable.Prompt);

                if (_interact.WasPerformedThisFrame()) _interactable.OnInteract(this);
            }
            else
            {
                if (_interactable != null) _interactable = null;

                uiEventChannel.HideTextInteract();
            }
        }
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            //Gizmos.DrawWireSphere(transform.position + _offset + transform.forward * _hitinfo.distance, 1.5f);
            Gizmos.DrawWireSphere(_interactionPoint.position, _interactionPointRadius);

        }

    }
}