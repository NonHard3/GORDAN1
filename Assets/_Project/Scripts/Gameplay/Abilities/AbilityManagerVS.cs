using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Project.Gameplay.Abilities
{
    public sealed class AbilityManagerVS : MonoBehaviour
    {
        [Header("Owner")]
        [Tooltip("Кто кастует способности (обычно объект игрока). Если не задан, берём собственный Transform.")]
        [SerializeField] private Transform owner;

        [Header("Starting Abilities")]
        [SerializeField] private List<ActiveAbilityDefinition> startingActiveAbilities = new();
        [SerializeField] private List<PassiveAbilityDefinition> startingPassiveAbilities = new();

        private readonly Dictionary<string, AbilityRuntimeData> _abilities = new();

        public IReadOnlyDictionary<string, AbilityRuntimeData> Abilities => _abilities;

        public event Action<AbilityRuntimeData>? AbilityLearned;
        public event Action<AbilityRuntimeData>? AbilityUpgraded;
        public event Action<AbilityRuntimeData>? AbilityActivated;

        private void Awake()
        {
            if (owner == null)
                owner = transform;

            // Стартовые способности
            foreach (var def in startingActiveAbilities)
            {
                if (def != null)
                    TryLearnAbility(def);
            }

            foreach (var def in startingPassiveAbilities)
            {
                if (def != null)
                    TryLearnAbility(def);
            }
        }

        private void Update()
        {
            float dt = Time.deltaTime;

            foreach (var ability in _abilities.Values)
            {
                ability.Tick(dt, owner);
            }
        }

        /// <summary>
        /// Получить способность. Если такая уже есть — попытаться апгрейдить.
        /// </summary>
        public bool TryLearnAbility(AbilityDefinition definition)
        {
            if (definition == null)
                return false;

            // Уже есть — пробуем апгрейд
            if (_abilities.TryGetValue(definition.Id, out var runtime))
            {
                return TryUpgradeAbility(definition.Id);
            }

            var newRuntime = new AbilityRuntimeData(definition);
            _abilities.Add(definition.Id, newRuntime);

            // Пассивка — применяем эффекты сразу
            if (definition is PassiveAbilityDefinition passiveDef)
            {
                passiveDef.OnLearn(newRuntime, owner);
            }

            AbilityLearned?.Invoke(newRuntime);
            return true;
        }

        /// <summary>
        /// Повысить уровень способности, если не достигнут MaxLevel.
        /// </summary>
        public bool TryUpgradeAbility(string id)
        {
            if (!_abilities.TryGetValue(id, out var runtime))
                return false;

            if (runtime.Level >= runtime.Definition.MaxLevel)
                return false;

            runtime.Level++;

            if (runtime.Definition is PassiveAbilityDefinition passiveDef)
            {
                passiveDef.OnLevelUp(runtime, owner);
            }

            AbilityUpgraded?.Invoke(runtime);
            return true;
        }

        public bool TryGetAbility(string id, out AbilityRuntimeData runtime)
            => _abilities.TryGetValue(id, out runtime);
    }
}