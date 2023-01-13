using UnityEngine;
using MyTownProject.Core;
using UnityEngine.InputSystem;

namespace MyTownProject.NPC
{
    [CreateAssetMenu(menuName = "States/NPC/Walk")]
    public class NPC_WalkStateSO : State<NPC_StateCtrl>
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
           if(Keyboard.current.oKey.wasPressedThisFrame)
                _runner.SetState(typeof(NPC_TalkStateSO));
            
        }

        public override void Exit() {}
    }
}