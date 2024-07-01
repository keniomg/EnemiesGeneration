using System.Collections;
using UnityEngine;
using UnityEngine.Pool;
using System.Collections.Generic;

public class ImprovedEnemiesSpawner : MonoBehaviour
{
    [SerializeField] private ImprovedEnemyMover _enemyMover;
    [SerializeField] private int _poolCapacity;
    [SerializeField] private int _maxPoolSize;
    [SerializeField] private Transform _target;
    [SerializeField] private List<Transform> _waypoints;

    private ObjectPool<ImprovedEnemyMover> _pool;
    private float _spawnAreaLength;
    private float _spawnAreaWidth;
    private WaitForSeconds _waitForSeconds;

    private void Awake()
    {
        int numberOfSide = 2;
        _spawnAreaWidth = transform.localScale.x / numberOfSide;
        _spawnAreaLength = transform.localScale.z / numberOfSide;

        _pool = new ObjectPool<ImprovedEnemyMover>(
            createFunc: () => Instantiate(_enemyMover),
            actionOnGet: (enemyMover) => AccompanyGetObjectWithAdditionalSettings(enemyMover),
            actionOnRelease: (enemyMover) => AccompanyReleaseObjectWithAdditionalSettings(enemyMover),
            actionOnDestroy: (enemyMover) => Destroy(enemyMover),
            collectionCheck: true,
            defaultCapacity: _poolCapacity,
            maxSize: _maxPoolSize
            );
    }

    private void Start()
    {
        StartCoroutine(SpawnEnemies());
    }

    private IEnumerator SpawnEnemies()
    {
        int spawnDelay = 2;
        _waitForSeconds = new(spawnDelay);

        while (true)
        {
            _pool.Get();
            yield return _waitForSeconds;
        }
    }

    private void AccompanyGetObjectWithAdditionalSettings(ImprovedEnemyMover enemyMover)
    {
        SetEnemyRotation(enemyMover);
        SetEnemyPosition(enemyMover);
        SetEnemyVelocity(enemyMover);

        enemyMover.InitializeTarget(_target);
        enemyMover.InitializeWaypoints(_waypoints);

        enemyMover.gameObject.SetActive(true);
        enemyMover.EnemyTouchedTarget += DisableEnemyTouchedTarget;
    }

    private void AccompanyReleaseObjectWithAdditionalSettings(ImprovedEnemyMover enemyMover)
    {
        enemyMover.gameObject.SetActive(false);
        enemyMover.EnemyTouchedTarget -= DisableEnemyTouchedTarget;
        enemyMover.ResetWaypointIndex();
    }

    private void SetEnemyRotation(ImprovedEnemyMover enemyMover)
    {
        float enemyRotationAngleX = 0;
        float enemyRotationAngleY = 0;
        float enemyRotationAngleZ = 0;
        Vector3 spawnRotationPosition = new(enemyRotationAngleX, enemyRotationAngleY, enemyRotationAngleZ);

        enemyMover.transform.rotation = Quaternion.Euler(spawnRotationPosition);
    }

    private void SetEnemyPosition(ImprovedEnemyMover enemyMover)
    {
        float enemyPositionX = Random.Range(transform.position.x - _spawnAreaWidth, transform.position.x + _spawnAreaWidth);
        float nonStuckHeightPosition = transform.position.y + enemyMover.transform.localScale.y;
        float enemyPositionY = nonStuckHeightPosition;
        float enemyPositionZ = Random.Range(transform.position.z - _spawnAreaLength, transform.position.z + _spawnAreaLength);
        Vector3 spawnTransformPosition = new(enemyPositionX, enemyPositionY, enemyPositionZ);

        enemyMover.transform.position = spawnTransformPosition;
    }

    private void SetEnemyVelocity(ImprovedEnemyMover enemyMover)
    {
        enemyMover.TryGetComponent(out Rigidbody enemyRigidbody);

        enemyRigidbody.velocity = Vector3.zero;
        enemyRigidbody.angularVelocity = Vector3.zero;
    }

    private void DisableEnemyTouchedTarget(ImprovedEnemyMover enemyMover)
    {
        enemyMover.gameObject.SetActive(false);
        _pool.Release(enemyMover);
    }
}