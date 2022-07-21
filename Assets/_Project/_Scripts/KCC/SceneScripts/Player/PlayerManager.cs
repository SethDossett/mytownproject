using System.Collections;
using UnityEngine.Events;
using UnityEngine;
using MyTownProject.SO;
using MyTownProject.Events;

namespace KinematicCharacterController.Examples
{
    public class PlayerManager : MonoBehaviour
    {
        [SerializeField] TransformEventSO PlayerRef;
        [SerializeField] ActionSO teleportPlayer;
        TheCharacterController cc;
        public UnityAction<TheCharacterController> OnCharacterTeleport;
        public bool isBeingTeleportedTo { get; set; }


        private void OnEnable()
        {
            teleportPlayer.OnTeleport += TeleportPlayer;
        }
        private void OnDisable()
        {
            teleportPlayer.OnTeleport -= TeleportPlayer;
        }
        private void Awake()
        {
            cc = GetComponent<TheCharacterController>();
            PlayerRef.RaiseEvent(transform);
        }
        private void TeleportPlayer(Vector3 location, Quaternion rotation)
        {
            if (!isBeingTeleportedTo)
            {
                if (cc)
                {
                    cc.Motor.SetPositionAndRotation(location, rotation);

                    if (OnCharacterTeleport != null)
                    {
                        OnCharacterTeleport(cc);
                    }
                    this.isBeingTeleportedTo = true;
                }
            }

            isBeingTeleportedTo = false;
        }

       
    }
}