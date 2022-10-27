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
    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }
    void Initialize(){
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
    }
}
