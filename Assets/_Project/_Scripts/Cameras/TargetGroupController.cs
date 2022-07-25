using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using MyTownProject.Events;
using KinematicCharacterController.Examples;
using MyTownProject.NPC;


public class TargetGroupController : MonoBehaviour
{
    [SerializeField] DialogueEventsSO DialogueEvents;
    [SerializeField] TransformEventSO playerRef;
    [SerializeField] TransformEventSO _targetingEvent;
    [SerializeField] GeneralEventSO _unTargetingEvent;
    CinemachineTargetGroup targetGroup;
    Transform _player;
    void OnEnable(){
        targetGroup = GetComponent<CinemachineTargetGroup>();
        playerRef.OnRaiseEvent += SetPlayerReference;
        DialogueEvents.onEnter += TalkingToNPC;
        DialogueEvents.onExit += BackToPlayerView;
        _targetingEvent.OnRaiseEvent += Targeting;
        _unTargetingEvent.OnRaiseEvent += BackToPlayerView;
    }
    void OnDisable(){
        playerRef.OnRaiseEvent -= SetPlayerReference;
        DialogueEvents.onEnter -= TalkingToNPC;
        DialogueEvents.onExit -= BackToPlayerView;
        _targetingEvent.OnRaiseEvent -= Targeting;
        _unTargetingEvent.OnRaiseEvent -= BackToPlayerView;
    }
    void Start(){
        AddPlayer();
    }

    void SetPlayerReference(Transform player) => _player = player;

    void AddPlayer(){
        Transform lookAtPoint = _player.gameObject.GetComponent<PlayerManager>()._LookAtPoint; 
        AddingMember(lookAtPoint, 1, 1);
    } 
    
    void TalkingToNPC(GameObject go, TextAsset text){
        Transform lookAtPoint = go.GetComponent<NPC_Manager>()._head.transform;
        AddingMember(lookAtPoint, 1, 1);
            
    }
    void Targeting(Transform t){
        Transform toTarget = t.gameObject.GetComponent<NPC_Manager>()._head.transform;
        AddingMember(toTarget, 1, 1);
    }

    void BackToPlayerView(){
           RemoveTargets();
           AddPlayer();
    }

    void RemoveTargets(){
        foreach(var target in targetGroup.m_Targets){
            targetGroup.RemoveMember(target.target);
        }
    }

    void AddingMember(Transform t, float weight, float radius){
        targetGroup.AddMember(t, weight, radius);
    }



        
    

}
