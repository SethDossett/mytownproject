using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KinematicCharacterController.Examples
{
    public class ExampleAIController : MonoBehaviour
    {
        [SerializeField] float _speed = 1f;
        public TheCharacterController Character;
        public Vector3 _moveDir;
        private bool _stepHandling;
        private bool _ledgeHandling;
        private bool _intHandling;
        private bool _safeMove;

        private void Update()
        {
            AICharacterInputs inputs = new AICharacterInputs();

            // Simulate an input on all controlled characters
            //inputs.MoveVector = Mathf.Sin(Time.time * MovementPeriod) * Vector3.forward;
            inputs.MoveVector = _moveDir;
            inputs.LookVector = _moveDir;
            //inputs.LookVector = Vector3.Slerp(-Vector3.forward, Vector3.forward, inputs.MoveVector.z).normalized;
            


            Character.SetInputs(ref inputs);
        }
    }
}