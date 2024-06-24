using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]

public class EnemyMover : MonoBehaviour
{
    [SerializeField] private float _enemySpeed;

    private Transform _target;

    public event Action<EnemyMover> EnemyTouchedTarget;

    private void Update()
    {
        MoveTowardsTarget(_target);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.TryGetComponent(out Target target))
        {
            EnemyTouchedTarget?.Invoke(this);
        }
    }

    public void InitializeTarget(Transform target)
    {
        _target = target;
    }
    
    private void MoveTowardsTarget(Transform target)
    {
        float defaultLookAtTargetPositionY = transform.position.y;
        Vector3 lookAtTargetPosition = new Vector3(target.position.x, defaultLookAtTargetPositionY, target.position.z);
        transform.LookAt(lookAtTargetPosition);
        transform.position = Vector3.MoveTowards(transform.position, target.position, _enemySpeed * Time.deltaTime);
    }
}