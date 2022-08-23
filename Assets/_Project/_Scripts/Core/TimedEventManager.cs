using MyTownProject.Events;
using MyTownProject.NPC;
using UnityEngine;

namespace MyTownProject.Core
{
    public class TimedEventManager : MonoBehaviour
    {
        private TimedEventMaster _master;
        [SerializeField] NPC_ScriptableObject[] NPC;
        int jerry = 0;
        int boyty = 2;
        void OnEnable()
        {
            _master = GameObject.Find("EventMaster").GetComponent<TimedEventMaster>();
            _master.sixEvent += SixEvent;
            _master.sixThirtyEvent += SixThirtyEvent;
            _master.sevenEvent += SevenEvent;
            _master.sevenThirtyEvent += SevenThirtyEvent;
            _master.eightEvent += EightEvent;
        }
        void OnDisable()
        {
            _master.sixEvent -= SixEvent;
            _master.sixThirtyEvent -= SixThirtyEvent;
            _master.sevenEvent -= SevenEvent;
            _master.sevenThirtyEvent -= SevenThirtyEvent;
            _master.eightEvent -= EightEvent;
        }
        private void SixEvent() 
        {
            //NPC[boyty].currentDestinationIndex = 0;
            //NPC[boyty].RaiseEventMove(); // can use NPC.canMove, but leaving it just in case I want event fired.
        }
        private void SixThirtyEvent()
        {
            


            //NPC[jerry].currentDestinationIndex = 3;
            //NPC[0].atDestination = false;
            //NPC[0].moveTowardsDestination = true;

        }
        private void SevenEvent()
        {
            //NPC[1].currentDestinationIndex = 0;
            //NPC[1].atDestination = false;
            //NPC[1].moveTowardsDestination = true;
            
        }
        private void SevenThirtyEvent()
        {


        }
        private void EightEvent()
        {


        }
    }
}