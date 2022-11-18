using MyTownProject.Events;
using UnityEngine;

namespace MyTownProject.Enviroment
{
    public class OOBTrigger : MonoBehaviour
    {
        [SerializeField] GeneralEventSO FellOffLedge;
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                print("Player Fell");
                FellOffLedge.RaiseEvent();
            }
        }
    }
}