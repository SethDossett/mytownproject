using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MyTownProject.Core
{
    public abstract class StateRunner<T> : MonoBehaviour where T : MonoBehaviour
    {
        private State<T> _activeState;
        [SerializeField]
        private List<State<T>> _states;

        protected virtual void Awake()
        {
            SetState(_states[0].GetType());
        }

        public void SetState(Type newStateType)
        {
            if (_activeState != null)
            {
                _activeState.Exit();
            }

            _activeState = _states.First(s => s.GetType() == newStateType);
            _activeState.Init(GetComponent<T>());
        }

        private void Update()
        {
            _activeState.CaptureInput();
            _activeState.Update();
            _activeState.ChangeState();
        }

        private void FixedUpdate()
        {
            _activeState.FixedUpdate();
        }
    }
}