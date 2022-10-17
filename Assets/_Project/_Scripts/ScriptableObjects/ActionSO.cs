using UnityEngine.Events;
using UnityEngine;

namespace MyTownProject.SO
{
    [CreateAssetMenu(menuName = "ScriptableObject/Action")]
    public class ActionSO : ScriptableObject
    {
        public UnityAction<Vector3, Quaternion> OnTeleport;
        public UnityAction<Vector3, float, Quaternion> OnSetTransientLocRot;

        public void TeleportObject(Vector3 loc, Quaternion rot)
        {
            OnTeleport?.Invoke(loc, rot);
        }

        public void SetTransientLocRot(Vector3 loc, float lerpSpeed, Quaternion rot)
        {
            OnSetTransientLocRot?.Invoke(loc, lerpSpeed, rot);
        }
    }
}
