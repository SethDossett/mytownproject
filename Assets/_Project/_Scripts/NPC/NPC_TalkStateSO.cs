using UnityEngine;
using MyTownProject.Core;
using UnityEngine.InputSystem;

namespace MyTownProject.NPC
{
    [CreateAssetMenu(menuName = "States/NPC/Talk")]
    public class NPC_TalkStateSO : State<NPC_StateCtrl>
    {
        [SerializeField]
        private float _speed = 5f;

        public override void Init(NPC_StateCtrl parent)
        {
            base.Init(parent);
        }

        public override void CaptureInput()
        {
           
        }

        public override void Update()
        {
           
        }

        public override void FixedUpdate() {}

        public override void ChangeState()
        {
            if(Keyboard.current.pKey.wasPressedThisFrame)
                _runner.SetState(typeof(NPC_WalkStateSO));
            
        }

        public override void Exit() {}
    }
}