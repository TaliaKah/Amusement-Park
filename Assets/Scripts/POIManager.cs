using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class POIManager : MonoBehaviour
{
    public List<Vector3> POIsPosition = new List<Vector3>();
    public void GetPOIsposition()
    {
        POI[] poiScripts = FindObjectsOfType<POI>();

        foreach (POI poiScript in poiScripts)
        {
            Transform entrance = poiScript.transform.Find("Entrance");

            if (entrance != null)
            {
                Vector3 entrancePosition = entrance.position;
                POIsPosition.Add(entrancePosition);
            }
        }
    }

    void Start()
    {
        GetPOIsposition();
    }
}
