using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using MyTownProject.NPC;

public class PlayerLookAtAim : MonoBehaviour
{
    [Header("References")]
    Transform _transform;


    [Header("OverLap Sphere")]
    [SerializeField] Transform _fieldOfView;
    [SerializeField] float _fofVRadius;
    [SerializeField] LayerMask _interactableLayer;
    [SerializeField] private Collider[] _colliders;
    [SerializeField] private Collider _closestCol = null;
    [SerializeField] int _numFound;
    string _tagNPC = "NPC";


    [Header("Rig Values")]
    [SerializeField] RigBuilder _rigBuilder;
    MultiAimConstraint _multiAimConstraint;
    bool _targetSet = false;
    float _lookSpeed = 1f;

    [Header("Prompt")]
    bool _promptOn = false;

    private void Awake()
    {
        _multiAimConstraint = GetComponent<MultiAimConstraint>();
    }
    void Start()
    {
        _transform = transform;
    }

    void Update()
    {
        //_numFound = Physics.OverlapSphereNonAlloc(_fieldOfView.position, _fofVRadius, _colliders, _interactableLayer);
        _colliders = Physics.OverlapSphere(_fieldOfView.position, _fofVRadius, _interactableLayer);

        if (_colliders.Length < 1)
        {
            _targetSet = false;
            _closestCol = null;
            //StopCoroutine(ChangeWeight(1));
            StopLooking();


            return;
        }

        if (!_targetSet)
        {
            /*float[] distance = null;

            for(int i = 0; i < _numFound; i++)
            {
                distance[i] = Vector3.Distance(_colliders[i].transform.position, _transform.position);

                if(i != 0)
                {
                    if(distance[i] < distance[i - 1])
                    {
                        if (_colliders[i].CompareTag(_tagNPC))
                        {
                            LookAtPlayer(_colliders[i].GetComponent<NPC_Manager>()._head.transform);
                        }
                        else
                        {
                            LookAtPlayer(_colliders[i].transform);
                        }
                    }
                    else
                    {
                        if (_colliders[i-1].CompareTag(_tagNPC))
                        {
                            LookAtPlayer(_colliders[i-1].GetComponent<NPC_Manager>()._head.transform);
                        }
                        else
                        {
                            LookAtPlayer(_colliders[i-1].transform);
                        }
                    }
                }
            }*/
            float hypotenus;
            float smallestHypotenus = Mathf.Infinity;
            foreach (Collider collider in _colliders)
            {
                hypotenus = CalculateHypotenus(collider.transform.position);
                if(smallestHypotenus > hypotenus)
                {
                    _closestCol = collider;
                    smallestHypotenus = hypotenus;
                    
                    ShowPrompt(_closestCol.transform.gameObject.GetComponentInChildren<InteractPrompt>());
                }

                if(collider != _closestCol)
                {
                    
                    HidePrompt(_closestCol.transform.gameObject.GetComponentInChildren<InteractPrompt>());
                }
            }
            if(_closestCol == null) HidePrompt(_closestCol.transform.gameObject.GetComponentInChildren<InteractPrompt>());


            if (_closestCol.CompareTag(_tagNPC))
            {
                LookAtPlayer(_closestCol.GetComponent<NPC_Manager>()._head.transform);
            }
            else
            {
                LookAtPlayer(_closestCol.transform);
            }

            
        }
       

    }
    float CalculateHypotenus(Vector3 pos)
    {
        float screenCenterX = Camera.main.pixelWidth / 2;
        float screenCenterY = Camera.main.pixelHeight / 2;

        Vector3 screenPos = Camera.main.WorldToScreenPoint(pos);
        float xDelta = screenCenterX - screenPos.x;
        float yDelta = screenCenterY - screenPos.y;
        float hypotenuse = Mathf.Sqrt(Mathf.Pow(xDelta, 2) + Mathf.Pow(yDelta, 2));

        return hypotenuse;
    }
    void LookAtPlayer(Transform target)
    {
        StartCoroutine(ChangeWeight(1));

        var data = _multiAimConstraint.data.sourceObjects;
        if (data[0].transform == null)
        {
            data.SetTransform(0, target);
            _multiAimConstraint.data.sourceObjects = data;
            _rigBuilder.Build();
        }

    }
    void StopLooking()
    {
        StartCoroutine(ChangeWeight(0));

        var data = _multiAimConstraint.data.sourceObjects;
        if (data[0].transform != null)
        {
            data.SetTransform(0, null);
            _multiAimConstraint.data.sourceObjects = data;
            _rigBuilder.Build();
        }
    }
    void ShowPrompt(InteractPrompt prompt)
    {
        prompt.enabled = true;
        prompt._imagePrompt.enabled = true;
        _promptOn = true;
    }
    void HidePrompt(InteractPrompt prompt)
    {
        prompt._imagePrompt.enabled = false;
        prompt.enabled = false;
        _promptOn = false;
    }
    IEnumerator ChangeWeight(int targetValue)
    {
        _multiAimConstraint.weight = Mathf.MoveTowards(_multiAimConstraint.weight, targetValue, _lookSpeed * Time.unscaledDeltaTime);
        

        yield return null;
        //if (_multiAimConstraint.weight == _targetValue) _targetSet = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_fieldOfView.position, _fofVRadius);
    }
}
