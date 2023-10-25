// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class Agent : MonoBehaviour
// {
//     private UnityEngine.AI.NavMeshAgent agent;
//     // Start is called before the first frame update
//     void Start()
//     {
//         agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
//     }

//     // Update is called once per frame
//     void Update()
//     {
//         if (Input.GetKeyDown(KeyCode.Mouse0)) // Vous pouvez utiliser un �v�nement d'entr�e pour d�clencher le d�placement.
//         {
//             Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
//             if (Physics.Raycast(ray, out RaycastHit hit))
//             {
//                 // D�finir la destination de l'agent sur le point de l'impact du rayon.
//                 agent.SetDestination(hit.point);
//             }
//         }
//     }

//     // obtenir la position des entrée POI, se diriger vers un au hasard
// }
