using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallIndicator : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) {
        if(other.tag == "Pawn"){
            PawnAttribute pawnAttribute = other.GetComponent<PawnAttribute>();
            if(pawnAttribute.side == GameController.PlayerSide.ATTACKER){
                GameController.instance.GetBall(pawnAttribute.GetPlayerID());
                
                print(other.name);
                transform.parent = other.GetComponent<AttackerBehavior>().ballPosition;
                transform.localPosition = Vector3.zero;
                other.GetComponent<AttackerBehavior>().GetBall();
                this.enabled = false;
            }
        }
    }
}
