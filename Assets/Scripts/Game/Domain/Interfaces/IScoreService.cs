public interface IScoreService
{
    int CurrentScore { get; }
    void AddScore(int amount);
    event System.Action<int> OnScoreChanged;
}