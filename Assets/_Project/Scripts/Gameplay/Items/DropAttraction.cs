using UnityEngine;
using System.Collections.Generic;

public class DropAttraction : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _moveSpeed = 10f;

    public static DropAttraction Instance { get; private set; }

    private Transform _player;
    private readonly List<Transform> _targets = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            _player = playerObj.transform;
        }
        else
        {
            Debug.LogWarning("DropAttraction: Игрок с тегом 'Player' не найден!");
        }
    }

    /// <summary>
    /// Добавить частицу опыта в список притягиваемых
    /// </summary>
    public void AddTarget(GameObject target)
    {
        if (target == null) return;
        Transform t = target.transform;

        if (!_targets.Contains(t))
            _targets.Add(t);
    }

    public void RemoveTarget(Transform target)
    {
        if (target == null) return;
        _targets.Remove(target);
    }

    private void Update()
    {
        if (_player == null || _targets.Count == 0)
            return;

        float delta = Time.deltaTime;

        // Идём с конца, чтобы безопасно удалять элементы
        for (int i = _targets.Count - 1; i >= 0; i--)
        {
            Transform t = _targets[i];

            if (t == null || !t.gameObject.activeInHierarchy)
            {
                _targets.RemoveAt(i);
                continue;
            }

            Vector3 direction = (_player.position - t.position).normalized;
            direction.y = 0f;

            t.position += direction * _moveSpeed * delta;
        }
    }
}
