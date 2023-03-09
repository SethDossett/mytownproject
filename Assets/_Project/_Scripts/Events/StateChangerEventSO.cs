using UnityEngine;
using UnityEngine.Events;
using MyTownProject.Core;
using MyTownProject.NPC;

namespace MyTownProject.Events
{
    [CreateAssetMenu(menuName = "ScriptableObject/Event/State")]
    public class StateChangerEventSO : ScriptableObject
    {
        public UnityAction<GameState> OnGameState;
        public UnityAction OnGameStateVoid;

        public void RaiseEventGame(GameState gameState) => OnGameState?.Invoke(gameState);
        public void RaiseGameStateVoid() => OnGameStateVoid?.Invoke();

       
    }
}
