using UnityEngine;
using _Project.Scripts.Utilities;
using _Project.Scripts.Gameplay.Drops;
using _Project.Scripts.Gameplay.Leveling;

public class ExperienceSpawner : MonoBehaviour
{
    [SerializeField] private ExperienceParticle _expPrefab;
    [SerializeField] private LevelingSystem _levelsystem;
    [SerializeField] private int _initialPoolSize = 20;
    [SerializeField] private int _maxPoolSize = 100;

    public static ExperienceSpawner Instance { get; private set; }
    private ILevel levelSystem;
    private ObjectPool<ExperienceParticle> _expPool;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        levelSystem = _levelsystem.GetComponent<LevelingSystem>();
        Instance = this;
    }

    private void Start()
    {
        _expPool = new ObjectPool<ExperienceParticle>(_expPrefab, _initialPoolSize, transform, _maxPoolSize);
    }

    /// <summary>
    /// Спавнит одну частицу опыта в позиции targetPosition
    /// amount – сколько опыта эта частица даёт
    /// </summary>
    public void SpawnExp(Vector3 targetPosition, int amount)
    {
        ExperienceParticle exp = _expPool.Get();
        if (exp == null)
            return;

        exp.transform.position = targetPosition;
        exp.Initialize(_expPool, amount, levelSystem);
    }
}
