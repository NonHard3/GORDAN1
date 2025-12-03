using System;
using UnityEngine;

namespace Project.Gameplay.Abilities
{
    public enum AbilityKind
    {
        Active,
        Passive
    }

    [Serializable]
    public class AbilityLevelData
    {
        [Tooltip("Кулдаун в секундах на этом уровне.")]
        public float cooldown = 1f;

        [Tooltip("Базовый урон способности на этом уровне.")]
        public float damage = 10f;

        [Tooltip("Радиус действия, размер зоны и т.п.")]
        public float radius = 1f;
    }
}