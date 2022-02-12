using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "ScriptableObject/Event/DestinationPathSO")]
public class DestinationPathsSO : ScriptableObject
{
    public UnityAction<Vector3[]> destinations;
    public int index;
    public Vector3[] path;
    public void RaiseEvent(Vector3[] pos)
    {
        destinations?.Invoke(pos);
    }
}
