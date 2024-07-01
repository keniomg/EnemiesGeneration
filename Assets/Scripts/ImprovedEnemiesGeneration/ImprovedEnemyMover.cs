using UnityEngine;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]

public class ImprovedEnemyMover : EnemyMover
{
    private List<Transform> _waypoints;
    private int _currentWaypointIndex = 0;
    private Transform _target;

    public event Action<ImprovedEnemyMover> EnemyTouchedTarget;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.TryGetComponent(out Target target))
        {
            EnemyTouchedTarget?.Invoke(this);
        }
        else
        {
            string noTargetComponentMessage = "Other gameObject have no 'Target' component.";
            Debug.Log(noTargetComponentMessage);
        }
    }

    public void InitializeTarget(Transform target)
    {
        _target = target;
    }

    public void ResetWaypointIndex()
    {
        _currentWaypointIndex = 0;
    }

    public void InitializeWaypoints(List<Transform> waypoints)
    {
        _waypoints = waypoints;
        _waypoints.Add(_target.transform);
    }

    private void MoveTowardsTarget()
    {
        float defaultHeightTargetPositionY = transform.position.y;
        Vector3 waypointPosition = new(_waypoints[_currentWaypointIndex].position.x, defaultHeightTargetPositionY, _waypoints[_currentWaypointIndex].position.z);

        if (transform.position == waypointPosition)
        {
            int nextWaypointIndex = _currentWaypointIndex + 1;
            _currentWaypointIndex = nextWaypointIndex % _waypoints.Count;
        }

        Vector3 lookAtTargetPosition = new(_waypoints[_currentWaypointIndex].position.x, defaultHeightTargetPositionY, _waypoints[_currentWaypointIndex].position.z);
        transform.LookAt(lookAtTargetPosition);
        transform.position = Vector3.MoveTowards(transform.position, waypointPosition, _enemySpeed * Time.deltaTime);
    }
}