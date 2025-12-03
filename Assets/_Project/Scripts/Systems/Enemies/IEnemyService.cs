using System.Collections.Generic;
using UnityEngine;

public interface IEnemyService
{
    void RegisterEnemy(IEnemy enemy);
    void UnregisterEnemy(IEnemy enemy);

    IEnemy GetEnemy(
        Vector3 fromPosition,
        float radius,
        Project.Systems.Enemies.EnemySearchMode mode = Project.Systems.Enemies.EnemySearchMode.Closest);

    List<IEnemy> GetEnemiesInRange(Vector3 fromPosition, float radius);
}