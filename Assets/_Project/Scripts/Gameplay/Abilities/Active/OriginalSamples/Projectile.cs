using UnityEngine;
using _Project.Gameplay.Attacks;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class Projectile : AttackInstanceBase
{
    [Header("Defaults (если не переопределили через Initialize)")]
    [SerializeField] private float defaultSpeed = 15f;

    [Header("VFX")]
    [SerializeField] private ParticleSystem sparks;

    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        if (_rb != null)
        {
            _rb.linearVelocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
        }
    }

    /// <summary>
    /// Универсальная инициализация: направление, урон, скорость, жизнь.
    /// Сигнатура оставлена такой же, как в твоём оригинале,
    /// чтобы ничего не сломать в существующем коде (Ability_ShootProjectileDefinition).
    /// </summary>
    public void Initialize(Vector3 direction, int damage, float speed, float lifeTime)
    {
        // Настраиваем урон и время жизни через базовый класс
        InitializeCommon(damage, lifeTime);

        float spd = speed > 0 ? speed : defaultSpeed;

        direction = direction.sqrMagnitude > 0.0001f
            ? direction.normalized
            : transform.forward;

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

    /// <summary>
    /// Что делать при завершении жизни (по таймеру или после попадания).
    /// Для снаряда — вернуть в пул.
    /// </summary>
    protected override void OnRelease()
    {
        if (_rb != null)
        {
            _rb.linearVelocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
        }

        ProjectilePool.Instance.Return(this);
    }

    /// <summary>
    /// Поведение при столкновении не с врагом.
    /// Повторяет твою оригинальную логику: столкновение с полом — в пул.
    /// </summary>
    protected override void OnHitNonEnemy(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Floor"))
        {
            Release();
        }
    }
}
