using UnityEngine;

namespace Project.Gameplay.Abilities
{
    public abstract class ActiveAbilityDefinition : AbilityDefinition
    {
        [Header("Active Ability Settings")]
        [Tooltip("Если true — способность кастуется автоматически по кулдауну.")]
        [SerializeField] private bool autoCast = true;

        public override AbilityKind Kind => AbilityKind.Active;
        public bool AutoCast => autoCast;

        /// <summary>
        /// Логика активации способности.
        /// </summary>
        public abstract void Activate(AbilityRuntimeData runtime, Transform caster);
    }
}