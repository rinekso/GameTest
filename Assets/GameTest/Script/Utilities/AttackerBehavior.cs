using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AttackerBehavior : PawnAttribute
{
    [Header("Attribute Attacker")]
    [SerializeField]
    float normalSpeed = 1.5f;
    [SerializeField]
    float carryingSpeed = .75f;
    public Transform ballPosition;
    public bool isGetBall;
    GameObject ball;
    // Start is called before the first frame update
    void Start()
    {
        ball = GameObject.FindGameObjectWithTag("Ball");
        SetAttribute();

        CheckBall();
    }
    void CheckBall(){
        if(!GameController.instance.playerInfos[GetPlayerID()].getBall){
            GoTo(ball.transform.position);
        }else{
            Vector3 pos = startPosition;

            if(GetPlayerID() == 0)
                pos.x = GameController.instance.Opponent(GetPlayerID()).area.GetComponent<Collider>().bounds.min.x;
            else
                pos.x = GameController.instance.Opponent(GetPlayerID()).area.GetComponent<Collider>().bounds.max.x;

            GoTo(pos);
        }
    }
    public void GetBall(){
        isGetBall = true;
        agent.speed = carryingSpeed;
        GoTo(GameController.instance.TargetGate(GetPlayerID()));
    }
    protected override void Reactive(){
        base.Reactive();

        if(isGetBall){
            GetBall();
        }else{
            CheckBall();
        }
    }
    void SetAttribute(){
        agent.speed = normalSpeed;
    }
}
