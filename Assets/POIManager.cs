using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class POIManager : MonoBehaviour
{
    public List<Vector3> POIs_position = new List<Vector3>();
    public void Get_POIs_position(){
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
                POIs_position.Add(entrancePosition);
            }
        }
    }

    void Start(){
        Get_POIs_position();
    }

        // visiteur se place dans la filequand il arrive près de la fin de la file
    // visiteur avec GameObject.Find() va trouver mes POIs qui seront gerer par un managerPOI où j'aurais le nombre de POI pour faire une boucle dessus
// le visiteur regarde si entr�e libre sinon se met dans la file, pour la sortie doit se casser pour laisser la place aux autres
// la sortie doit dire si elle est libre
}
