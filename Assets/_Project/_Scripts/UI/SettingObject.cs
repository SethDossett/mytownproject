using UnityEngine;

namespace MyTownProject.UI
{
    public class SettingObject : MonoBehaviour
    {
        [field: SerializeField] public SelectedObjectType objectType { get; private set; }
    }
}