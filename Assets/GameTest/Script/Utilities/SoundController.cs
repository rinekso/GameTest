using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    public static SoundController instance;
    [SerializeField]
    AudioSource BGM;
    [SerializeField]
    AudioSource SFX;
    [SerializeField]
    AudioClip[] clips;
    private void Awake() {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        BGM.Play();
    }
    public void PlaySFX(int index){
        SFX.Stop();
        SFX.clip = clips[index];
        SFX.Play();
    }
}
