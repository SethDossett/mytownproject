using UnityEngine;
using UnityEngine.Events;

namespace MyTownProject.Cameras
{
    public class VisibleDetection : MonoBehaviour
    {
        [SerializeField] UnityEvent visible;
        [SerializeField] UnityEvent invisible;


        private void OnBecameVisible()
        {
            visible?.Invoke();
        }
        private void OnBecameInvisible()
        {
            invisible?.Invoke();
        }
    }
}
