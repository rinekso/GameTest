using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MainmenuController : MonoBehaviour
{
    [SerializeField]
    Toggle ARMode;
    public void LoadLevel(string _scene){
        Application.LoadLevel(_scene);
    }
    public void StartGame2Player(){
        if(!ARMode.isOn){
            Application.LoadLevel("GameplayDuel");
        }else{
            Application.LoadLevel("GameplayAR");
        }
    }
    public void StartGamePinaltyMode(){
        if(!ARMode.isOn){
            Application.LoadLevel("PinaltyMode");
        }else{
            Application.LoadLevel("PinaltyModeAR");
        }
    }
}
