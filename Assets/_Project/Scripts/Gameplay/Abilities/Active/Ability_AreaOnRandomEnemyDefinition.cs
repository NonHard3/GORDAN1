using UnityEngine;
using _Project.Core;                 // ServiceLocator
using _Project.Systems.Enemies;      // EnemySearchMode, IEnemyService
using _Project.Gameplay.Abilities;   // ActiveAbilityDefinition, AbilityRuntimeData, AbilityLevelData
using _Project.Gameplay.Attacks;     // AreaAttackInstance (если у тебя другой namespace — поменяй)

// Способность: создать AreaAttack на случайном враге в радиусе
[CreateAssetMenu(
    fileName = "Ability_AreaOnRandomEnemy",
    menuName = "Scriptable Objects/Abilities/Active/Area On Random Enemy")]
public class Ability_AreaOnRandomEnemyDefinition : ActiveAbilityDefinition
{
    [Header("Targeting")]
    [Tooltip("Радиус поиска врага относительно кастера.")]
    [SerializeField] private float searchRadius = 20f;

    [Tooltip("Режим поиска врага. Для случайного выбора поставь Random в инспекторе.")]
    [SerializeField] private EnemySearchMode searchMode = EnemySearchMode.Random;

    [Header("Area Settings (база, можно масштабировать уровнем)")]
    [Tooltip("Базовый радиус области урона (множитель — в level.area, если захочешь).")]
    [SerializeField] private float baseRadius = 3f;

    [Tooltip("Базовая длительность жизни зоны (секунды).")]
    [SerializeField] private float baseLifeTime = 5f;

    [Tooltip("Базовый интервал между тиками урона (секунды).")]
    [SerializeField] private float baseTickInterval = 0.5f;

    [Header("Damage")]
    [Tooltip("Множитель к урону с уровня (runtime.CurrentLevelData.damage * damageMultiplier).")]
    [SerializeField] private float damageMultiplier = 1f;

    public override void Activate(AbilityRuntimeData runtime, Transform caster)
    {
        if (caster == null)
            return;

        // 1. Находим сервис врагов
        IEnemyService enemyService = null;
        try
        {
            enemyService = ServiceLocator.Get<IEnemyService>();
        }
        catch
        {
            Debug.LogWarning(
                "Ability_AreaOnRandomEnemyDefinition: IEnemyService не найден. " +
                "Убедись, что EnemyManager есть на сцене и зарегистрирован в ServiceLocator.");
            return;
        }

        if (enemyService == null)
            return;

        // 2. Берём врага через сервис
        // searchMode в инспекторе можно поставить на Random, тогда будет случайный враг в радиусе.
        Vector3 origin = caster.position;
        IEnemy target = enemyService.GetEnemy(origin, searchRadius, searchMode);

        if (target == null || !target.IsAlive)
        {
            // Врагов нет — способность ничего не делает
            return;
        }

        Vector3 spawnPos = target.Position;
        Quaternion spawnRot = Quaternion.identity;

        // 3. Достаём область из пула
        if (AreaAttackPool.Instance == null)
        {
            Debug.LogError(
                "Ability_AreaOnRandomEnemyDefinition: AreaAttackPool.Instance == null. " +
                "Добавь AreaAttackPool на сцену и задай префаб AreaAttackInstance.");
            return;
        }

        AreaAttackInstance area = AreaAttackPool.Instance.Get(spawnPos, spawnRot);
        if (area == null)
            return;

        // 4. Считаем параметры от уровня способности
        AbilityLevelData level = runtime.CurrentLevelData;

        int damage = Mathf.RoundToInt(level.damage * damageMultiplier);

        float radius = baseRadius;      // при желании можешь умножать на level.area или level.range
        float lifeTime = baseLifeTime;
        float tickInterval = baseTickInterval;

        // 5. Инициализируем область
        area.InitializeArea(
            damage: damage,
            lifeTime: lifeTime,
            tickInterval: tickInterval,
            radius: radius
        );

        // 6. При необходимости — обновляем кулдаун через runtime (если у тебя так заведено)
        // runtime.SetCooldown(level.cooldown); // если есть такое поле
    }
}
