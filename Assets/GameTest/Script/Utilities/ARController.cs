using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ARController : MonoBehaviour
{
    ARRaycastManager raycastManager;
    [SerializeField]
    ARCameraManager aRCameraManager;
    List<ARRaycastHit> hits;
    [SerializeField]
    GameObject model;
    bool placeedModel = false;
    // Start is called before the first frame update
    void Start()
    {
        raycastManager = GetComponent<ARRaycastManager>();
        print(aRCameraManager.permissionGranted);
        hits = new List<ARRaycastHit>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0)){
            if(raycastManager.Raycast(Input.mousePosition, hits)){
                Pose pose = hits[0].pose;
                if(!placeedModel){
                    placeedModel = true;
                    Instantiate(model, pose.position, pose.rotation);

                    GetComponent<ARPlaneManager>().enabled = false;
                    GameObject[] arplanes = GameObject.FindGameObjectsWithTag("ARPlane");
                    for (int i = 0; i < arplanes.Length; i++)
                    {
                        arplanes[i].SetActive(false);
                    }
                }
            }
        }
    }
}
