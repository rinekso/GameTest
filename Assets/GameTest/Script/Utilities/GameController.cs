using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor;

public class GameController : MonoBehaviour
{
    public static GameController instance;

    public enum PlayerSide
    {
        ATTACKER, DEFENDER
    }

    [System.Serializable]
    public struct PlayerInfo
    {
        public EnergyGenerator energy;
        public TextMeshProUGUI textStatus;
        public int score;
        public GameObject pawnAtk;
        public GameObject pawnDef;
        public GameObject area;
        public GameObject gate;
        public PlayerSide playerSide;
        public bool getBall;
    }

    [Header("Variable")]
    [SerializeField]
    GameObject ballPrefabs;
    public List<PlayerInfo> playerInfos;

    [SerializeField]
    GameObject blackPanel;
    [SerializeField]
    TextMeshProUGUI titleText;

    [Header("Timer")]
    [SerializeField]
    TextMeshProUGUI timeText;
    [SerializeField]
    float currentTime; //In second
    [SerializeField]
    float durationState;

    [Header("Indicator")]
    public bool isStart = false;
    [SerializeField]
    int currentMatch = 1;
    [SerializeField]
    int maxMatch = 5;
    bool ending = false;

    private void Awake() {
        instance = this;
    }
    private void Start() {
        MatchStart();
    }
    public void MatchStart(){
        currentTime = durationState;

        blackPanel.SetActive(true);
        titleText.gameObject.SetActive(true);
        
        titleText.text = "Match "+currentMatch;
        
        StartCoroutine(StartingMatch());
    }
    IEnumerator StartingMatch(){
        SpawnBall();

        yield return new WaitForSeconds(1);

        // Count down
        for (int i = 3; i > 0; i--)
        {
            titleText.text = i.ToString();
            yield return new WaitForSeconds(1);
        }

        titleText.text = "Go!!!";
        yield return new WaitForSeconds(1);
        blackPanel.SetActive(false);

        // Energy start
        foreach (var item in playerInfos)
        {
            item.energy.Initialize();
        }

        isStart = true;
    }
    public void SpawnPawn(GameObject _area, Vector3 _position){
        PlayerInfo result = playerInfos.Find( x => x.area == _area);
        _position.y += result.pawnAtk.transform.localScale.y/2;

        CheckingPawn(result, (result.playerSide == PlayerSide.ATTACKER) ? result.pawnAtk : result.pawnDef, _position);
    }
    void CheckingPawn(PlayerInfo _playerInfo, GameObject _prefab, Vector3 _position){
        int cost = _prefab.GetComponent<PawnAttribute>().cost;
        if(_playerInfo.energy.currentBar >= cost){
            _playerInfo.energy.EnergyDecrease(cost);
            GameObject go = Instantiate(
                _prefab,
                _position,
                _prefab.transform.rotation);
            go.GetComponent<PawnAttribute>().startPosition = _position;
        }
    }
    void SpawnBall(){
        Collider collider = playerInfos.Find( x => x.playerSide == PlayerSide.ATTACKER).area.GetComponent<Collider>();
        float margin = .05f;
        Vector3 randomPos = new Vector3(
            UnityEngine.Random.Range(collider.bounds.min.x + margin, collider.bounds.max.x - margin), 
            collider.bounds.max.y + ballPrefabs.transform.localScale.y/2,
            UnityEngine.Random.Range(collider.bounds.min.z + margin, collider.bounds.max.z - margin)
        );
        Instantiate(ballPrefabs,randomPos,new Quaternion());
    }
    public PlayerInfo Opponent(int _playerID){
        for (int i = 0; i < playerInfos.Count; i++)
            if(i != _playerID)
                return playerInfos[i];
        return playerInfos[0];
    }
    public void GetBall(int _playerID){
        for (int i = 0; i < playerInfos.Count; i++)
        {
            PlayerInfo playerInfoTemp = playerInfos[i];
            playerInfoTemp.getBall = (i == _playerID) ? true : false;

            playerInfos[i] = playerInfoTemp;
        }
    }
    public Vector3 TargetGate(int _playerID){
        return Opponent(_playerID).gate.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(isStart){
            // Time decrease
            currentTime -= Time.deltaTime;
            if(currentTime <= 0)
                MatchEnd(false);
            else
                UpdateTimeText();
        }
    }
    public void MatchEnd(bool isGol = false){
        if(!ending){
            ending = true;
            isStart = false;
            print("Match End");
            StartCoroutine(ClosingMatch(isGol));
        }
    }
    IEnumerator ClosingMatch(bool isGol){
        blackPanel.SetActive(true);

        int playerWin = 0;
        for (int i = 0; i < playerInfos.Count; i++)
        {
            PlayerInfo playerInfoTemp = playerInfos[i];
            if(isGol){
                if(playerInfoTemp.getBall) playerInfoTemp.score++; playerWin = i;
            }else{
                if(!playerInfoTemp.getBall) playerInfoTemp.score++; playerWin = i;
            }
            playerInfos[i] = playerInfoTemp;
        }
        titleText.text = "Player " + (playerWin+1) + "Win";
        yield return new WaitForSeconds(2);
        PreparingMatchStart();
    }
    void PreparingMatchStart(){
        // remove all pawn and ball
        ClearObject("Pawn");
        ClearObject("Ball");

        // Reset player and switch side
        PlayerResetAndSwitchSide();

        if(currentMatch < maxMatch)
            currentMatch++;
        MatchStart();
    }
    void PlayerResetAndSwitchSide(){
        for (int i = 0; i < playerInfos.Count; i++)
        {
            PlayerInfo playerInfoTemp = playerInfos[i];
            playerInfoTemp.playerSide = (playerInfoTemp.playerSide == PlayerSide.ATTACKER) ? PlayerSide.DEFENDER : PlayerSide.ATTACKER;

            string status = (playerInfoTemp.playerSide == PlayerSide.ATTACKER) ? "(Attacker)" : "(Defender)";
            playerInfoTemp.textStatus.text = "Player "+ (i+1) + " " + status;

            playerInfoTemp.getBall = false;
            playerInfos[i] = playerInfoTemp;
        }
        ending = false;
    }
    void ClearObject(string _tag){
        GameObject[] objs = GameObject.FindGameObjectsWithTag(_tag);
        for (int i = 0; i < objs.Length; i++)
        {
            Destroy(objs[i]);
        }
    }
    void UpdateTimeText(){
        timeText.text = Mathf.Round(currentTime) +"s";
    }
}