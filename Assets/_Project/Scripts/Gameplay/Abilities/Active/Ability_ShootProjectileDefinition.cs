using UnityEngine;
using Project.Core;                 // ServiceLocator
using Project.Systems.Enemies;      // EnemySearchMode
using Project.Gameplay.Abilities;   // ActiveAbilityDefinition, AbilityRuntimeData, AbilityLevelData

// IEnemy у тебя в глобальном namespace, так что using не нужен

[CreateAssetMenu(
    fileName = "Ability_DirectedToNeareastEnemyShoot",
    menuName = "Scriptable Objects/Abilities/Active/DirectedToNeareastEnemyShoot")]
public class Ability_ShootProjectileDefinition : ActiveAbilityDefinition
{
    [Header("Targeting")]
    [SerializeField] private float searchRadius = 10f;
    [SerializeField] private EnemySearchMode searchMode = EnemySearchMode.Closest;

    [Header("Projectile Settings")]
    [SerializeField] private float projectileSpeed = 20f;
    [SerializeField] private float projectileLifeTime = 3f;

    [Header("Spawn Settings")]
    [Tooltip("Если null — спавним из позиции caster-а")]
    [SerializeField] private Vector3 spawnOffset = Vector3.zero;

    public override void Activate(AbilityRuntimeData runtime, Transform caster)
    {
        if (caster == null)
            return;

        Vector3 casterPos = caster.position;
        Vector3 spawnPos = casterPos + spawnOffset;

        // Пытаемся получить сервис врагов через ServiceLocator
        IEnemy target = null;

        try
        {
            var enemyService = ServiceLocator.Get<IEnemyService>();
            target = enemyService.GetEnemy(casterPos, searchRadius, searchMode);
        }
        catch
        {
            // Если сервиса нет (EnemyManager не на сцене) — просто стреляем вперёд
            Debug.LogWarning("Ability_ShootProjectileDefinition: IEnemyService не найден. Убедись, что EnemyManager есть на сцене.");
        }

        // Определяем направление
        Vector3 direction;

        if (target != null && target.IsAlive)
        {
            direction = (target.Position - spawnPos).normalized;
            direction.y = 0f;
        }
        else
        {
            direction = caster.forward;
            direction.y = 0f;
        }

        if (direction.sqrMagnitude <= 0.0001f)
            direction = caster.forward; // подстраховка

        Quaternion rot = Quaternion.LookRotation(direction, Vector3.up);

        // Берём снаряд из пула
        Projectile proj = ProjectilePool.Instance.Get(spawnPos, rot);
        if (proj == null)
        {
            Debug.LogWarning("Ability_ShootProjectileDefinition: ProjectilePool вернул null.");
            return;
        }

        // Параметры с текущего уровня способности
        AbilityLevelData level = runtime.CurrentLevelData;

        int damage = Mathf.RoundToInt(level.damage);
        float speed = projectileSpeed;
        float lifeTime = projectileLifeTime;

        proj.Initialize(direction, damage, speed, lifeTime);
    }
}