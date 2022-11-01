using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateIndicator : MonoBehaviour
{
    [SerializeField]
    int playerId;
    private void OnCollisionEnter(Collision other) {
        print(other.gameObject.name);
    }
    private void OnTriggerEnter(Collider other) {
        if(other.GetComponent<PawnAttribute>()){
            PawnAttribute pawn = other.GetComponent<PawnAttribute>();
            if(pawn.side == GameController.PlayerSide.ATTACKER && pawn.GetPlayerID() != playerId && other.GetComponent<AttackerBehavior>().isGetBall){
                SoundController.instance.PlaySFX(1);
                GameController.instance.MatchEnd(true);
            }
        }
    }
}
