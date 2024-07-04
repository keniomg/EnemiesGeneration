using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class EnemiesSpawner : MonoBehaviour
{
    [SerializeField] private EnemyMover _enemyMover;
    [SerializeField] private int _poolCapacity;
    [SerializeField] private int _maxPoolSize;

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
            actionOnGet: (enemyMover) => AccompanyGetObject(enemyMover),
            actionOnRelease: (enemyMover) => AccompanyReleaseObject(enemyMover),
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

    private void AccompanyGetObject(EnemyMover enemyMover)
    {
        SetEnemyPosition(enemyMover);
        SetEnemyVelocity(enemyMover);
        enemyMover.InitializeDirection(GetEnemyDirection());
        enemyMover.gameObject.SetActive(true);
        enemyMover.EnemyTouchedBorder += OnEnemyTouchedTarget;
    }

    private void AccompanyReleaseObject(EnemyMover enemyMover)
    {
        enemyMover.gameObject.SetActive(false);
        enemyMover.EnemyTouchedBorder -= OnEnemyTouchedTarget;
    }

    private Vector3 GetEnemyDirection()
    {
        float minimumEnemyDirectionX = -1;
        float maximumEnemyDirectionX = 1;
        float minimumEnemyDirectionZ = -1;
        float maximumEnemyDirectionZ = 1;

        float enemyDirectionX = Random.Range(minimumEnemyDirectionX, maximumEnemyDirectionX);
        float enemyDirectionY = 0;
        float enemyDirectionZ = Random.Range(minimumEnemyDirectionZ, maximumEnemyDirectionZ);
        Vector3 enemyDirection = new(enemyDirectionX, enemyDirectionY, enemyDirectionZ);
        enemyDirection.Normalize();

        return enemyDirection;
    }

    private void SetEnemyPosition(EnemyMover enemyMover)
    {
        float enemyPositionX = Random.Range(transform.position.x - _spawnAreaWidth, transform.position.x + _spawnAreaWidth);
        float nonStuckHeightPosition = transform.position.y + enemyMover.transform.localScale.y;
        float enemyPositionY = nonStuckHeightPosition;
        float enemyPositionZ = Random.Range(transform.position.z - _spawnAreaLength, transform.position.z + _spawnAreaLength);
        Vector3 spawnTransformPosition = new(enemyPositionX, enemyPositionY, enemyPositionZ);

        enemyMover.transform.position = spawnTransformPosition;
    }

    private void SetEnemyVelocity(EnemyMover enemyMover)
    {
        enemyMover.TryGetComponent(out Rigidbody enemyRigidbody);

        enemyRigidbody.velocity = Vector3.zero;
        enemyRigidbody.angularVelocity = Vector3.zero;
    }

    private void OnEnemyTouchedTarget(EnemyMover enemyMover)
    {
        _pool.Release(enemyMover);
    }
}