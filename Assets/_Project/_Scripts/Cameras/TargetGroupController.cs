using MyTownProject.SO;
using UnityEngine;
using Cinemachine;
using MyTownProject.Events;
using KinematicCharacterController.Examples;
using MyTownProject.NPC;
using MyTownProject.Enviroment;
using MyTownProject.Interaction;

namespace MyTownProject.Cameras
{
    public class TargetGroupController : MonoBehaviour
    {
        [SerializeField] DialogueEventsSO DialogueEvents;
        [SerializeField] TransformEventSO playerRef;
        [SerializeField] TransformEventSO _targetingEvent;
        [SerializeField] TransformEventSO _changeToNextTarget;
        [SerializeField] GeneralEventSO _unTargetingEvent;
        [SerializeField] ActionSO _openDoorEvent;
        CinemachineTargetGroup targetGroup;
        [SerializeField] GameObject _player;
        void Awake()
        {
            print($"awake {gameObject.name}");
            playerRef.OnRaiseEvent += SetPlayerReference;
        }
        void OnDestroy()
        {
            playerRef.OnRaiseEvent -= SetPlayerReference;
        }
        void OnEnable()
        {
            targetGroup = GetComponent<CinemachineTargetGroup>();
            DialogueEvents.onEnter += TalkingToNPC;
            DialogueEvents.onExit += BackToPlayerView;
            _targetingEvent.OnRaiseEvent += Targeting;
            _changeToNextTarget.OnRaiseEvent += ChangeTarget;
            _unTargetingEvent.OnRaiseEvent += BackToPlayerView;
            _openDoorEvent.OnOpenDoor += OpeningDoor;
        }
        void OnDisable()
        {
            DialogueEvents.onEnter -= TalkingToNPC;
            DialogueEvents.onExit -= BackToPlayerView;
            _targetingEvent.OnRaiseEvent -= Targeting;
            _changeToNextTarget.OnRaiseEvent = ChangeTarget;
            _unTargetingEvent.OnRaiseEvent -= BackToPlayerView;
            _openDoorEvent.OnOpenDoor -= OpeningDoor;
        }

        void SetPlayerReference(Transform player)
        {
            _player = player.gameObject;
            print($"Got Ref {gameObject.name}");
            RemoveTargets();
            AddPlayer();
        }

        void Start()
        {
        }

        void AddPlayer()
        {
            Transform lookAtPoint = _player.GetComponent<PlayerManager>()._LookAtPoint;
            AddingMember(lookAtPoint, 1, 3);
        }

        void TalkingToNPC(GameObject go, TextAsset text)
        {
            Transform lookAtPoint = go.GetComponent<NPC_Manager>()._head.transform;
            AddingMember(lookAtPoint, 1, 3);

        }
        void Targeting(Transform t)
        {
            Transform toTarget = t.gameObject.GetComponent<NPC_Manager>()._head.transform;
            AddingMember(toTarget, 1, 3);
        }
        void ChangeTarget(Transform newtarget)
        {
            RemoveTargets();
            Transform toTarget = newtarget.gameObject.GetComponent<NPC_Manager>()._head.transform;
            AddingMember(toTarget, 1, 3);
            AddPlayer();
        }
        void OpeningDoor(DoorType doorType, GameObject door)
        {
            RemoveTargets();
            AddingMember(door.GetComponent<Door>().LookAtPosition, 1, 3);
        }

        void BackToPlayerView()
        {
            RemoveTargets();
            AddPlayer();
        }

        void RemoveTargets()
        {
            foreach (var target in targetGroup.m_Targets)
            {
                targetGroup.RemoveMember(target.target);
            }
        }

        void AddingMember(Transform t, float weight, float radius)
        {
            targetGroup.AddMember(t, weight, radius);
        }





    }
}