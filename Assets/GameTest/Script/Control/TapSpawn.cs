using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapSpawn : MonoBehaviour
{
    [SerializeField]
    Camera camera;
    Ray ray;
    RaycastHit hit;
    float hitDistance = 100f;
    private void OnMouseDown() {
        Collider collider = GetComponent<Collider>();
        if(camera != null)
            ray = camera.ScreenPointToRay(Input.mousePosition);
        else
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        if (collider.Raycast(ray, out hit, hitDistance))
        {
            // print(hit.collider.tag);
            if(hit.collider.tag == "Field" && GameController.instance.isStart)
            {
                GameController.instance.SpawnPawn(gameObject,hit.point);
            }
        } 
    }
}
