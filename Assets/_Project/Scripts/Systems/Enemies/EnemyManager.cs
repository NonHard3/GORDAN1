using System.Collections.Generic;
using UnityEngine;
using _Project.Core;              // ServiceLocator

namespace _Project.Systems.Enemies
{
    public class EnemyManager : MonoBehaviour, IEnemyService
    {
        private readonly List<IEnemy> _enemies = new List<IEnemy>();

        private void Awake()
        {
            // Регистрируем себя как сервис в ServiceLocator
            ServiceLocator.Register<IEnemyService>(this);
            Debug.Log("[EnemyManager] Registered IEnemyService in ServiceLocator.");
        }

        #region Регистрация врагов

        public void RegisterEnemy(IEnemy enemy)
        {
            if (enemy == null) return;
            if (!_enemies.Contains(enemy))
                _enemies.Add(enemy);
        }

        public void UnregisterEnemy(IEnemy enemy)
        {
            if (enemy == null) return;
            _enemies.Remove(enemy);
        }

        #endregion

        #region Поиск врагов

        public IEnemy GetEnemy(
            Vector3 fromPosition,
            float radius,
            EnemySearchMode mode = EnemySearchMode.Closest)
        {
            var candidates = GetEnemiesInRange(fromPosition, radius);
            if (candidates.Count == 0)
                return null;

            switch (mode)
            {
                case EnemySearchMode.Closest:
                    return GetClosest(fromPosition, candidates);

                case EnemySearchMode.Random:
                    return GetRandom(candidates);

                case EnemySearchMode.Farthest:
                    return GetFarthest(fromPosition, candidates);

                default:
                    return GetClosest(fromPosition, candidates);
            }
        }

        public List<IEnemy> GetEnemiesInRange(Vector3 fromPosition, float radius)
        {
            float radiusSqr = radius * radius;
            var result = new List<IEnemy>();

            foreach (var enemy in _enemies)
            {
                if (enemy == null) continue;
                if (!enemy.IsAlive) continue;

                Vector3 toEnemy = enemy.Position - fromPosition;
                if (toEnemy.sqrMagnitude <= radiusSqr)
                    result.Add(enemy);
            }

            return result;
        }

        private IEnemy GetClosest(Vector3 fromPosition, List<IEnemy> list)
        {
            IEnemy best = null;
            float bestDistSqr = float.MaxValue;

            foreach (var enemy in list)
            {
                if (enemy == null) continue;
                float dSqr = (enemy.Position - fromPosition).sqrMagnitude;
                if (dSqr < bestDistSqr)
                {
                    bestDistSqr = dSqr;
                    best = enemy;
                }
            }

            return best;
        }

        private IEnemy GetFarthest(Vector3 fromPosition, List<IEnemy> list)
        {
            IEnemy best = null;
            float bestDistSqr = 0f;

            foreach (var enemy in list)
            {
                if (enemy == null) continue;
                float dSqr = (enemy.Position - fromPosition).sqrMagnitude;
                if (dSqr > bestDistSqr)
                {
                    bestDistSqr = dSqr;
                    best = enemy;
                }
            }

            return best;
        }

        private IEnemy GetRandom(List<IEnemy> list)
        {
            if (list.Count == 0) return null;
            int index = Random.Range(0, list.Count);
            return list[index];
        }

        #endregion
    }
}