using UnityEngine;
using _Project.Scripts.Utilities;
using _Project.Scripts.Gameplay.Leveling;

namespace _Project.Scripts.Gameplay.Drops
{
    public class ExperienceParticle : MonoBehaviour
    {
        [Header("Visual float")]
        [SerializeField] private float _baseHeight = 0.1275f;
        [SerializeField] private float _floatSpeed = 2f;
        [SerializeField] private float _floatAmplitude = 0.0725f;

        [Header("Visual config")]
        [SerializeField] private Renderer _renderer; // MeshRenderer/SpriteRenderer
        [SerializeField] private ExperienceVisualConfig _visualConfig;

        private int _expAmount;
        private ObjectPool<ExperienceParticle> _pool;
        private ILevel _levelSystem;

        private bool _isCollected;
        private float _floatOffset;

        public int ExpAmount => _expAmount;

        public void Initialize(ObjectPool<ExperienceParticle> pool, int amount, ILevel levelSystem)
        {
            _pool = pool;
            _expAmount = amount;
            _levelSystem = levelSystem;

            _isCollected = false;

            if (DropAttraction.Instance != null)
            {
                DropAttraction.Instance.RemoveTarget(transform);
            }

            _floatOffset = Random.value * 10f;

            ApplyVisual();
        }

        private void Update()
        {
            float y = Mathf.Sin((Time.time + _floatOffset) * _floatSpeed) * _floatAmplitude;

            var pos = transform.position;
            pos.y = _baseHeight + y;
            transform.position = pos;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("MagnitArea"))
            {
                if (DropAttraction.Instance != null)
                {
                    DropAttraction.Instance.AddTarget(gameObject);
                }
                else
                {
                    Debug.LogWarning("[ExperienceParticle] DropAttraction.Instance is null.");
                }
            }

            if (other.CompareTag("Player"))
            {
                Collect();
            }
        }

        /// <summary>
        /// Применяет материал в зависимости от количества опыта.
        /// </summary>
        private void ApplyVisual()
        {
            if (_renderer == null || _visualConfig == null)
                return;

            var material = _visualConfig.GetMaterialForAmount(_expAmount);
            if (material != null)
            {
                // sharedMaterial — чтобы не дублировать материал на каждый объект
                _renderer.sharedMaterial = material;
            }
        }

        public void Collect()
        {
            if (_isCollected)
                return;

            _isCollected = true;

            if (_levelSystem != null)
            {
                _levelSystem.AddExp(_expAmount);
            }
            else
            {
                Debug.LogWarning("[ExperienceParticle] Level system is null, cannot add exp.");
            }

            Despawn();
        }

        /// <summary>
        /// Базовый метод "исчезновения" частицы (в пул или Destroy).
        /// Используется и при сборе, и при слиянии.
        /// </summary>
        private void Despawn()
        {
            if (DropAttraction.Instance != null)
            {
                DropAttraction.Instance.RemoveTarget(transform);
            }

            if (_pool != null)
            {
                _pool.Return(this);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        // ==========================
        //     ЗАДЕЛ ПОД СЛИЯНИЕ
        // ==========================

        /// <summary>
        /// Текущая частица поглощает другую:
        /// увеличивает свой опыт, обновляет визуал,
        /// а вторую частицу отправляет в пул.
        /// </summary>
        public void Absorb(ExperienceParticle other)
        {
            if (other == null || other == this)
                return;
            if (_isCollected || other._isCollected)
                return;

            _expAmount += other._expAmount;
            other._isCollected = true;
            other.Despawn();

            ApplyVisual();
        }
    }
}