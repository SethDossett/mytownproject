using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyTownProject.Interaction
{
    public abstract class Interactable : MonoBehaviour
    {
        public virtual void Awake()
        {
            gameObject.layer = 8;
        }
        public abstract void OnInteract();
        public abstract void OnFocus();
        public abstract void OnLoseFocus();


    }
}