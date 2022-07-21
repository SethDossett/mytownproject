using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NPCManager : MonoBehaviour
{
    public float maxDistance = 5f;
    public float currentDistance;
    [SerializeField] GameObject _hoverIcon;
    [SerializeField] GameObject _targetIcon;
    [SerializeField] GameObject _interactionUI;
    [SerializeField] TextMeshProUGUI _NPCName;
    [SerializeField] GameObject _titlName;
    [SerializeField] String _name;


    [SerializeField] bool ShowName = false;
    int targetedAnim = Animator.StringToHash("Targeted");
    int hoverAnim = Animator.StringToHash("Hover");

    public bool beenTargeted;

    int hshow = 0;
    int tshow = 1;
    void Awake(){
        if(_interactionUI.activeInHierarchy)
            _interactionUI.SetActive(false);
    }

    public void Hovered(){

        if(!_interactionUI.activeInHierarchy){
            _interactionUI.SetActive(true);

        }
        else{
            ShowIcon(hshow);

            if(ShowName){
                if(!_titlName.activeInHierarchy)
                    _titlName.SetActive(true);

                   
            }
            else{
                if(_titlName.activeInHierarchy)
                _titlName.SetActive(false);  
            } 
        }

        _NPCName.text = _name; 
    }
    public void HideHover(){
        if(_interactionUI.activeInHierarchy){
            _interactionUI.SetActive(false);
        }
    }
    public void Targeted(){
        if(!_interactionUI.activeInHierarchy){
            _interactionUI.SetActive(true);
        }

        if(_titlName.activeInHierarchy)
            _titlName.SetActive(false);    
        ShowIcon(tshow);
    }
    public void UnTargeted(){
        if(!_interactionUI.activeInHierarchy)
            _interactionUI.SetActive(true);

        ShowIcon(hshow);
    }


    public void SetTargeted(){
        if(!beenTargeted)
            beenTargeted = true;
        
    }

    public void UnsetTargeted(){
        if(beenTargeted)
            beenTargeted = false;
    }



    private void ShowIcon(int i){
        if(i == hshow){
            if(_targetIcon.activeInHierarchy)
                _targetIcon.SetActive(false);
            if(!_hoverIcon.activeInHierarchy)
                _hoverIcon.SetActive(true);
        }
        else{
            if(_hoverIcon.activeInHierarchy)
                _hoverIcon.SetActive(false);
            if(!_targetIcon.activeInHierarchy)
                _targetIcon.SetActive(true);
        }
    }
}
