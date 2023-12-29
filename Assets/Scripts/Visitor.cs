using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Visitor : MonoBehaviour
{
    public GameObject door;
    private NavMeshAgent agent;
    private Entrance entranceScript;
    private POIManager poiManager;

    private Vector3 destination;
    private State state;
    private Visitor target;
    private int poiIndex;

    public double threshold = 3.0;
    public float distanceBehindLastVisitor = 3f;
    public float wanderingProbability = 0.1f;
    public float wanderingRadius = 100f;
    public float wanderingInterval = 2f;

    private float timer = 0f;

    enum State
    {
        InAttraction,
        Waiting,
        Leaving,
        OnTheirWay,
        Wandering,
    }

    public void UpdateState()
    {
        switch (state)
        {
            case State.OnTheirWay:
                state = State.Waiting;
                break;
            case State.Waiting:
                state = State.InAttraction;
                gameObject.SetActive(false);
                break;
            case State.InAttraction:
                state = State.Leaving;
                gameObject.SetActive(true);
                break;
            case State.Leaving:
                state = (wanderingProbability < Random.Range(0f, 1f)) ? State.OnTheirWay : State.Wandering;
                if (state == State.Wandering)
                {
                    Debug.Log(name + " is wandering");
                }
                break;
        }
    }

    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }

    public void SetTargetToNull()
    {
        target = null;
    }

    private void SetDestination()
    {
        List<Vector3> POIsPosition = poiManager.POIsPosition;
        if (POIsPosition.Count > 0)
        {
            poiIndex = Random.Range(0, POIsPosition.Count);
            destination = POIsPosition[poiIndex];
            agent.SetDestination(destination);
        }
    }

    private void GoToWaitingQueue()
    {
        int index = poiIndex + 1;
        POI poi = poiManager.transform.Find("POI " + index).GetComponent<POI>();
        entranceScript = poi?.GetComponentInChildren<Entrance>();

        if (entranceScript != null)
        {
            entranceScript.VisitorReachTheQueue(this);
        }
        else
        {
            Debug.LogError("Entrance script not found!");
        }
    }

    private void StopMoving()
    {
        agent.isStopped = true;
    }

    private Vector3 GetRandomPointOnNavMesh()
    {
        NavMeshHit hit;
        Vector3 randomPoint = Vector3.zero;

        if (NavMesh.SamplePosition(transform.position, out hit, 500.0f, NavMesh.AllAreas))
        {
            randomPoint = hit.position;
        }

        return randomPoint;
    }

    private void UpdateWandering()
    {
        if (!agent.pathPending && agent.remainingDistance < 0.1f && timer >= wanderingInterval)
        {
            if (timer >= 6 * timer)
            {
                SetDestination();
                state = State.OnTheirWay;
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

    private void SetRandomDestination()
    {
        NavMeshHit hit;
        Vector3 randomPosition = transform.position + Random.insideUnitSphere * wanderingRadius;

        if (NavMesh.SamplePosition(randomPosition, out hit, wanderingRadius, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
        poiManager = FindObjectOfType<POIManager>();

        if (poiManager == null)
        {
            Debug.LogError("POIManager not found!");
            return;
        }

        agent = GetComponent<NavMeshAgent>();
        transform.position = door.transform.position;
        transform.rotation = door.transform.rotation;
        SetDestination();
        state = State.OnTheirWay;
    }

    // Update is called once per frame
    private void Update()
    {
        if (state == State.Leaving)
        {
            SetDestination();
            UpdateState();
        }

        if (state == State.OnTheirWay)
        {
            int index = poiIndex + 1;
            POI poi = poiManager.transform.Find("POI " + index).GetComponent<POI>();
            entranceScript = poi?.GetComponentInChildren<Entrance>();

            if (entranceScript != null)
            {
                destination = entranceScript.GetWaitingPositionAtTheEndOfTheQueue(distanceBehindLastVisitor);
                agent.SetDestination(destination);
            }
            else
            {
                Debug.LogError("Entrance script not found!");
            }
        }

        if (Mathf.Abs(transform.position.x - destination.x) < threshold &&
            Mathf.Abs(transform.position.z - destination.z) < threshold &&
            state == State.OnTheirWay)
        {
            target = entranceScript.GetLastVisitor();
            GoToWaitingQueue();
            agent.SetDestination(destination);
            UpdateState();
        }

        if (state == State.Waiting)
        {
            Vector3 waitingDestination = (target == null) ?
                entranceScript.GetEntrancePosition() :
                target.transform.position - (transform.forward * distanceBehindLastVisitor);

            if (Vector3.Distance(transform.position, waitingDestination) > 1f)
            {
                agent.isStopped = false;
                agent.SetDestination(waitingDestination);
            }
            else
            {
                StopMoving();
            }
        }

        if (state == State.Wandering)
        {
            UpdateWandering();
        }
    }
}
