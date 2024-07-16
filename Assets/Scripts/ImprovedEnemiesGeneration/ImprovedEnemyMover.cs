using UnityEngine;
using System;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]

public class ImprovedEnemyMover : EnemyMover
{
    private Transform _target;

    public event Action<ImprovedEnemyMover> EnemyTouchedTarget;

    private void Update()
    {
        MoveTowardsTarget();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.TryGetComponent(out Target target))
        {
            EnemyTouchedTarget?.Invoke(this);
        }
        else
        {
            return;
        }
    }

    public void InitializeTarget(Transform target)
    {
        _target = target;
    }

    private void MoveTowardsTarget()
    {
        float defaultHeightTargetPositionY = transform.position.y;
        Vector3 lookAtTargetPosition = new(_target.position.x, defaultHeightTargetPositionY, _target.position.z);
        transform.LookAt(lookAtTargetPosition);
        transform.position = Vector3.MoveTowards(transform.position, _target.position, _enemySpeed * Time.deltaTime);
    }
}