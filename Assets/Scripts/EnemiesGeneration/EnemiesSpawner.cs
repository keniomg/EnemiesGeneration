using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class EnemiesSpawner : MonoBehaviour
{
    [SerializeField] private EnemyMover _enemyMover;
    [SerializeField] private int _poolCapacity;
    [SerializeField] private int _maxPoolSize;
    [SerializeField] private Transform _target;

    private ObjectPool<EnemyMover> _pool;
    private float _spawnAreaLength;
    private float _spawnAreaWidth;
    private WaitForSeconds _waitForSeconds;

    private void Awake()
    {
        int numberOfSide = 2;
        _spawnAreaWidth = transform.localScale.x / numberOfSide;
        _spawnAreaLength = transform.localScale.z / numberOfSide;

        _pool = new ObjectPool<EnemyMover>(
            createFunc: () => Instantiate(_enemyMover),
            actionOnGet: (enemyMover) => GetObjectFromPool(enemyMover),
            actionOnRelease: (enemyMover) => ReleaseObjectToPool(enemyMover),
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

    private void GetObjectFromPool(EnemyMover enemyMover)
    {
        SetEnemyRotation(enemyMover);
        SetEnemyPosition(enemyMover);
        SetEnemyVelocity(enemyMover);
        enemyMover.InitializeTarget(_target);
        enemyMover.gameObject.SetActive(true);
        enemyMover.EnemyTouchedTarget += ExecuteActionOnEnemyTouchedTarget;
    }

    private void ReleaseObjectToPool(EnemyMover enemyMover)
    {
        enemyMover.gameObject.SetActive(false);
        enemyMover.EnemyTouchedTarget -= ExecuteActionOnEnemyTouchedTarget;
    }

    private void SetEnemyRotation(EnemyMover enemyMover)
    {
        float enemyRotationAngleX = 0;
        float enemyRotationAngleY = 0;
        float enemyRotationAngleZ = 0;
        Vector3 spawnRotationPosition = new Vector3(enemyRotationAngleX, enemyRotationAngleY, enemyRotationAngleZ);

        enemyMover.transform.rotation = Quaternion.Euler(spawnRotationPosition);
    }

    private void SetEnemyPosition(EnemyMover enemyMover)
    {
        float enemyPositionX = Random.Range(transform.position.x - _spawnAreaWidth, transform.position.x + _spawnAreaWidth);
        float nonStuckHeightPosition = transform.position.y + enemyMover.transform.localScale.y;
        float enemyPositionY = nonStuckHeightPosition;
        float enemyPositionZ = Random.Range(transform.position.z - _spawnAreaLength, transform.position.z + _spawnAreaLength);
        Vector3 spawnTransformPosition = new Vector3(enemyPositionX, enemyPositionY, enemyPositionZ);

        enemyMover.transform.position = spawnTransformPosition;
    }

    private void SetEnemyVelocity(EnemyMover enemyMover)
    {
        enemyMover.TryGetComponent(out Rigidbody enemyRigidbody);

        enemyRigidbody.velocity = Vector3.zero;
        enemyRigidbody.angularVelocity = Vector3.zero;
    }

    private void ExecuteActionOnEnemyTouchedTarget(EnemyMover enemyMover)
    {
        enemyMover.gameObject.SetActive(false);
        _pool.Release(enemyMover);
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
}