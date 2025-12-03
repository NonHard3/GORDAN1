using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.UI
{
    [RequireComponent(typeof(Slider))]
    public class ProgressBar : MonoBehaviour
    {
        [SerializeField] private float _fillSpeed;

        private Slider _slider;
        private float _targetValue;

        private void Awake()
        {
            _slider = GetComponent<Slider>();
            Update();
        }

        private void Update()
        {
            if (!Mathf.Approximately(_slider.value, _targetValue))
            {
                _slider.value = Mathf.MoveTowards(
                    _slider.value,
                    _targetValue,
                    _fillSpeed * Time.deltaTime);
            }
        }

        public void UpdateFillSpeed()
        {
            _fillSpeed = (_slider.maxValue - _slider.minValue) / 2;
        }

        /// <summary>
        /// Мгновенно выставить текущий прогресс, без анимации.
        /// </summary>
        public void SetInstant(int current, int max)
        {
            if (max <= 0)
            {
                max = 1;
            }

            _slider.minValue = 0;
            _slider.maxValue = max;

            _targetValue = current;
            _slider.value = current;
        }

        /// <summary>
        /// Плавно анимировать прогресс бар к новому значению.
        /// </summary>
        public void SetAnimated(int current, int max)
        {
            if (max <= 0)
            {
                max = 1;
            }

            _slider.minValue = 0;
            _slider.maxValue = max;

            _targetValue = current;
            // _slider.value не меняем — он дойдёт до target в Update()
        }

#if UNITY_EDITOR
        private void OnGUI()
        {
            GUI.Box(new Rect(20, 50, 200, 25),
                $"Bar: {(int)_slider.value}/{_slider.maxValue} (target: {_targetValue})");
        }
#endif
    }
}
