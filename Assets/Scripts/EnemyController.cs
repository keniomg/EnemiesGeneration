using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]

public class EnemyContoller : MonoBehaviour
{
    [SerializeField] private float _enemySpeed;

    public event Action<EnemyContoller> EnemyTouchedBorder;

    private void Update()
    {
        transform.Translate(Vector3.forward * Time.deltaTime* _enemySpeed);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.TryGetComponent(out Border border))
        {
            EnemyTouchedBorder?.Invoke(this);
        }
    }
}