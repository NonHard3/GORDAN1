namespace _Project.Gameplay.Progression
{
    public interface ILevel
    {
        void AddExp(int amount);

        int CurrentLevel { get; }
        int CurrentExp { get; }
        int ExpToNextLevel { get; }
    }
}
