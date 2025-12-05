using UnityEngine;
using _Project.Utilities;

public class AreaAttackPool : MonoBehaviour
{
    public static AreaAttackPool Instance { get; private set; }

    [Header("Pool Settings")]
    [SerializeField] private AreaAttackInstance areaPrefab;
    [SerializeField] private int initialSize = 10;
    [SerializeField] private int maxSize = 30;

    private ObjectPool<AreaAttackInstance> _pool;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (areaPrefab == null)
        {
            Debug.LogError("[AreaAttackPool] Prefab не задан!");
            return;
        }

        _pool = new ObjectPool<AreaAttackInstance>(
            areaPrefab,
            initialSize,
            transform,
            maxSize
        );
    }

    public AreaAttackInstance Get(Vector3 position, Quaternion rotation)
    {
        if (_pool == null)
        {
            Debug.LogError("[AreaAttackPool] Пул не инициализирован.");
            return null;
        }

        var area = _pool.Get();
        if (area == null)
            return null;

        area.transform.SetPositionAndRotation(position, rotation);
        return area;
    }

    public void Return(AreaAttackInstance area)
    {
        if (_pool == null)
            return;

        _pool.Return(area);
    }
}
