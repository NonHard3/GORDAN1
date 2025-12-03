using UnityEngine;

namespace Project.Gameplay.Abilities
{
    public sealed class AbilityRuntimeData
    {
        public AbilityDefinition Definition { get; }
        public int Level { get; set; }

        private float _cooldownTimer;

        public AbilityRuntimeData(AbilityDefinition definition)
        {
            Definition = definition;
            Level = 1;
            ResetCooldown();
        }

        public AbilityLevelData CurrentLevelData => Definition.GetLevelData(Level);
        public float CooldownTimer => _cooldownTimer;

        /// <summary>
        /// Обновление кулдауна и автокаст для активных способностей.
        /// </summary>
        public void Tick(float deltaTime, Transform caster)
        {
            if (Definition is not ActiveAbilityDefinition activeDef)
                return;

            if (!activeDef.AutoCast)
                return;

            _cooldownTimer -= deltaTime;
            if (_cooldownTimer <= 0f)
            {
                activeDef.Activate(this, caster);
                ResetCooldown();
            }
        }

        public void ResetCooldown()
        {
            _cooldownTimer = CurrentLevelData.cooldown;
        }
    }
}