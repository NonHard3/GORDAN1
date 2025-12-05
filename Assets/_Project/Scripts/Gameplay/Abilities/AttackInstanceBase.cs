using UnityEngine;

namespace _Project.Gameplay.Attacks
{
    [RequireComponent(typeof(Collider))]
    public abstract class AttackInstanceBase : MonoBehaviour
    {
        [Header("Defaults (если не переопределили через Initialize)")]
        [SerializeField] protected int defaultDamage = 5;
        [SerializeField] protected float defaultLifeTime = 5f;

        protected int _damage;
        protected float _lifeTime;

        protected float _spawnTime;
        protected bool _isReleased;

        protected virtual void OnEnable()
        {
            _spawnTime = Time.time;
            _isReleased = false;
        }

        protected virtual void FixedUpdate()
        {
            if (_isReleased)
                return;

            if (_lifeTime > 0f && Time.time - _spawnTime > _lifeTime)
            {
                OnLifeTimeEnded();
                Release();
            }
        }

        protected void InitializeCommon(int damage, float lifeTime)
        {
            _damage = damage > 0 ? damage : defaultDamage;
            _lifeTime = lifeTime > 0 ? lifeTime : defaultLifeTime;
            _spawnTime = Time.time;
            _isReleased = false;
        }

        protected virtual void OnLifeTimeEnded() { }

        // ---------- COLLISION PIPELINE (3D) ----------
        protected virtual void OnTriggerEnter(Collider other)
        {
            if (_isReleased) return;
            HandleTriggerEnter(other);
        }

        /// <summary>
        /// Эти методы теперь виртуальные, можно переопределять в наследниках.
        /// </summary>
        protected virtual void OnTriggerStay(Collider other) { }

        protected virtual void OnTriggerExit(Collider other) { }
        // ---------------------------------------------------

        protected void HandleTriggerEnter(Collider other)
        {
            if (_isReleased)
                return;

            IEnemy enemy = other.GetComponent<IEnemy>() ?? other.GetComponentInParent<IEnemy>();

            if (enemy != null && enemy.IsAlive)
            {
                enemy.TakeDamage(_damage);
                OnHitEnemy(enemy, other);
                return;
            }

            OnHitNonEnemy(other);
        }

        protected virtual void OnHitEnemy(IEnemy enemy, Collider other)
        {
            Release();
        }

        protected virtual void OnHitNonEnemy(Collider other) { }

        protected void Release()
        {
            if (_isReleased)
                return;

            _isReleased = true;
            OnRelease();
        }

        protected abstract void OnRelease();
    }
}
