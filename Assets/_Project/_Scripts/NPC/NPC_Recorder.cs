using MyTownProject.SO;
using MyTownProject.Core;
using UnityEngine;

namespace MyTownProject.NPC
{
    public class NPC_Recorder : MonoBehaviour
    {
        public PathSO CurrentPath;
        [SerializeField] bool _isRecord;

        private void OnEnable()
        {
            TimeManager.OnDateTimeChanged += TimeUpdated;
        }
        private void OnDisable()
        {
            TimeManager.OnDateTimeChanged -= TimeUpdated;
        }
        void Awake()
        {
            if (_isRecord)
            {
                CurrentPath.Records.Clear();
            }
        }
        private void Start()
        {
            if (_isRecord)
                UpdateRecords();
        }

        void TimeUpdated(DateTime dateTime)
        {
            if (_isRecord)
                UpdateRecords();
        }
        void UpdateRecords()
        {
            RecordedValues values = new RecordedValues();
            values.TimeStep = Mathf.Floor(TimeManager.GlobalTime);
            values.Positions = this.transform.position;
            values.Rotations = this.transform.eulerAngles;

            CurrentPath.Records.Add(values);
        }


    }
} 
