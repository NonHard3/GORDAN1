using UnityEngine;

public interface IEnemy
{
    void TakeDamage(int amount);
    bool IsAlive { get; }
    Vector3 Position { get; }
}