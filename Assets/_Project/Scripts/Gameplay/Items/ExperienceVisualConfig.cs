using System;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Drops
{
    [CreateAssetMenu(fileName = "ExperienceVisualConfig", menuName = "Scriptable Objects/Gameplay/Items/Experience Visual Config")]
    public class ExperienceVisualConfig : ScriptableObject
    {
        [Serializable]
        public struct Tier
        {
            public string id;
            public int minAmount;   // Минимальное количество опыта для этого тира
            public Material material;
        }

        [SerializeField] private Tier[] _tiers;

        public Material GetMaterialForAmount(int amount)
        {
            if (_tiers == null || _tiers.Length == 0)
                return null;

            Tier bestTier = _tiers[0];

            for (int i = 0; i < _tiers.Length; i++)
            {
                var tier = _tiers[i];

                // Берём "самый дорогой" тиер, минималка которого <= amount
                if (amount >= tier.minAmount && tier.minAmount >= bestTier.minAmount)
                {
                    bestTier = tier;
                }
            }

            return bestTier.material;
        }
    }
}