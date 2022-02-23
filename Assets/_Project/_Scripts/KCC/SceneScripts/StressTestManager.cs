using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KinematicCharacterController.Examples
{
    public class StressTestManager : MonoBehaviour
    {
        public Camera Camera;
        public LayerMask UIMask;

        public InputField CountField;
        public Image RenderOn;
        public Image SimOn;
        public Image InterpOn;
        public TheCharacterController CharacterPrefab;
        public ExampleAIController AIController;
        public int SpawnCount = 100;
        public float SpawnDistance = 2f;

        private void Start()
        {
            KinematicCharacterSystem.EnsureCreation();
            CountField.text = SpawnCount.ToString();
            UpdateOnImages();

            KinematicCharacterSystem.Settings.AutoSimulation = false;
            KinematicCharacterSystem.Settings.Interpolate = false;
        }

        private void Update()
        {

            KinematicCharacterSystem.Simulate(Time.deltaTime, KinematicCharacterSystem.CharacterMotors, KinematicCharacterSystem.PhysicsMovers);
        }

        private void UpdateOnImages()
        {
            RenderOn.enabled = Camera.cullingMask == -1;
            SimOn.enabled = Physics.autoSimulation;
            InterpOn.enabled = KinematicCharacterSystem.Settings.Interpolate;
        }

        public void SetSpawnCount(string count)
        {
            if (int.TryParse(count, out int result))
            {
                SpawnCount = result;
            }
        }

        public void ToggleRendering()
        {
            if(Camera.cullingMask == -1)
            {
                Camera.cullingMask = UIMask;
            }
            else
            {
                Camera.cullingMask = -1;
            }
            UpdateOnImages();
        }

        public void TogglePhysicsSim()
        {
            Physics.autoSimulation = !Physics.autoSimulation;
            UpdateOnImages();
        }

        public void ToggleInterpolation()
        {
            KinematicCharacterSystem.Settings.Interpolate = !KinematicCharacterSystem.Settings.Interpolate;
            UpdateOnImages();
        }

        
    }
}