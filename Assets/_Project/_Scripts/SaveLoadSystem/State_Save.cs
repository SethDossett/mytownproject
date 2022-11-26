using UnityEngine;

namespace MyTownProject.SaveLoadSystem
{
    public class State_Save : MonoBehaviour
    {

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
