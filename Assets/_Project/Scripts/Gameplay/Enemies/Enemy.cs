using System;
using UnityEngine;
using Project.Core;                 // ServiceLocator
using _Project.Scripts.Utilities;   // ObjectPool
using Project.Systems.Enemies;      // IEnemyService

namespace _Project.Scripts.Gameplay.Enemies
{
    [RequireComponent(typeof(Collider))]
    public class Enemy : MonoBehaviour, IEnemy
    {
        [Header("Settings")]
        [SerializeField] private float _moveSpeed = 2f;
        [SerializeField] private int _maxHealth = 3;
        [SerializeField] private int _expPoint = 1;

        private int _currentHealth;
        private Transform _player;
        private ObjectPool<Enemy> _pool;

        private bool _isInitialized;

        public bool IsAlive => _currentHealth > 0;
        public Vector3 Position => transform.position;

        public void Initialize(ObjectPool<Enemy> pool)
        {
            _pool = pool;
            _currentHealth = _maxHealth;

            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                _player = playerObj.transform;
            else
                Debug.LogWarning("Enemy: Игрок не найден!");

            _isInitialized = true;
        }

        private void OnEnable()
        {
            TryRegisterToEnemyService();
        }

        private void OnDisable()
        {
            TryUnregisterFromEnemyService();
        }

        private void OnDestroy()
        {
            TryUnregisterFromEnemyService();
        }

        private void TryRegisterToEnemyService()
        {
            try
            {
                var enemyService = ServiceLocator.Get<IEnemyService>();
                enemyService.RegisterEnemy(this);
            }
            catch (Exception)
            {
                Debug.LogWarning("Enemy: IEnemyService не найден в ServiceLocator. Убедись, что EnemyManager есть на сцене.");
            }
        }

        private void TryUnregisterFromEnemyService()
        {
            try
            {
                var enemyService = ServiceLocator.Get<IEnemyService>();
                enemyService.UnregisterEnemy(this);
            }
            catch (Exception)
            {
                // Тихо игнорируем, если сервис не найден (например, при выгрузке сцены)
            }
        }

        private void Update()
        {
            if (!_isInitialized || _player == null) return;

            // Движение к игроку по XZ-плоскости
            Vector3 direction = (_player.position - transform.position).normalized;
            direction.y = 0f;

            transform.position += direction * _moveSpeed * Time.deltaTime;
        }

        public void TakeDamage(int amount)
        {
            _currentHealth -= amount;

            if (_currentHealth <= 0)
                Die();
        }

        private void Die()
        {
            ExperienceSpawner.Instance.SpawnExp(transform.position, _expPoint);
            _pool.Return(this);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, 0.5f);
        }
    }
}