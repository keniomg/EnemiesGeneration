using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(Collider))]

public class Target : MonoBehaviour
{
    [SerializeField] private float _targetSpeed;

    private int _currentWaypointIndex = 0;
    private List<Transform> _waypoints;

    private void Update()
    {
        MoveThroughWaypoints();
    }

    public void InitializeWaypoints(List<Transform> waypoints)
    {
        _waypoints = waypoints;
    }

    private void MoveThroughWaypoints()
    {
        float defaultHeightTargetPositionY = transform.position.y;
        Vector3 waypointPosition = new(_waypoints[_currentWaypointIndex].position.x, defaultHeightTargetPositionY, _waypoints[_currentWaypointIndex].position.z);

        if (transform.position == waypointPosition)
        {
            int nextWaypointIndex = _currentWaypointIndex + 1;
            _currentWaypointIndex = nextWaypointIndex % _waypoints.Count;
        }

        transform.position = Vector3.MoveTowards(transform.position, waypointPosition, _targetSpeed * Time.deltaTime);
    }
}