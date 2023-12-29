using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class POIManager : MonoBehaviour
{
    public List<Vector3> POIsPosition = new List<Vector3>();
    public void GetPOIsposition()
    {
        // Trouver tous les objets de type POI dans la scène.
        POI[] poiScripts = FindObjectsOfType<POI>();

        foreach (POI poiScript in poiScripts)
        {
            Transform entrance = poiScript.transform.Find("Entrance");

            if (entrance != null)
            {
                // Accéder à la position de l'entrée.
                Vector3 entrancePosition = entrance.position;
                Debug.Log("Position de l'entrée : " + entrancePosition);
                POIsPosition.Add(entrancePosition);
            }
        }
    }

    void Start()
    {
        GetPOIsposition();
    }
}
