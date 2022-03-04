using UnityEngine;
using UnityEngine.Events;
using MyTownProject.Core;

namespace MyTownProject.Events
{
    [CreateAssetMenu(menuName = "ScriptableObject/Event/State")]
    public class StateChangerEventSO : ScriptableObject
    {
        public UnityAction<GameStateManager.GameState> OnGameState;

        public void RaiseEvent(GameStateManager.GameState gameState) => OnGameState?.Invoke(gameState);
       
    }
}
