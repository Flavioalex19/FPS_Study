using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public enum AIState
{
    Patrolling,
    Waiting,
    Looking,
    Aiming,
    Shooting
}
public class AiBehavior : MonoBehaviour
{
    public AIState CurrentState = AIState.Patrolling;
    [SerializeField] float patrolSpeed = 3f;
    [SerializeField] float waitTime = 2f;
    [SerializeField] float fieldOfViewAngle = 90f;
    [SerializeField] float viewDistance = 10f;
    [SerializeField] LayerMask targetMask;
    [SerializeField] List<Transform> waypoints = new List<Transform>();
    [SerializeField] Transform _target;

    int _currentWaypointIndex = 0;
    float _waitTimer = 0f;

    NavMeshAgent _agent;

    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        switch (CurrentState)
        {
            case AIState.Patrolling:
                Patrol();
                break;
            case AIState.Waiting:
                Wait();
                break;
            case AIState.Looking:
                Look();
                break;
            case AIState.Aiming:
                Aim();
                break;
            case AIState.Shooting:
                Shoot();
                break;
        }

        // Check if there is a player in the field of view
        if (CurrentState != AIState.Shooting)
        {
            if (DetectPlayer())
            {
                ChangeState(AIState.Aiming);
            }
        }
    }

    private void Patrol()
    {
        // Rotate towards the current waypoint
        Vector3 targetPosition = waypoints[_currentWaypointIndex].position;
        Quaternion targetRotation = Quaternion.LookRotation(targetPosition - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 2f);

        // Check if rotation is close enough to the target
        if (Quaternion.Angle(transform.rotation, targetRotation) < 1f)
        {
            // Delay rotation before moving
            _waitTimer += Time.deltaTime;

            if (_waitTimer >= 5f)
            {
                _waitTimer = 0f;
                _agent.isStopped = false;
                // Move towards the current waypoint
                _agent.SetDestination(targetPosition);
            }
        }

        // Check if reached the waypoint
        if (_agent.remainingDistance <= _agent.stoppingDistance)
        {
            ChangeState(AIState.Waiting);
        }
    }

    private void Wait()
    {
        // Start waiting timer
        _waitTimer += Time.deltaTime;

        // Check if waiting time is over
        if (_waitTimer >= waitTime)
        {
            _waitTimer = 0f;
            ChangeState(AIState.Patrolling);
        }
    }

    private void Look()
    {
        // Perform looking behavior (e.g., rotate the AI character's head)

        if(_target == null)
        {
            // Example Gizmo for visualizing field of view
            Handles.color = Color.yellow;
            Handles.DrawWireArc(transform.position, transform.up, transform.forward, fieldOfViewAngle, viewDistance);
        }
        
    }
    //Aim at waypoint or target
    private void Aim()
    {
        if (_target != null)
        {
            // Rotate towards the target
            Vector3 targetDirection = _target.position - transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 2f);

            // Check if rotation is close enough to the target
            if (Quaternion.Angle(transform.rotation, targetRotation) < 1f)
            {
                // Perform aiming behavior (e.g., shoot at the target)
            }
        }

        if (!DetectPlayer())
        {
            ChangeState(AIState.Looking);
        }
        else if (_target != null)
        {
            ChangeState(AIState.Shooting);
        }
    }

    private void Shoot()
    {
        // Perform shooting behavior
        _agent.isStopped = true;
        Aim();

        if (!DetectPlayer())
        {
            ChangeState(AIState.Looking);
        }
    }

    private bool DetectPlayer()
    {
        // Check if there is a player in the AI's field of view

        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewDistance, targetMask);

        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, directionToTarget) < fieldOfViewAngle / 2f)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget))
                {
                    _target = target;
                    return target;
                }
            }
        }

        return false;
    }

    private void ChangeState(AIState newState)
    {
        CurrentState = newState;

        switch (newState)
        {
            case AIState.Patrolling:
                //_currentWaypointIndex = (_currentWaypointIndex + 1) % waypoints.Count;
                _currentWaypointIndex = Random.Range(0, waypoints.Count);
                _agent.SetDestination(waypoints[_currentWaypointIndex].position);
                break;
            case AIState.Waiting:
                // Optionally, play a waiting animation or perform any necessary setup
                break;
            case AIState.Looking:
                // Optionally, perform any necessary setup for looking behavior
                break;
            case AIState.Aiming:
                // Optionally, play an aiming animation or perform any necessary setup
                break;
            case AIState.Shooting:
                // Optionally, play a shooting animation or perform any necessary setup
                break;
        }
    }

    private void OnDrawGizmos()
    {
        // Display the AI's patrol path in the editor
        Gizmos.color = Color.cyan;

        for (int i = 0; i < waypoints.Count; i++)
        {
            if (waypoints[i] != null)
            {
                Gizmos.DrawSphere(waypoints[i].position, 0.2f);
            }
        }
    }
}
