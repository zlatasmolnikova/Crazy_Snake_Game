using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class AIWaypointNavigation : MonoBehaviour
{
    private NavMeshAgent agent;
    [SerializeField]
    private List<Transform> waypoints;
    private int currentWayPointNum;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!agent.hasPath)
            SetNextDestination();
    }

    void SetNextDestination()
    {
        currentWayPointNum++;
        if (currentWayPointNum >= waypoints.Count)
            currentWayPointNum = 0;
        agent.SetDestination(waypoints[currentWayPointNum].position);
    }
}
