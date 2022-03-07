using UnityEngine;
using UnityEngine.Events;
using MyTownProject.Core;
using MyTownProject.NPC;

namespace MyTownProject.Events
{
    [CreateAssetMenu(menuName = "ScriptableObject/Event/State")]
    public class StateChangerEventSO : ScriptableObject
    {
        public UnityAction<GameStateManager.GameState> OnGameState;
        public UnityAction OnGameStateVoid;
        public UnityAction<NPC_StateHandler.NPCSTATE> OnNPCState;
        public UnityAction OnNPCStateVoid;

        public void RaiseEventGame(GameStateManager.GameState gameState) => OnGameState?.Invoke(gameState);
        public void RaiseGameStateVoid() => OnGameStateVoid?.Invoke();

        public void RaiseEventNPC(NPC_StateHandler.NPCSTATE npcState) => OnNPCState?.Invoke(npcState);
        public void RaiseNPCStateVoid() => OnNPCStateVoid?.Invoke();
       
    }
}
