using System;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Gameplay.Abilities
{
    public abstract class AbilityDefinition : ScriptableObject
    {
        [Header("Base Info")]
        [SerializeField] private string id;
        [SerializeField] private string displayName;
        [TextArea][SerializeField] private string description;
        [SerializeField] private Sprite icon;
        [SerializeField] private int maxLevel = 5;

        [Header("Per-Level Data")]
        [SerializeField] private List<AbilityLevelData> levels = new();

        public string Id => id;
        public string DisplayName => displayName;
        public string Description => description;
        public Sprite Icon => icon;
        public int MaxLevel => maxLevel;

        public abstract AbilityKind Kind { get; }

        public AbilityLevelData GetLevelData(int level)
        {
            if (levels == null || levels.Count == 0)
                throw new InvalidOperationException($"Ability \"{name}\" has no level data.");

            var index = Mathf.Clamp(level - 1, 0, levels.Count - 1);
            return levels[index];
        }
    }
}