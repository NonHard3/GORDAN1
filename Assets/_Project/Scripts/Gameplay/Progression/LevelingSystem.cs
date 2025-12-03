using UnityEngine;
using _Project.UI;
// using _Project.Core; // если захочешь внедрять через ServiceLocator

namespace _Project.Gameplay.Progression
{
    public class LevelingSystem : MonoBehaviour, ILevel
    {
        [Header("Base settings")]
        [SerializeField] private int _startLevel = 1;
        [SerializeField] private int _startExpToNextLevel = 5;
        [SerializeField] private float _levelGrowthFactor = 1.2f;

        [Header("UI")]
        [SerializeField] private ProgressBar _expBar;

        public int CurrentLevel => _currentLevel;
        public int CurrentExp => _currentExpInLevel;
        public int ExpToNextLevel => _expToNextLevel;

        private int _currentLevel;
        private int _currentExpInLevel;
        private int _expToNextLevel;

        private void Awake()
        {
            _currentLevel = _startLevel;
            _currentExpInLevel = 0;
            _expToNextLevel = _startExpToNextLevel;

            if (_expBar == null)
            {
                Debug.LogWarning("[LevelingSystem] ExpBar is not assigned in inspector.");
            }
        }

        private void Start()
        {
            UpdateExpUI();
        }

        public void AddExp(int amount)
        {
            if (amount <= 0)
                return;

            _currentExpInLevel += amount;

            // Можем апнуть сразу несколько уровней, если опыта очень много
            while (_currentExpInLevel >= _expToNextLevel)
            {
                _currentExpInLevel -= _expToNextLevel;
                LevelUp();
                _expBar.UpdateFillSpeed();
            }

            _expBar.SetAnimated(_currentExpInLevel, _expToNextLevel);
        }

        private void LevelUp()
        {
            _currentLevel++;

            // Простейшая формула роста стоимости уровня
            _expToNextLevel = Mathf.CeilToInt(_expToNextLevel * _levelGrowthFactor);

            UpdateExpUI();
            Debug.Log($"[LevelingSystem] Level up: {_currentLevel}. Exp to next: {_expToNextLevel}");
        }

        private void UpdateExpUI()
        {
            if (_expBar == null)
                return;

            _expBar.SetInstant(_currentExpInLevel, _expToNextLevel);
        }

#if UNITY_EDITOR
        private void OnGUI()
        {
            GUI.Box(new Rect(20, 20, 200, 25),
                $"Level: {CurrentLevel} | Exp: {CurrentExp}/{ExpToNextLevel}");
        }
#endif
    }
}
