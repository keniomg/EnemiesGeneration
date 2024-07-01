using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]

public class EnemyMover : MonoBehaviour
{
    [SerializeField] protected float _enemySpeed;

    public event Action<EnemyMover> EnemyTouchedBorder;

    protected void Update()
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
        transform.rotation = Quaternion.Euler(direction);
    }

    private void MoveInDirection()
    {
        transform.Translate(Vector3.forward * _enemySpeed * Time.deltaTime);
    }
}