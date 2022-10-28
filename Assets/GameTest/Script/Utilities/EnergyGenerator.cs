using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnergyGenerator : MonoBehaviour
{
    enum Side
    {
        left = 0, right = 1
    }
    [Header("Initialize")]

    [SerializeField]
    int maxBar;
    [SerializeField]
    float definedRate = 1;
    Coroutine invokeRepeat;
    [SerializeField]
    Side sideSort;
    [SerializeField]
    GameObject barPrefab;
    [SerializeField]
    Color barFillColor;
    [SerializeField]
    Transform barContainer;

    [Space]
    [SerializeField]
    GameObject[] barEnergy;
    int currentBar = 0;
    // Start is called before the first frame update
    void Start()
    {
    }
    public void Initialize(){
        // Clear Container
        ClearContainer();
        currentBar = 0;

        barEnergy = new GameObject[maxBar];
        for (int i = 0; i < maxBar; i++)
        {
            GameObject gameObjectTemp = Instantiate(barPrefab,barContainer);
            Image image = gameObjectTemp.transform.GetChild(0).GetComponent<Image>();
            image.color = barFillColor;
            if(sideSort == Side.left){
                barEnergy[i] = gameObjectTemp;
                image.fillOrigin = (int) Image.OriginHorizontal.Left;
            }else{
                barEnergy[maxBar-(i+1)] = gameObjectTemp;
                gameObjectTemp.GetComponentInChildren<Image>().fillOrigin = 1;
                image.fillOrigin = (int) Image.OriginHorizontal.Right;
            }
        }
        // InvokeRepeating("GenerateEnergy",0,definedRate);
        invokeRepeat = StartCoroutine(GenerateEnergy(currentBar, definedRate));
    }
    void ClearContainer(){
        for (int i = 0; i < barContainer.childCount; i++)
        {
            Destroy(barContainer.GetChild(i).gameObject);
        }
    }
    IEnumerator GenerateEnergy(int id, float duration){
        print(barEnergy[id].name);
        Image image = barEnergy[id].transform.GetChild(0).GetComponent<Image>();
        float t = 0;

        while(t<1){

            yield return null;

            t += Time.deltaTime/duration;
            
            image.fillAmount = Mathf.Lerp(0,1,t);
        }
        
        if(currentBar+1 < barEnergy.Length){
            currentBar++;
            yield return GenerateEnergy(currentBar,definedRate);
        }
    }
}
