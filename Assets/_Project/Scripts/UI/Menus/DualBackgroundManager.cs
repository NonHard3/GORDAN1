using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Systems.UI
{
    /// <summary>
    /// Менеджер двойного фона: A анимируется, B всегда видим.
    /// BackgroundB меняет изображение только тогда, когда BackgroundA полностью виден.
    /// </summary>
    public class DualBackgroundManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Image backgroundA; // передний план — исчезает/появляется
        [SerializeField] private Image backgroundB; // задний план — всегда виден

        [Header("Settings")]
        [SerializeField] private List<Sprite> backgrounds;
        [SerializeField] private float fadeDuration = 1f;     // скорость фейда
        [SerializeField] private float changeInterval = 5f;   // интервал между циклами

        private int currentIndexA;
        private int currentIndexB;
        private float timer;
        private bool isFading;

        private void Start()
        {
            if (backgrounds.Count < 2)
            {
                Debug.LogWarning("[DualBackgroundManager] Нужно минимум два изображения!");
                return;
            }

            // Стартовые значения
            currentIndexA = 0;
            currentIndexB = 1;

            backgroundA.sprite = backgrounds[currentIndexA];
            backgroundB.sprite = backgrounds[currentIndexB];

            backgroundA.color = Color.white;
            backgroundB.color = Color.white;
        }

        private void Update()
        {
            if (isFading || backgrounds.Count < 2)
                return;

            timer += Time.deltaTime;
            if (timer >= changeInterval)
            {
                timer = 0f;
                StartCoroutine(FadeCycle());
            }
        }

        private IEnumerator FadeCycle()
        {
            isFading = true;

            // 1️ Плавное исчезновение A
            for (float t = 0; t < fadeDuration; t += Time.deltaTime)
            {
                float alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);
                backgroundA.color = new Color(1f, 1f, 1f, alpha);
                yield return null;
            }
            backgroundA.color = new Color(1f, 1f, 1f, 0f);

            // 2️ Когда A полностью исчез — меняем его изображение
            int nextA = GetNextIndex(currentIndexA, currentIndexB);
            backgroundA.sprite = backgrounds[nextA];
            currentIndexA = nextA;

            yield return new WaitForSeconds(changeInterval);

            // 3️ Плавное появление A
            for (float t = 0; t < fadeDuration; t += Time.deltaTime)
            {
                float alpha = Mathf.Lerp(0f, 1f, t / fadeDuration);
                backgroundA.color = new Color(1f, 1f, 1f, alpha);
                yield return null;
            }
            backgroundA.color = Color.white;

            // 4️ Когда A полностью виден — меняем изображение B
            int nextB = GetNextIndex(currentIndexB, currentIndexA);
            backgroundB.sprite = backgrounds[nextB];
            currentIndexB = nextB;

            isFading = false;
        }

        private int GetNextIndex(int current, int other)
        {
            int count = backgrounds.Count;

            // Если фонов вообще нет
            if (count == 0)
                return -1; // можно вернуть -1 как "ничего нет"

            // Если один фон — возвращаем единственный индекс
            if (count == 1)
                return 0;

            // Если 2 или меньше — просто чередуем
            if (count == 2)
                return (current + 1) % count;

            // --- Линейный перебор по порядку ---
            int next = (current + 1) % count; // следующий по кругу

            // Пропускаем current и other, если попались
            while (next == current || next == other)
            {
                next = (next + 1) % count;
            }

            return next;
        }
    }
}
