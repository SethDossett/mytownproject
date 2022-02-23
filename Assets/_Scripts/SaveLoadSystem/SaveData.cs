using UnityEngine;


namespace SaveLoadSystem
{
    [System.Serializable]
    public class SaveData 
    {
        public int index = 1;
        [SerializeField] private float myfloat = 5.1f;
        public bool ourBool = true;
        public Vector3 ourVector = new Vector3(0f, 2.2f, 99f);





    }

}