using System.Collections.Generic;
using UnityEngine;

namespace _Project.Systems.Enemies
{
    public interface IEnemyService
    {
        void RegisterEnemy(IEnemy enemy);
        void UnregisterEnemy(IEnemy enemy);

        IEnemy GetEnemy(
            Vector3 fromPosition,
            float radius,
            _Project.Systems.Enemies.EnemySearchMode mode = _Project.Systems.Enemies.EnemySearchMode.Closest);

        List<IEnemy> GetEnemiesInRange(Vector3 fromPosition, float radius);
    }
}