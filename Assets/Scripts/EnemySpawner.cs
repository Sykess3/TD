using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private EnemyFactory _enemyFactory;
    [SerializeField] private GameBoard _gameBoard;
    [Tooltip("Time to next spawn in seconds")]
    [SerializeField, Range(0.1f, 10)]  private float _spawnSpeed;

    private EnemyCollection _enemyCollection = new EnemyCollection();
    private float _spawnProgress = 0f;
    
    

    private void Update()
    {
        _enemyCollection.MoveAll();
        Physics.SyncTransforms();

        _spawnProgress += Time.deltaTime * _spawnSpeed;
        while (_spawnProgress >= 1)
        {
            _spawnProgress -= 1f;
            Spawn();
        }
    }

    private void Spawn()
    {
        Enemy enemy = _enemyFactory.Get();
        GameTile spawnTile = 
            _gameBoard.SpawnPoints[Random.Range(0, _gameBoard.SpawnPoints.Count - 1)];
        enemy.SpawnOn(spawnTile);
        _enemyCollection.Add(enemy);
    }
}
