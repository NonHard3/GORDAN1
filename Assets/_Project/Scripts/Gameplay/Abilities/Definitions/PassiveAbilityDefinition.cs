using UnityEngine;

namespace _Project.Gameplay.Abilities
{

    public abstract class PassiveAbilityDefinition : AbilityDefinition
    {
        public override AbilityKind Kind => AbilityKind.Passive;

        /// <summary>Вызывается при первом получении способности.</summary>
        public virtual void OnLearn(AbilityRuntimeData runtime, Transform owner) { }

        /// <summary>Вызывается при повышении уровня способности.</summary>
        public virtual void OnLevelUp(AbilityRuntimeData runtime, Transform owner) { }
    }
}