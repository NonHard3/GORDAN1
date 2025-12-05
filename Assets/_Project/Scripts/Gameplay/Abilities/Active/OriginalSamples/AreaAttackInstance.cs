using System.Collections.Generic;
using UnityEngine;
using _Project.Gameplay.Attacks;

[RequireComponent(typeof(Collider))]
public class AreaAttackInstance : AttackInstanceBase
{
    [Header("Area Settings")]
    [SerializeField] private float tickInterval = 0.5f;

    private Collider _collider;
    private readonly Dictionary<Collider, float> _nextHit = new();

    private void Awake()
    {
        _collider = GetComponent<Collider>();
        if (_collider != null)
            _collider.isTrigger = true;
    }

    /// <summary>
    /// Инициализация ауры
    /// </summary>
    public void InitializeArea(int damage, float lifeTime, float tickInterval, float radius)
    {
        InitializeCommon(damage, lifeTime);

        if (tickInterval > 0f)
            this.tickInterval = tickInterval;

        if (radius > 0f)
            SetRadius(radius);
    }

    /// <summary>
    /// Устанавливает радиус зоны и корректирует размер коллайдера.
    /// Работает для SphereCollider и BoxCollider.
    /// </summary>
    private void SetRadius(float radius)
    {
        transform.localScale = new Vector3(radius, radius, radius);
        //if (_collider is SphereCollider sphere)
        //{
        //    sphere.radius = radius;
        //}
        //else if (_collider is BoxCollider box)
        //{
        //    box.size = new Vector3(radius * 2f, box.size.y, radius * 2f);
        //}
        //else
        //{
        //    // Если другой тип — масштабируем объект целиком
        //    transform.localScale = new Vector3(radius * 2f, 1f, radius * 2f);
        //}
    }

    protected override void OnTriggerStay(Collider other)
    {
        if (_isReleased)
            return;

        if (_nextHit.TryGetValue(other, out float next) && Time.time < next)
            return;

        _nextHit[other] = Time.time + tickInterval;
        HandleTriggerEnter(other);
    }

    protected override void OnTriggerExit(Collider other)
    {
        _nextHit.Remove(other);
    }

    protected override void OnHitEnemy(IEnemy enemy, Collider other)
    {
        // Урон наносится, но зона не исчезает
    }

    protected override void OnRelease()
    {
        _nextHit.Clear();

        if (AreaAttackPool.Instance != null)
            AreaAttackPool.Instance.Return(this);
        else
            Destroy(gameObject); // на случай, если пула нет на сцене
    }
}
