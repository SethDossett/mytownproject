using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyTownProject.SO;

namespace MyTownProject.Enviroment
{
    public class DoorsInScene : MonoBehaviour
    {
        [SerializeField] SceneSO scene;
        public void FindAllDoorsInScene()
        {
            scene.DoorsInScene.Clear();
            foreach (Transform child in transform)
            {
                scene.DoorsInScene.Add(child.gameObject);
            }
            print("1st");
        }
        private void OnDisable() {
            scene.DoorsInScene.Clear();
        }


    }

}
