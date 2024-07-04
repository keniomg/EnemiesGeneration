using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]

public class EnemyMover : MonoBehaviour
{
    [SerializeField] protected float _enemySpeed;

    private Vector3 _direction;

    public event Action<EnemyMover> EnemyTouchedBorder;

    private void Update()
    {
        MoveInDirection();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.TryGetComponent(out Border border))
        {
            EnemyTouchedBorder?.Invoke(this);
        }
        else
        {
            string noBorderComponentMessage = "Other gameObject have no 'Border' component.";
            Debug.Log(noBorderComponentMessage);
        }
    }

    public void InitializeDirection(Vector3 direction)
    {
        _direction = direction;
    }

    private void MoveInDirection()
    {
        transform.Translate(_direction * _enemySpeed * Time.deltaTime);
    }
}