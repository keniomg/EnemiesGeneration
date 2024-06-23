using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class EnemiesSpawner : MonoBehaviour
{
    [SerializeField] private EnemyContoller _enemyContoller;
    [SerializeField] private int _poolCapacity;
    [SerializeField] private int _maxPoolSize;

    private ObjectPool<EnemyContoller> _pool;
    private float _spawnAreaLength;
    private float _spawnAreaWidth;
    private WaitForSeconds _waitForSeconds;

    private void Awake()
    {
        int numberOfSide = 2;
        _spawnAreaWidth = transform.localScale.x / numberOfSide;
        _spawnAreaLength = transform.localScale.z / numberOfSide;

        _pool = new ObjectPool<EnemyContoller>(
            createFunc: () => Instantiate(_enemyContoller),
            actionOnGet: (enemyController) => ExecuteActionOnGet(enemyController),
            actionOnRelease: (enemyController) => ExecuteActionOnRelease(enemyController),
            actionOnDestroy: (enemyController) => Destroy(enemyController),
            collectionCheck: true,
            defaultCapacity: _poolCapacity,
            maxSize: _maxPoolSize
            );
    }

    private void Start()
    {
        StartCoroutine(SpawnEnemies());
    }

    private void ExecuteActionOnGet(EnemyContoller enemyContoller)
    {
        SetEnemyRotation(enemyContoller);
        SetEnemyPosition(enemyContoller);
        SetEnemyVelocity(enemyContoller);
        enemyContoller.gameObject.SetActive(true);
        enemyContoller.EnemyTouchedBorder += ExecuteActionOnEnemyTouchedBorder;
    }

    private void ExecuteActionOnRelease(EnemyContoller enemyContoller)
    {
        enemyContoller.gameObject.SetActive(false);
        enemyContoller.EnemyTouchedBorder -= ExecuteActionOnEnemyTouchedBorder;
    }

    private void SetEnemyRotation(EnemyContoller enemyContoller)
    {
        float enemyRotationAngleX = 0;
        float minimumEnemyRotationAngleY = 0;
        float maximumEnemyRotationAngleY = 360;
        float enemyRotationAngleY = Random.Range(minimumEnemyRotationAngleY, maximumEnemyRotationAngleY);
        float enemyRotationAngleZ = 0;
        Vector3 spawnRotationPosition = new Vector3(enemyRotationAngleX, enemyRotationAngleY, enemyRotationAngleZ);

        enemyContoller.transform.rotation = Quaternion.Euler(spawnRotationPosition);
    }

    private void SetEnemyPosition(EnemyContoller enemyContoller)
    {
        float enemyPositionX = Random.Range(transform.position.x - _spawnAreaWidth, transform.position.x + _spawnAreaWidth);
        float nonStuckHeightPosition = transform.position.y + enemyContoller.transform.localScale.y;
        float enemyPositionY = nonStuckHeightPosition;
        float enemyPositionZ = Random.Range(transform.position.z - _spawnAreaLength, transform.position.z + _spawnAreaLength);
        Vector3 spawnTransformPosition = new Vector3(enemyPositionX, enemyPositionY, enemyPositionZ);

        enemyContoller.transform.position = spawnTransformPosition;
    }

    private void SetEnemyVelocity(EnemyContoller enemyContoller)
    {
        enemyContoller.TryGetComponent(out Rigidbody enemyRigidbody);

        enemyRigidbody.velocity = Vector3.zero;
        enemyRigidbody.angularVelocity = Vector3.zero;
    }

    private void ExecuteActionOnEnemyTouchedBorder(EnemyContoller enemyContoller)
    {
        enemyContoller.gameObject.SetActive(false);
        _pool.Release(enemyContoller);
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