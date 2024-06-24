using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]

public class ImprovedEnemyContoller : MonoBehaviour
{
    [SerializeField] private float _enemySpeed;

    public event Action<ImprovedEnemyContoller> EnemyTouchedBorder;

    private void Update()
    {
        transform.Translate(Vector3.forward * Time.deltaTime* _enemySpeed);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.TryGetComponent(out Target border))
        {
            EnemyTouchedBorder?.Invoke(this);
        }
    }
}