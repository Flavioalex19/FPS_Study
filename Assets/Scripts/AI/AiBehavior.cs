using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

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
    public AIState currentState = AIState.Patrolling;
    public float patrolSpeed = 3f;
    public float waitTime = 2f;
    public float fieldOfViewAngle = 90f;
    public float viewDistance = 10f;
    public LayerMask targetMask;
    public List<Transform> waypoints = new List<Transform>();
    public Transform _target;

    private int currentWaypointIndex = 0;
    private float waitTimer = 0f;

    private void Update()
    {
        switch (currentState)
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
        if (currentState != AIState.Shooting)
        {
            if (DetectPlayer())
            {
                ChangeState(AIState.Aiming);
            }
        }
    }

    private void Patrol()
    {
        // Move towards the current waypoint
        Vector3 targetPosition = waypoints[currentWaypointIndex].position;

        // Rotate towards the waypoint
        Quaternion targetRotation = Quaternion.LookRotation(targetPosition - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 2f);

        // Check if rotation is close enough to the target
        if (Quaternion.Angle(transform.rotation, targetRotation) < 1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, patrolSpeed * Time.deltaTime);
        }

        // Check if reached the waypoint
        if (transform.position == targetPosition)
        {
            ChangeState(AIState.Waiting);
        }
    }

    private void Wait()
    {
        // Start waiting timer
        waitTimer += Time.deltaTime;

        // Check if waiting time is over
        if (waitTimer >= waitTime)
        {
            waitTimer = 0f;
            ChangeState(AIState.Patrolling);
        }
    }

    private void Look()
    {
        // Perform looking behavior (e.g., rotate the AI character's head)

        // Example Gizmo for visualizing field of view
        Handles.color = Color.yellow;
        Handles.DrawWireArc(transform.position, transform.up, transform.forward, fieldOfViewAngle, viewDistance);
    }

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
        currentState = newState;

        switch (newState)
        {
            case AIState.Patrolling:
                // Choose the next waypoint
                currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Count;
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
