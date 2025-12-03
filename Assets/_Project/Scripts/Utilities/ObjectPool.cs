using System.Collections.Generic;
using UnityEngine;

namespace _Project.Scripts.Utilities
{
    /// <summary>
    /// Универсальный пул объектов для любых MonoBehaviour-префабов.
    /// </summary>
    public class ObjectPool<T> where T : MonoBehaviour
    {
        private readonly Queue<T> _pool = new();
        private readonly T _prefab;
        private readonly Transform _parent;
        private readonly int _maxSize;

        private int _activeCount;

        public ObjectPool(T prefab, int initialSize, Transform parent = null, int maxSize = 50)
        {
            _prefab = prefab;
            _parent = parent;
            _maxSize = maxSize;

            Debug.Log(_maxSize);

            for (int i = 0; i < initialSize; i++)
            {
                var obj = GameObject.Instantiate(_prefab, _parent);
                obj.gameObject.SetActive(false);
                _pool.Enqueue(obj);
            }
        }

        public T Get()
        {
            if (_pool.Count > 0)
            {
                var obj = _pool.Dequeue();
                obj.gameObject.SetActive(true);
                _activeCount++;
                return obj;
            }

            if (_activeCount < _maxSize)
            {
                var obj = GameObject.Instantiate(_prefab, _parent);
                _activeCount++;
                return obj;
            }

            // Если достигнут предел — возвращаем null
            Debug.LogWarning($"[ObjectPool] Достигнут лимит: {_maxSize} активных объектов.");
            return null;
        }

        public void Return(T obj)
        {
            if (_pool.Contains(obj))
            {
                Debug.LogWarning($"[ObjectPool] Попытка вернуть объект {obj.name}, который уже в пуле!");
                return;
            }

            obj.gameObject.SetActive(false);
            _pool.Enqueue(obj);
            _activeCount = Mathf.Max(0, _activeCount - 1);
        }
    }
}