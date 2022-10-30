using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DefenderBehavior : PawnAttribute
{
    [Header("Attribute Defender")]
    [SerializeField]
    float normalSpeed = 1f;
    [SerializeField]
    float returnSpeed = 2f;

    [Space]
    [SerializeField]
    bool isChasing = false;
    [SerializeField]
    bool isReturn = false;
    [SerializeField]
    GameObject targetChasing;
    private void OnTriggerStay(Collider other) {
        if(other.GetComponent<PawnAttribute>() && isPawnActive){
            PawnAttribute pawn = other.GetComponent<PawnAttribute>();
            if(pawn.side == GameController.PlayerSide.ATTACKER && !isChasing && pawn.isPawnActive){
                agent.isStopped = false;
                // print("chasing");
                isChasing = true;
                GetComponent<SphereCollider>().enabled = false;
                Vector3 pos = other.bounds.center;
                pos.y = other.bounds.min.y;
                GoTo(pos);
                targetChasing = other.gameObject;
            }
        }
    }
    private void OnCollisionEnter(Collision other) {
        if(other.gameObject.GetComponent<PawnAttribute>() && isPawnActive){
            PawnAttribute pawn = other.gameObject.GetComponent<PawnAttribute>();
            // print("got");
            isChasing = false;
            if(pawn.side == GameController.PlayerSide.ATTACKER && pawn.isPawnActive){
                isPawnActive = false;
                isReturn = true;
                Deactive();
                targetChasing.GetComponent<PawnAttribute>().Deactive();
            }else if(pawn.side == GameController.PlayerSide.ATTACKER && !pawn.isPawnActive){
                GetComponent<SphereCollider>().enabled = true;
                isPawnActive = true;

                agent.isStopped = true;
                // agent.velocity = Vector3.zero;
                // rigidbody.velocity = Vector3.zero;
            }
        }
    }
    protected override void Reactive()
    {
        isChasing = false;
        targetChasing = null;
        base.Reactive();
        isPawnActive = false;
        GetComponent<SphereCollider>().enabled = true;
        GoTo(startPosition);
    }
    void Stop()
    {
        if(isReturn){
            animator.SetFloat("Blend",0);
            isChasing = false;
            isReturn = false;
            isPawnActive = true;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if(!agent.pathPending){
            if(agent.remainingDistance <= agent.stoppingDistance){
                if(!agent.hasPath || agent.velocity.sqrMagnitude == 0){
                    Stop();
                }
            }
        }
    }
}
