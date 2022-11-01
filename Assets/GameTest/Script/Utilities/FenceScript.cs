using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FenceScript : MonoBehaviour
{
    [SerializeField]
    GameObject deathparticle;
    private void OnCollisionEnter(Collision other) {
        if(other.gameObject.tag == "Pawn"){
            PawnAttribute pawnAttribute = other.gameObject.GetComponent<PawnAttribute>();
            if(pawnAttribute.side == GameController.PlayerSide.ATTACKER && !pawnAttribute.GetComponent<AttackerBehavior>().isGetBall){
                Instantiate(deathparticle,other.transform.position,new Quaternion());
                Destroy(other.gameObject);
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
