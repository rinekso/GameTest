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
    public int currentBar = 0;
    // Start is called before the first frame update
    void Start()
    {
    }
    public void Initialize(){
        if(invokeRepeat != null) StopCoroutine(invokeRepeat);
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
    IEnumerator GenerateEnergy(int _id, float _duration){
        Image image = barEnergy[_id].transform.GetChild(0).GetComponent<Image>();
        float t = image.fillAmount;

        while(t<1){

            yield return null;

            t += Time.deltaTime/_duration;
            
            image.fillAmount = Mathf.Lerp(0,1,t);
        }
        
        currentBar++;

        while (currentBar >= barEnergy.Length)
        {
            yield return null;
        }
        yield return GenerateEnergy(currentBar,definedRate);
    }
    public void EnergyDecrease(int _amount){
        StopCoroutine(invokeRepeat);
        // get value current bar
        float valueCurrentBar = 0;
        if(currentBar < barEnergy.Length){
            Image image = barEnergy[currentBar].transform.GetChild(0).GetComponent<Image>();
            valueCurrentBar = image.fillAmount;
        }

        // decrease bar
        currentBar -= _amount;
        if(currentBar < 0) currentBar = 0;

        // Change ui image fill
        for (int i = 0; i < barContainer.childCount; i++)
        {
            Image imageBar = barEnergy[i].transform.GetChild(0).GetComponent<Image>();
            if(i < currentBar)
                imageBar.fillAmount = 1;
            else if(i == currentBar){
                imageBar.fillAmount = valueCurrentBar;
            }else{
                imageBar.fillAmount = 0;
            }
        }

        invokeRepeat = StartCoroutine(GenerateEnergy(currentBar, definedRate));
    }
}
