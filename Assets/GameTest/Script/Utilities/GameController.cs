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
        public int score;
        public GameObject pawnAtk;
        public GameObject pawnDef;
        public GameObject area;
        public PlayerSide playerSide;
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

    private void Awake() {
        instance = this;
    }
    private void Start() {
        StartGame();
    }
    public void StartGame(){
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
        Instantiate(result.pawnAtk, _position, new Quaternion());
    }
    void SpawnBall(){
        Collider collider = playerInfos.Find( x => x.playerSide == PlayerSide.ATTACKER).area.GetComponent<Collider>();
        Vector3 randomPos = new Vector3(
            UnityEngine.Random.Range(collider.bounds.min.x, collider.bounds.max.x), 
            collider.bounds.max.y + .025f,
            UnityEngine.Random.Range(collider.bounds.min.z, collider.bounds.max.z)
        );
        Instantiate(ballPrefabs,randomPos,new Quaternion());
    }

    // Update is called once per frame
    void Update()
    {
        if(isStart){
            // Time decrease
            currentTime -= Time.deltaTime;
            if(currentTime <= 0)
                GameEnd();
            else
                UpdateTimeText();
        }
    }
    void GameEnd(){
        isStart = false;
        print("Match End");

        // remove all pawn and ball
        ClearObject("Pawn");
        ClearObject("Ball");

        if(currentMatch < maxMatch)
            currentMatch++;
        StartGame();
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