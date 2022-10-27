using UnityEngine;
using MyTownProject.Interaction;
using System.Threading.Tasks;

namespace MyTownProject.Enviroment
{
    public enum DoorType
    {
        PushOpenR, PullOpenR, PullOpenL, PushOpenL 
    }
    public class DoorAnimator : MonoBehaviour
    {
        [field: SerializeField] public DoorType CurrentDoorType { get; private set; }
        Door _door;
        Animator _animator;
        string _npcTag = "NPC";


        #region Animation Hashes
        int pullOpenWide = Animator.StringToHash("PullOpenWide");
        int pushOpenWide = Animator.StringToHash("PushOpenWide");
        int pullCloseWide = Animator.StringToHash("PullCloseWide");
        int pushCloseWide = Animator.StringToHash("PushCloseWide");
        int pullOpenCrack = Animator.StringToHash("PullOpenCrack");
        int pushCloseCrack = Animator.StringToHash("PushCloseCrack");
        int pushOpenCrack = Animator.StringToHash("PushOpenCrack");
        int pullCloseCrack = Animator.StringToHash("PullCloseCrack");
        int _animationHashOpenWide = 0;
        int _animationHashCloseWide = 0;
        int _animationHashOpenCrack = 0;
        int _animationHashCloseCrack = 0;
        #endregion
        void Start()
        {
            _door = GetComponent<Door>();
            _animator = GetComponent<Animator>();
        }

        void GetAnimationHash(DoorType doorType)
        {

            switch (doorType)
            {
                case DoorType.PushOpenR:
                    {
                        _animationHashOpenWide = pushOpenWide;
                        _animationHashCloseWide = pullCloseWide;
                        _animationHashOpenCrack = pushOpenCrack;
                        _animationHashCloseCrack = pullCloseCrack;
                        break;
                    }
                case DoorType.PullOpenR:
                    {
                        _animationHashOpenWide = pullOpenWide;
                        _animationHashCloseWide = pushCloseWide;
                        _animationHashOpenCrack = pullOpenCrack;
                        _animationHashCloseCrack = pushCloseCrack;
                        break;
                    }
                case DoorType.PushOpenL:
                    {
                        break;
                    }
                case DoorType.PullOpenL:
                    {
                        break;
                    }
            }

        }

        public void PlayDoorAnimation()
        {
            GetAnimationHash(CurrentDoorType);
            _animator.CrossFadeInFixedTime(_animationHashOpenCrack, 0, 0);
            print($"Play animation on {this.name} ");
        }

        private async void OnTriggerEnter(Collider other)
        {//When NPC Walks into Trigger, door opens animation

            if (other.gameObject.CompareTag(_npcTag))
            {

                GetAnimationHash(CurrentDoorType);
                _door.CanBeInteractedWith = false;
                _animator.CrossFadeInFixedTime(_animationHashOpenWide, 0, 0);
            }
            await Task.Delay(1500);


            if (other.gameObject.CompareTag(_npcTag))
            {
                _door.CanBeInteractedWith = true;
                _animator.CrossFadeInFixedTime(_animationHashCloseWide, 0, 0);
            }

        }
        private void OnTriggerExit(Collider other) // trigger door close event
        {

        }
    }

}
