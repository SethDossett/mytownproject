using UnityEngine.InputSystem;
using UnityEngine;

public class PlayerRacasting : MonoBehaviour
{
    private IInteractable currentTarget;
    private NewControls _inputActions;
    private InputAction _interact;

    [Header("Attributes")]
    [SerializeField] private float _interactRayLength = 5f;
    private RaycastHit _hitinfo;
    [SerializeField] private Vector3 _offset;

    private GameManager gameManager;
    private GameManager.GameState _game_Playing_State;
     

    private void OnEnable()
    {
        _inputActions = new NewControls();
        _interact = _inputActions.GamePlay.Interact;
        _interact.Enable();
    }
    private void OnDisable()
    {
        _interact.Disable();
    }
    private void Start()
    {
        gameManager = GameManager.instance;
        _game_Playing_State = GameManager.GameState.GAME_PLAYING;
    }
    private void Update()
    {
        if (gameManager.gameState != _game_Playing_State)
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
        Ray ray = new Ray(transform.position + _offset, transform.forward);
        Debug.DrawRay(ray.origin, ray.direction * _interactRayLength);
        Debug.DrawRay(ray.origin, ray.direction * 2f, Color.green);
        if (Physics.SphereCast(transform.position + _offset, 1.5f, transform.forward, out _hitinfo, _interactRayLength))
        {
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
                    currentTarget.OnFocus(name);
                    return;
                }
                else
                {
                    currentTarget = interactable;
                    currentTarget.OnFocus(name);
                    return;
                }
            }
            else
            {
                if(currentTarget != null)
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
