using UnityEngine;
using MyTownProject.Interaction;
using System.Threading.Tasks;

namespace MyTownProject.Enviroment
{
    public class DoorAnimator : MonoBehaviour
    {
        [SerializeField] DoorType CurrentDoorType;
        Door _door;
        Animator _animator;
        string _npcTag = "NPC";
        int pullOpenWide = Animator.StringToHash("PullOpenWide");
        int pushOpenWide = Animator.StringToHash("PushOpenWide");
        int pullCloseWide = Animator.StringToHash("PullCloseWide");
        int pushCloseWide = Animator.StringToHash("PushCloseWide");
        int _animationHashOpen = 0;
        int _animationHashClose = 0;

        void Start()
        {
            _door = GetComponent<Door>();
            _animator = GetComponent<Animator>();
        }

    void GetAnimationHash(DoorType doorType)
        {
            
            switch (doorType)
            {
                case DoorType.PushDoorR:{
                    _animationHashOpen = pushOpenWide;
                    _animationHashClose = pullCloseWide;
                    break;
                }
                case DoorType.PullDoorR:{
                    _animationHashOpen = pullCloseWide;
                    _animationHashClose = pushCloseWide;
                    break;
                }
                case DoorType.PushDoorL:{
                    break;
                }
                case DoorType.PullDoorL:{
                    break;
                }
            }

        }

        private async void OnTriggerEnter(Collider other) // trigger door open event
        {

            if (other.gameObject.CompareTag("Player"))
            {

                GetAnimationHash(CurrentDoorType);
                _door.CanBeInteractedWith = false;
                _animator.CrossFadeInFixedTime(_animationHashOpen, 0, 0);
            }
            await Task.Delay(1500);


            if (other.gameObject.CompareTag("Player"))
            {
                _door.CanBeInteractedWith = true;
                _animator.CrossFadeInFixedTime(_animationHashClose, 0, 0);
            }

        }
        private void OnTriggerExit(Collider other) // trigger door close event
        {

        }
    }

}
