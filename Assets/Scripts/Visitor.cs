using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Visitor : MonoBehaviour
{
    public GameObject door;
    private UnityEngine.AI.NavMeshAgent agent;
    private Entrance entranceScript;
    private POIManager poiManager;

    private Vector3 destination;
    private State state;
    private Visitor target;
    private int poiIndex;

    public double threshold = 3.0;
    public float distanceBehindLastVisitor = 3f;

    enum State
    {
        In_attraction,
        Waiting,
        Leaving,
        On_their_way,
        Wandering,
    }

    public void Update_state()
    {
        switch (state)
        {
            case State.On_their_way:
                state = State.Waiting;
                // Debug.Log("State: Waiting");
                break;
            case State.Waiting:
                state = State.In_attraction;
                gameObject.SetActive(false);
                //Debug.Log("State: In attraction");
                break;
            case State.In_attraction:
                state = State.Leaving;
                gameObject.SetActive(true);
                //Debug.Log("State: Leaving");
                break;
            case State.Leaving:
                state = (WanderingProbability < Random.Range(0f, 1f)) ? 
                State.On_their_way : State.Wandering;
                if (state == State.Wandering){ 
                    Debug.Log(name + " is wandering");
                }
                //Debug.Log("State: On_their_Way");
                break;
        }
    }

    public void Set_position(Vector3 position)
    {
        transform.position = position;
    }

    public void Set_target_to_null()
    {
        target = null;
    }

    private void Set_destination()
    {
        List<Vector3> POIs_position = poiManager.POIs_position;
        if (POIs_position.Count > 0)
        {
            poiIndex = new System.Random().Next(0, POIs_position.Count);
            destination = POIs_position[poiIndex];
            agent.SetDestination(destination);
        }
    }

    private void Go_to_waiting_queue()
    {
        int index = poiIndex + 1;
        POI poi = poiManager.transform.Find("POI " + index).GetComponent<POI>();
        entranceScript = poi?.GetComponentInChildren<Entrance>();

        if (entranceScript != null)
        {
            entranceScript.Visitor_reach_the_queue(this);
            // Debug.Log(this.name + " enters the waiting queue");
            // Debug.Log("Number of elements in waiting queue in POI " + index + entranceScript.Get_queue().Count);
        }
        else
        {
            Debug.LogError("Entrance script not found!");
        }
    }

    private void Stopmoving()
    {
        agent.isStopped = true;
    }

    public float WanderingProbability = 0.5f;
    public float wanderingRadius = 10f;  
    public float WanderingInterval = 30f; 

    private float timer = 0f;

    Vector3 GetRandomPointOnNavMesh()
    {
        UnityEngine.AI.NavMeshHit hit;
        Vector3 randomPoint = Vector3.zero;

        // Essayez de trouver un point aléatoire sur le navmesh
        if (UnityEngine.AI.NavMesh.SamplePosition(transform.position, out hit, 500.0f, UnityEngine.AI.NavMesh.AllAreas))
        {
            randomPoint = hit.position;
        }

        return randomPoint;
    }

    void UpdateWandering()
    { 
        // Si le NavMesh Agent a atteint sa destination ou si le temps d'attente est écoulé
        if (!agent.pathPending && agent.remainingDistance < 0.1f && timer >= WanderingInterval)
        {
            if (timer >= 6* timer)
            {
                Set_destination();
                state = State.On_their_way; 
            }
            else
            {
                SetRandomDestination();
                Debug.Log(name + " is wandering");
            }
            timer = 0f;
        }
        timer += Time.deltaTime;
    }

    void SetRandomDestination()
    {
        UnityEngine.AI.NavMeshHit hit;
        Vector3 randomPosition = transform.position + Random.insideUnitSphere * wanderingRadius;
        
        if (UnityEngine.AI.NavMesh.SamplePosition(randomPosition, out hit, wanderingRadius, UnityEngine.AI.NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        poiManager = GameObject.FindObjectOfType<POIManager>();

        if (poiManager == null)
        {
            Debug.LogError("POIManager not found!");
            return;
        }

        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        transform.position = door.transform.position;
        transform.rotation = door.transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (state == State.Leaving)
        {
            Set_destination();
            Update_state();
        }

        if (state == State.On_their_way)
        {
            int index = poiIndex + 1;
            POI poi = poiManager.transform.Find("POI " + index).GetComponent<POI>();
            entranceScript = poi?.GetComponentInChildren<Entrance>();

            if (entranceScript != null)
            {
                destination = entranceScript.Get_waiting_position_at_the_end_of_the_file(distanceBehindLastVisitor);
                agent.SetDestination(destination);
            }
            else
            {
                Debug.LogError("Entrance script not found!");
            }
        }

        if (Mathf.Abs(transform.position.x - destination.x) < threshold &&
            Mathf.Abs(transform.position.z - destination.z) < threshold &&
            state == State.On_their_way)
        {
            target = entranceScript.Get_last_visitor();
            Go_to_waiting_queue();
            agent.SetDestination(destination);
            Update_state();
        }
        if (state == State.Waiting)
        {
            Vector3 waiting_destination =
                (target == null) ?
                    entranceScript.Get_entrance_position() :
                    target.transform.position - (transform.forward * distanceBehindLastVisitor);
            if (Vector3.Distance(transform.position, waiting_destination) > 1f)
            {
                agent.isStopped = false;
                agent.SetDestination(waiting_destination);
            }
            else
            {
                Stopmoving();
            }
        }
        if (state == State.Wandering)
        {
            UpdateWandering();
        }
    }
}
