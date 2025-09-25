using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityExtensions;

public class SpawnManager : ReferenceBehaviour<SpawnManager>
{
    #region Fields

    [Header("Enemy")]
    public List<GameObject> EnemyPrefabs;
    public Transform EnemyParent;

    [Header("Obstacle")]
    public List<GameObject> ObstaclePrefabs;
    public Transform ObstacleParent;

    [Header("Spawn Settings Speed")]
    [SerializeField]
    private float _minSpeed = 1f;

    [SerializeField]
    private float _maxSpeed = 5f;

    [Header("Spawn Settings Healh")]
    [SerializeField]
    private int _minHealh = 1;

    [SerializeField]
    private int _maxHealh = 2;

    [Header("Spawn Settings X Position")]
    [SerializeField]
    private float _minXPosition = -5f;

    [SerializeField]
    private float _maxXPosition = 5f;

    [Header("Spawn Settings Size")]
    [SerializeField]
    private float _minSize = 15f;

    [SerializeField]
    private float _maxSize = 25f;

    [Header("Spawn Settings Shoot Delay")]
    [SerializeField]
    private int _minShootDelay = 1;

    [SerializeField]
    private int _maxShootDelay = 5;

    [Header("Spawn Settings Delay")]
    [SerializeField]
    private float _minDelay = 0f;

    [SerializeField]
    private float _maxDelay = 5f;

    public float RandomSpeed => Random.Range(_minSpeed, _maxSpeed);
    public int RandomHealh => Random.Range(_minHealh, _maxHealh);
    public float RandomPosition => Random.Range(_minXPosition, _maxXPosition);
    public float RandomSize => Random.Range(_minSize, _maxSize);
    public int RandomShootDelay => Random.Range(_minShootDelay, _maxShootDelay);
    public float RandomDelay => Random.Range(_minDelay, _maxDelay);
    public GameObject RandomEnemyPrefab => EnemyPrefabs[Random.Range(0, EnemyPrefabs.Count - 1)];
    public GameObject RandomObstaclePrefab =>
        ObstaclePrefabs[Random.Range(0, ObstaclePrefabs.Count - 1)];

    private CancellationTokenSource _cts;

    #endregion

    #region Unity Methods

    protected override void OnEnable()
    {
        base.OnEnable();
        UEvent.OnPlayGame += PlayGame;
        UEvent.OnStopGame += StopGame;
    }

    private void OnDisable()
    {
        UEvent.OnPlayGame -= PlayGame;
        UEvent.OnStopGame -= StopGame;
    }

    #endregion

    #region Private Methods

    private void PlayGame()
    {
        CallToken();
        StartSpawn().Forget();
    }

    private void StopGame()
    {
        CallToken();
    }

    private void CallToken()
    {
        _cts?.Cancel();
        _cts = new CancellationTokenSource();
    }

    private async UniTask StartSpawn()
    {
        try
        {
            while (GameManager.Ref.GameState == GameState.Game)
            {
                float xPos = RandomPosition;

                if (Random.Range(0, 2) == 0)
                {
                    Enemy enemy = Instantiate(RandomEnemyPrefab).GetComponent<Enemy>();
                    enemy.transform.parent = EnemyParent;
                    enemy.transform.localPosition = new Vector3(xPos, 0, 0);
                    enemy.SetParameters(RandomHealh, RandomSpeed, RandomShootDelay);
                }
                else
                {
                    Obstacle obstacle = Instantiate(RandomObstaclePrefab).GetComponent<Obstacle>();
                    obstacle.transform.parent = ObstacleParent;
                    obstacle.transform.localPosition = new Vector3(xPos, 0, 0);
                    obstacle.transform.localScale = Vector3.one * RandomSize;
                    obstacle.SetParameters(RandomHealh, RandomSpeed);
                }

                await UniTask.Delay(
                    System.TimeSpan.FromSeconds(RandomDelay),
                    cancellationToken: _cts.Token
                );
            }
        }
        catch (System.OperationCanceledException)
        {
            Debug.Log("Obstacle population cancelled");
        }
    }

    #endregion
}
