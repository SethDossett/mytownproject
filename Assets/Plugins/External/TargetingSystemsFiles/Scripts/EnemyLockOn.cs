using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLockOn : MonoBehaviour
{
    [SerializeField] Transform currentTarget;
    [SerializeField] Transform _closestTarget = null;
    Animator anim;

    [SerializeField] LayerMask targetLayers;
    [SerializeField] Transform enemyTarget_Locator;

    [Tooltip("StateDrivenMethod for Switching Cameras")]
    [SerializeField] Animator cinemachineAnimator;

    [Header("Settings")]
    [SerializeField] bool zeroVert_Look;
    [SerializeField] float noticeZone = 10;
    [SerializeField] float lookAtSmoothing = 2;
    [Tooltip("Angle_Degree")] [SerializeField] float maxNoticeAngle = 60;
    [SerializeField] float crossHair_Scale = 0.1f;
    [SerializeField] Collider[] nearbyTargets;
    List<Transform> remainingTargets = new List<Transform>();
    
    
    Transform cam;
    bool enemyLocked;
    float currentYOffset;
    Vector3 pos;
    [SerializeField] int _NPCIndex;

    [SerializeField] CameraFollow camFollow;
    [SerializeField] Transform lockOnCanvas;
    DefMovement defMovement;
    float _timer = 0;
    bool _startTimer;
    [SerializeField] float _nextTargetWindow;

    void Start()
    {
        defMovement = GetComponent<DefMovement>();
        anim = GetComponent<Animator>();
        cam = Camera.main.transform;
        lockOnCanvas.gameObject.SetActive(false);
        _startTimer = false;
    }

    void Update()
    {
        camFollow.lockedTarget = enemyLocked;
        defMovement.lockMovement = enemyLocked;
       
        
        IconControl();
        CheckTimer();
           
        
        if (Input.GetKeyDown(KeyCode.Space)){
            print("Fire");
            if(_startTimer == true && remainingTargets.Count > 0){
                if(_timer <= _nextTargetWindow)
                    StartCoroutine(FindNextTarget());
                    return;
            }
            currentTarget = _closestTarget;
            if (currentTarget)
            {

                if(enemyLocked) StartCoroutine(FindNextTarget()); else FoundTarget();

            }
            

    

        }

        if(Input.GetKeyUp(KeyCode.Space)){
            if(currentTarget != null)
                _startTimer = true;
        }





        if(!enemyLocked){
            CheckForNPCS();
        }
        if (enemyLocked) {
            if(!TargetOnRange()) ResetTarget();
            LookAtTarget();
        }

        


    }

    void CheckTimer(){
        if(_startTimer) {
            _timer += Time.deltaTime;
        } 


        if(_timer > _nextTargetWindow){
            ResetTarget();
        } 
    }
    void FoundTarget(){
        print("Found");
        currentTarget.GetComponent<NPCManager>().Targeted();
        lockOnCanvas.gameObject.SetActive(true);
        anim.SetLayerWeight(1, 1);
        cinemachineAnimator.Play("TargetCamera");
        enemyLocked = true;
        _closestTarget.GetComponent<NPCManager>().SetTargeted();

        for(int i = 0; i < nearbyTargets.Length; i++){
            Transform t = nearbyTargets[i].transform;
            remainingTargets.Add(t);
            if(t.gameObject.GetComponent<NPCManager>().beenTargeted == true){
                remainingTargets.Remove(t);
            }
            if(GetDistance(t) > t.GetComponent<NPCManager>().maxDistance){
                remainingTargets.Remove(t);
            }
            
        }
    }
    //public int index;
    IEnumerator FindNextTarget(){
        print("FindNext");
        _startTimer = false;
        _timer = 0;
        
        if(remainingTargets.Count <= 0) ResetTarget();
        else{
            // MAKE IT FIND NEXT CLOSEST
            // AND NOT BE OUT OF MAXDISTANCE
            //Need To Check if New TArgets have come in or Out!!!!!!!!!!!!!!!!!!!!!!!!!!
            currentTarget = remainingTargets[0];
            remainingTargets.Remove(remainingTargets[0]);

        }
        
        yield return null;

    }

    void ResetTarget()
    {
        print("reset");
        _startTimer = false;
        _timer = 0;
        //nearbyTargets[i].GetComponent<NPCManager>().Hovered();
        lockOnCanvas.gameObject.SetActive(false);
        currentTarget = null;
        _closestTarget = null;
        enemyLocked = false;
        anim.SetLayerWeight(1, 0);
        cinemachineAnimator.Play("FollowCamera");
        _NPCIndex = 0;
        BeenTargetedReset();
    }

    void BeenTargetedReset(){
        foreach(var npc in nearbyTargets){
            NPCManager m = npc.GetComponent<NPCManager>();
            m.UnsetTargeted();
            m.UnTargeted();
            m.HideHover();
        }
        remainingTargets.Clear();
    }

    private void CheckForNPCS(){
        nearbyTargets = Physics.OverlapSphere(transform.position, noticeZone, targetLayers);
        float closestAngle = maxNoticeAngle;
        Transform closestTarget = null;
        if (nearbyTargets.Length <= 0) return;
        for (int i = 0; i < nearbyTargets.Length; i++)
        {
            
            Vector3 dir = nearbyTargets[i].transform.position - cam.position;
            dir.y = 0;
            float _angle = Vector3.Angle(cam.forward, dir);
            
            if (_angle < closestAngle)
            {
                closestTarget = nearbyTargets[i].transform;
                closestAngle = _angle; 
                _NPCIndex = i;
            }
            
        }

        if (!closestTarget ) return;
        float h1 = closestTarget.GetComponent<CapsuleCollider>().height;
        float h2 = closestTarget.localScale.y;
        float h = h1 * h2;
        float half_h = (h / 2) / 2;
        currentYOffset = h - half_h;
        if(zeroVert_Look && currentYOffset > 1.6f && currentYOffset < 1.6f * 3) currentYOffset = 1.6f;
        Vector3 tarPos = closestTarget.position + new Vector3(0, currentYOffset, 0);
        if(Blocked(tarPos)) return;
        if(GetDistance(closestTarget) > closestTarget.GetComponent<NPCManager>().maxDistance){
                print("OutOfRange " + closestTarget.gameObject.name);
                return;
            }
        _closestTarget = closestTarget;
        
    }

    bool Blocked(Vector3 t){
        RaycastHit hit;
        if(Physics.Linecast(transform.position + Vector3.up * 0.5f, t, out hit)){
            if(!hit.transform.CompareTag("Interactable")) return true;
        }
        return false;
    }
    [SerializeField] float dis;
    bool TargetOnRange(){
        dis = (transform.position - pos).magnitude;
        if(dis > noticeZone) return false; else return true;
    }

    float GetDistance(Transform t){
        float distance = (transform.position - t.position).magnitude;

        return distance;
    }


    private void LookAtTarget()
    {
        if(currentTarget == null) {
            ResetTarget();
            return;
        }
        //currentTarget.GetComponent<NPCManager>().Targeted();
        pos = currentTarget.position + new Vector3(0, currentYOffset, 0);
        
        enemyTarget_Locator.position = pos;
        Vector3 dir = currentTarget.position - transform.position;
        dir.y = 0;
        Quaternion rot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * lookAtSmoothing);
    }
    private void IconControl(){
        

        if(_closestTarget != null){
            foreach(var npc in nearbyTargets){
            if(npc.gameObject.transform == _closestTarget){
                npc.gameObject.GetComponent<NPCManager>().Hovered();
            }
            else npc.gameObject.GetComponent<NPCManager>().HideHover();
            }   
        }
        // NOT WORKING RIGHT ?
        if(currentTarget != null){
            foreach(var npc in nearbyTargets){
            if(npc.gameObject.transform == currentTarget){
                npc.gameObject.GetComponent<NPCManager>().Targeted();
            }
            else npc.gameObject.GetComponent<NPCManager>().HideHover();
            }   
        }


        
    }

    private void LockOnCanvas(){
        lockOnCanvas.position = pos;
        lockOnCanvas.localScale = Vector3.one * ((cam.position - pos).magnitude * crossHair_Scale);
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, noticeZone);   
    }
}
