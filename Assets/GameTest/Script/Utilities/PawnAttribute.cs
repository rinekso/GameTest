using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PawnAttribute : MonoBehaviour
{
    [Header("Attribute Pawn")]
    public GameController.PlayerSide side;
    public int cost;
    [SerializeField]
    float reactiveTime; //in second
    [SerializeField]
    int playerId;
    
    [Space]
    [SerializeField]
    Material normalMat;
    [SerializeField]
    Material grayscaleMat;
    [SerializeField]
    GameObject meshBody;

    [Header("Indicator")]
    [SerializeField]
    Vector3 destination;
    [SerializeField]
    Vector3 velocity;
    [SerializeField]
    float distance;
    [SerializeField]
    bool isStop;
    

    [Space]
    protected NavMeshAgent agent;
    protected Animator animator;
    protected Rigidbody rigidbody;
    public bool isPawnActive = true;
    public Vector3 startPosition;
    private void Awake() {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        rigidbody = GetComponent<Rigidbody>();
    }
    private void Start() {
    }
    public int GetPlayerID(){
        return playerId;
    }
    public void Deactive(){
        // print("deactive");
        animator.SetFloat("Blend",0);

        meshBody.GetComponent<SkinnedMeshRenderer>().materials = new Material[1] {grayscaleMat};
        isPawnActive = false;
        agent.Stop();
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        rigidbody.velocity = Vector3.zero;

        StartCoroutine(Cooldown());
    }
    IEnumerator Cooldown(){
        yield return new WaitForSeconds(reactiveTime);
        Reactive();
    }
    protected virtual void Reactive(){
        agent.isStopped = false;
        isPawnActive = true;
        meshBody.GetComponent<SkinnedMeshRenderer>().materials = new Material[1] {normalMat};
    }
    protected void GoTo(Vector3 _destination){
        animator.SetFloat("Blend",1);
        // print(gameObject.name + "-destination = "+_destination);
        print("goto destination = "+_destination.x+"/"+_destination.y+"/"+_destination.z);
        agent.SetDestination(_destination);
    }
    // Update is called once per frame
    void Update()
    {
        destination = agent.destination;
        velocity = agent.velocity;
        distance = agent.remainingDistance;
        isStop = agent.isStopped;
        
        if(!agent.pathPending){
            if(agent.remainingDistance <= agent.stoppingDistance){
                if(!agent.hasPath || agent.velocity.sqrMagnitude == 0){
                    if(!GameController.instance.pinaltyMode)
                        agent.isStopped = true;
                    // print("set animation zero");
                    animator.SetFloat("Blend",0);
                }
            }
        }

    }
}
