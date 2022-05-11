using UnityEngine.Events;
using UnityEngine;

namespace MyTownProject.SO
{
    [CreateAssetMenu(menuName = "ScriptableObject/Action")]
    public class ActionSO : ScriptableObject
    {
        public UnityAction<Vector3, Quaternion> OnTeleport;

        public void TeleportObject(Vector3 loc, Quaternion rot)
        {
            OnTeleport?.Invoke(loc, rot);
        }
    }
}
