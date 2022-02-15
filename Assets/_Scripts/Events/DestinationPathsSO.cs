using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "ScriptableObject/Event/DestinationPathSO")]
public class DestinationPathsSO : ScriptableObject
{
    #region Events
    public UnityAction<Vector3[]> destinations;
    public void RaiseEvent(Vector3[] pos)
    {
        destinations?.Invoke(pos);
    }
    #endregion


    #region Path
    public int index;
    public Vector3[] path;
    #endregion

    #region Conditions //when path ends these are checked by NPC Manager
    public bool hasDoor;
    public bool continueMoving;
    public bool needToTeleport;
    public Vector3 teleportPosition;
    #endregion
}
