using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapSpawn : MonoBehaviour
{
    Ray ray;
    RaycastHit hit;
    float hitDistance = 100f;
    private void OnMouseDown() {
        Collider collider = GetComponent<Collider>();
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        if (collider.Raycast(ray, out hit, hitDistance))
        {
            if(hit.collider.tag == "Field" && GameController.instance.isStart)
            {
                Debug.Log(hit.point);
                GameController.instance.SpawnPawn(gameObject,hit.point);
            }
        } 
        print("mouse click");
    }
}
