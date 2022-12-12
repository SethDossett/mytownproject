using UnityEngine;
using System.Collections.Generic;

namespace MyTownProject.SaveLoadSystem
{
    public abstract class State_Save : MonoBehaviour
    {
        [field:SerializeField] public string SaveLocation{get; private set;}
        
        public virtual string SaveState()
        {
            return null;
        }

        public virtual void LoadState(string loadedJSON)
        {

        }

        public virtual bool ShouldSave()
        {
            return true;
        }

        public virtual string GetUID()
        {
            return (gameObject.scene.name + "_" + gameObject.name + "_" + (this.GetType()));

        }

        public virtual bool ShouldLoad()
        {
            return true;
        }

        public virtual void NoSaveFile()
        {

        }
    }
}
