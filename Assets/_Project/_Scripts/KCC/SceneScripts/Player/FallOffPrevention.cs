using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KinematicCharacterController.Examples
{
    public class FallOffPrevention : MonoBehaviour
    {
        [SerializeField] private bool _objectsVisible;
        [SerializeField] private float _minDistance = 0.6f;
        [SerializeField] private float _distance = 2.5f;
        [SerializeField] private int _count = 8;
        [SerializeField] private float _maxHeight = 0.66f;// 0.4f ledge he can crawl off, 0.5f he cant, dont know where limit is in between.
        [SerializeField] private int _testsCount = 5;
        [SerializeField] private int _layer;
        [SerializeField] private Vector3 _offset = new Vector3(0, 1f, 0);

        [SerializeField] Transform[] _borders;

        private void Awake()
        {
            //_borders[i] = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
            //_borders[i].localScale = new Vector3(1f, 0.5f, 0.1f);
            //_borders[i].gameObject.SetActive(isActiveAndEnabled);
            //_borders[i].gameObject.layer = _layer;
            SetRenderer(_objectsVisible);
        }
        private void SetRenderer(bool visible){
            for (int i = 0; i < _borders.Length; i++)
            {
                if(visible)
                    _borders[i].GetComponent<Renderer>().enabled = true;
                else
                    _borders[i].GetComponent<Renderer>().enabled = false;
            }
            
        }

        private void OnEnable()
        {
            for (int i = 0; i < _count; i++)
            {
                _borders[i].gameObject.SetActive(true);
            }
        }

        private void OnDisable()
        {
            for (int i = 0; i < _count; i++)
            {
                _borders[i].gameObject.SetActive(false);
            }
        }

        private void LateUpdate()
        {
            var fromPosition = transform.position + _offset;

            for (int i = 0; i < _count; i++)
            {
                var angle = 360 / _count * i;
                var direction = Quaternion.Euler(0, angle, 0) * transform.forward;

                var previousTestDistance = _minDistance;

                for (int t = 0; t < _testsCount; t++)
                {
                    var testDistance = (_distance - _minDistance) / _testsCount * t + _minDistance;
                    var testPosition = fromPosition + direction * testDistance;

                    Debug.DrawRay(testPosition, Vector3.down * _maxHeight);

                    if (Physics.Linecast(testPosition, testPosition + Vector3.down * _maxHeight))
                    {
                        previousTestDistance = testDistance;
                    }
                    else
                    {
                        break;
                    }
                }

                _borders[i].transform.position = fromPosition + direction * previousTestDistance;
                _borders[i].transform.forward = -direction;
            }
        }

    }
}
