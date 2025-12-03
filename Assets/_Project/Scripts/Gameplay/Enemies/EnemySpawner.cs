using UnityEngine;
using _Project.Scripts.Utilities;

namespace _Project.Scripts.Gameplay.Enemies
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private Enemy _enemyPrefab;
        [SerializeField] private int _initialPoolSize = 20;
        [SerializeField] private int _maxPoolSize = 20;
        [SerializeField] private float _spawnInterval = 2f;
        [SerializeField] private float _spawnRadius = 10f;

        private ObjectPool<Enemy> _enemyPool;
        private Transform _player;
        private float _timer;

        private void Start()
        {
            _enemyPool = new ObjectPool<Enemy>(_enemyPrefab, _initialPoolSize, transform, _maxPoolSize);

            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                _player = playerObj.transform;
            else
                Debug.LogWarning("EnemySpawner: Игрок не найден!");
        }

        private void Update()
        {
            if (_player == null) return;

            _timer += Time.deltaTime;
            if (_timer >= _spawnInterval)
            {
                _timer = 0f;
                SpawnEnemy();
            }
        }

        private void SpawnEnemy()
        {
            Vector2 randomPos = Random.insideUnitCircle.normalized * _spawnRadius;
            Vector3 spawnPos = _player.position + new Vector3(randomPos.x, 0, randomPos.y);

            Enemy enemy = _enemyPool.Get();
            if (enemy == null)
            {
                // Достигнут лимит мобов — просто выходим
                return;
            }

            enemy.transform.position = spawnPos;
            enemy.Initialize(_enemyPool);
        }
    }
}
