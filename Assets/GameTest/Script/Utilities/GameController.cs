using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController instance;
    public bool pinaltyMode = false;

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
    GameObject completePanel;
    [SerializeField]
    TextMeshProUGUI scoreText;
    [SerializeField]
    TextMeshProUGUI finalText;
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
    public void GoToScene(string _scene){
        Application.LoadLevel(_scene);
    }
    public void ReloadScene(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void MatchStart(){
        currentTime = durationState;

        blackPanel.SetActive(true);
        titleText.gameObject.SetActive(true);
        
        if(pinaltyMode)
            titleText.text = "Pinalty Mode";
        else
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

        SoundController.instance.PlaySFX(0);
        titleText.text = "Go!!!";
        yield return new WaitForSeconds(1);
        blackPanel.SetActive(false);

        // Energy start
        foreach (var item in playerInfos)
        {
            if(item.energy != null) item.energy.Initialize();
        }

        isStart = true;
    }
    public void SpawnPawn(GameObject _area, Vector3 _position){
        PlayerInfo result = playerInfos.Find( x => x.area == _area);
        _position.y += result.pawnAtk.transform.localScale.y/2;

        CheckingPawn(result, (result.playerSide == PlayerSide.ATTACKER) ? result.pawnAtk : result.pawnDef, _position);
    }
    void CheckingPawn(PlayerInfo _playerInfo, GameObject _prefab, Vector3 _position){
        if(pinaltyMode){
            GameObject go = Instantiate(
                _prefab,
                _position,
                _prefab.transform.rotation);
            go.GetComponent<PawnAttribute>().startPosition = _position;
        }else{
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
    }
    void SpawnBall(){
        Collider collider = playerInfos.Find( x => x.playerSide == PlayerSide.ATTACKER).area.GetComponent<Collider>();
        float margin = .05f;
        Vector3 randomPos;
        if(pinaltyMode){
            randomPos = GetRandomGameBoardLocation();
        }else{
            randomPos = new Vector3(
                UnityEngine.Random.Range(collider.bounds.min.x + margin, collider.bounds.max.x - margin), 
                collider.bounds.max.y + ballPrefabs.transform.localScale.y/2,
                UnityEngine.Random.Range(collider.bounds.min.z + margin, collider.bounds.max.z - margin)
            );
        }

        Instantiate(ballPrefabs,randomPos,new Quaternion());
    }
    private Vector3 GetRandomGameBoardLocation()
    {
        NavMeshTriangulation navMeshData = NavMesh.CalculateTriangulation();
 
        int maxIndices = navMeshData.indices.Length - 3;
 
        // pick the first indice of a random triangle in the nav mesh
        int firstVertexSelected = UnityEngine.Random.Range(0, maxIndices);
 
        // spawn on verticies
        Vector3 point = navMeshData.vertices[navMeshData.indices[firstVertexSelected]];
 
        return point;
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
        return Opponent(_playerID).gate.transform.Find("GatePoin").position;
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
    void GameEnd(){
        completePanel.SetActive(true);
        titleText.gameObject.SetActive(false);

        if(!pinaltyMode){
            scoreText.text = "Score <br>"+
                "Player 1 = "+playerInfos[0].score+"<br>"+
                "Player 2 = "+playerInfos[1].score;

            if(playerInfos[0].score > playerInfos[1].score)
                finalText.text = "Player 1 Win!!!";
            else if(playerInfos[0].score < playerInfos[1].score)
                finalText.text = "Player 2 Win!!!";
            else
                finalText.text = "Draw";

        }else{
            scoreText.text = "You made it !!!";
        }
        
    }
    public void MatchEnd(bool _isGol = false){
        if(!ending){
            ending = true;
            isStart = false;
            print("Match End");
            StartCoroutine(ClosingMatch(_isGol));
        }
    }
    IEnumerator ClosingMatch(bool _isGol){
        blackPanel.SetActive(true);

        int playerWin = 0;
        for (int i = 0; i < playerInfos.Count; i++)
        {
            PlayerInfo playerInfoTemp = playerInfos[i];
            if(_isGol){
                if(playerInfoTemp.getBall){ playerInfoTemp.score++; playerWin = i; }
            }else{
                if(!playerInfoTemp.getBall) playerWin = i;
            }
            playerInfos[i] = playerInfoTemp;
        }
        print(playerWin+" win");
        bool isDraw = false;
        if(!_isGol){
            AttackerBehavior[] attacker = GameObject.FindObjectsOfType<AttackerBehavior>();
            for (int i = 0; i < attacker.Length; i++)
            {
                if(attacker[i].isPawnActive){
                    isDraw = true;
                    break;
                }
            }
        }

        if(isDraw)
            titleText.text = "Draw";
        else{
            if(!_isGol && !isDraw){
                PlayerInfo playerInfoTemp = playerInfos[playerWin];
                playerInfoTemp.score++;
                playerInfos[playerWin] = playerInfoTemp;
            }
            titleText.text = "Player " + (playerWin+1) + "Win";
        }
        print(_isGol+"/"+isDraw);
        yield return new WaitForSeconds(4);
        PreparingMatchStart();
    }
    void PreparingMatchStart(){
        // remove all pawn and ball
        ClearObject("Pawn");
        ClearObject("Ball");

        // Reset player and switch side
        if(!pinaltyMode) PlayerResetAndSwitchSide();

        if(currentMatch < maxMatch)
            currentMatch++;
        else{
            print("final match");;
            GameEnd();
            return;
        }
            
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