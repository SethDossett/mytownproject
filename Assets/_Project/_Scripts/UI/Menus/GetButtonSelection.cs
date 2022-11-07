using UnityEngine;
using LeTai.TrueShadow;
using MyTownProject.SO;
using MyTownProject.Core;
using MyTownProject.Events;

namespace MyTownProject.UI
{
    public class GetButtonSelection : MonoBehaviour
    {
        [SerializeField] GameObject _button;
        public bool IgnoreGetter;
        public GameObject Button
        {
            get
            {
                return _button;
            }
            set
            {
                value = _button;
            }
        }

    }
}
