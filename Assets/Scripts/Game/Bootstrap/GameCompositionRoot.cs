using UnityEngine;

public class GameCompositionRoot : MonoBehaviour
{
    [SerializeField] private UIManager uiManager;
    
    private IScoreService _scoreService;
    private PlayerPrefsScoreRepository _scoreRepository;

    private void Awake()
    {
        InitializeServices();
        InitializeUI();
    }

    private void InitializeServices()
    {
        _scoreRepository = new PlayerPrefsScoreRepository();
        _scoreService = new ScoreService();
        
        // Load initial score from repository
        int savedScore = _scoreRepository.LoadScore();
        // You'd need to modify ScoreService to support initial value
    }

    private void InitializeUI()
    {
        uiManager.Initialize(_scoreService);
    }

    private void OnApplicationQuit()
    {
        // Save score when application quits
        _scoreRepository.SaveScore(_scoreService.CurrentScore);
    }
}