using System.Collections.Generic;
using UnityEngine;

public class ProjectilePool : MonoBehaviour
{
    public static ProjectilePool Instance { get; private set; }

    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private int initialSize = 20;

    private readonly Queue<Projectile> pool = new();

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        for (int i = 0; i < initialSize; i++)
        {
            var p = CreateNew();
            Return(p);
        }
    }

    private Projectile CreateNew()
    {
        var go = Instantiate(projectilePrefab, transform);
        go.gameObject.SetActive(false);
        return go;
    }

    public Projectile Get(Vector3 position, Quaternion rotation)
    {
        Projectile p;
        if (pool.Count == 0)
            p = CreateNew();
        else
            p = pool.Dequeue();

        p.transform.SetPositionAndRotation(position, rotation);
        p.gameObject.SetActive(true);
        return p;
    }


    public void Return(Projectile p)
    {
        if (pool.Contains(p))
        {
            Debug.LogWarning($"[ProjectilePool] Попытка вернуть {p.name}, который уже в пуле!");
            return;
        }

        p.gameObject.SetActive(false);
        pool.Enqueue(p);
    }

}
