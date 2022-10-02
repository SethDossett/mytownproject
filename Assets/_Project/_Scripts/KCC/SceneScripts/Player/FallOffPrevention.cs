using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KinematicCharacterController.Examples
{
    public class FallOffPrevention : MonoBehaviour
    {
        [SerializeField]Transform _player;
        private void Update() {
            transform.position = _player.TransformPoint(new Vector3(0, 1f, 0.7f));
        }

        bool RayDown(){
            
        }
    }

}
