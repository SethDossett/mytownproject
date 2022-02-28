using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

namespace DPUtils.Systems.DateTime
{
    public class CalendarController : MonoBehaviour
    {
        public List<CalendarPanel> calendarPanels;
        public List<KeyDates> keyDates;
        public TextMeshProUGUI seasonText;
        public TextMeshProUGUI setDescriptionText;
        public static TextMeshProUGUI DescriptionText;

        private int currentSeasonView = 0;
        private DateTime previousDateTime;

        private void Awake()
        {
            TimeManager.OnDateTimeChanged += DateTimeChanged;
        }

        private void OnDisable()
        {
            TimeManager.OnDateTimeChanged -= DateTimeChanged;
        }

        private void Start()
        {
            DescriptionText = setDescriptionText;
            DescriptionText.text = "";
            previousDateTime = TimeManager.DateTime;
            SortDates();
            FillPanels((Season)0);
        }

        void DateTimeChanged(DateTime _date)
        {
            if (currentSeasonView == (int)_date.Season)
            {
                if (previousDateTime.Date != _date.Date)
                {
                    var index = (previousDateTime.Date - 1) < 0 ? 0 : (previousDateTime.Date - 1);
                    calendarPanels[index].HideHighlight();
                    calendarPanels[_date.Date - 1].ShowHighlight();
                }
                calendarPanels[_date.Date - 1].ShowHighlight();
                previousDateTime = _date;
            }
        }

        private void SortDates()
        {
            keyDates = keyDates
                .OrderBy(d => d.KeyDate.Season)
                .ThenBy(d => d.KeyDate.Date)
                .ToList();
        }

        private void FillPanels(Season _season)
        {
            seasonText.text = _season.ToString();

            for (int i = 0; i < calendarPanels.Count; i++)
            {
                calendarPanels[i].SetUpDate((i + 1).ToString());

                if (currentSeasonView == (int)TimeManager.DateTime.Season && (i + 1) == TimeManager.DateTime.Date)
                {
                    calendarPanels[i].ShowHighlight();
                }
                else
                {
                    calendarPanels[i].HideHighlight();
                }

                foreach (var date in keyDates)
                {
                    if ((i + 1) == date.KeyDate.Date && date.KeyDate.Season == _season)
                    {
                        calendarPanels[i].AssignKeyDate(date);
                    }
                }
            }
        }

        public void OnNextSeason()
        {
            currentSeasonView += 1;
            if (currentSeasonView > 3) currentSeasonView = 0;
            FillPanels((Season)currentSeasonView);
        }

        public void OnPreviousSeason()
        {
            currentSeasonView -= 1;
            if (currentSeasonView < 0) currentSeasonView = 3;
            FillPanels((Season)currentSeasonView);
        }

    }

}
