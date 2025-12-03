using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class Projectile : MonoBehaviour
{
    [Header("Defaults (если не переопределили через Initialize)")]
    [SerializeField] private int defaultDamage = 5;
    [SerializeField] private float defaultSpeed = 15f;
    [SerializeField] private float defaultLifeTime = 5f;

    [Header("VFX")]
    [SerializeField] private ParticleSystem sparks;

    private Rigidbody _rb;
    private float _spawnTime;
    private bool _isReturned;

    // Текущие параметры выстрела
    private int _damage;
    private float _lifeTime;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        _spawnTime = Time.time;
        _isReturned = false;

        if (_rb != null)
        {
            _rb.linearVelocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
        }
    }

    private void FixedUpdate()
    {
        if (_isReturned) return;

        if (Time.time - _spawnTime > _lifeTime)
            ReturnToPool();
    }

    /// <summary>
    /// Универсальная инициализация: направление, урон, скорость, жизнь.
    /// </summary>
    public void Initialize(Vector3 direction, int damage, float speed, float lifeTime)
    {
        _damage = damage > 0 ? damage : defaultDamage;
        float spd = speed > 0 ? speed : defaultSpeed;
        _lifeTime = lifeTime > 0 ? lifeTime : defaultLifeTime;

        _spawnTime = Time.time;
        _isReturned = false;

        direction = direction.sqrMagnitude > 0.0001f ? direction.normalized : transform.forward;

        if (_rb != null)
        {
            _rb.linearVelocity = direction * spd;
            _rb.angularVelocity = Vector3.zero;
        }

        if (direction.sqrMagnitude > 0.0001f)
            transform.rotation = Quaternion.LookRotation(direction, Vector3.up);

        if (sparks != null)
        {
            sparks.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            sparks.Play(true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_isReturned) return;

        // Ищем врага через интерфейс IEnemy
        IEnemy enemy = other.GetComponent<IEnemy>() ?? other.GetComponentInParent<IEnemy>();

        if (enemy != null && enemy.IsAlive)
        {
            enemy.TakeDamage(_damage);
            ReturnToPool();
            return;
        }

        // Пример: столкновение с полом — просто вернуть в пул
        if (other.gameObject.layer == LayerMask.NameToLayer("Floor"))
            ReturnToPool();
    }

    private void ReturnToPool()
    {
        if (_isReturned) return;
        _isReturned = true;

        if (_rb != null)
        {
            _rb.linearVelocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
        }

        ProjectilePool.Instance.Return(this);
    }
}