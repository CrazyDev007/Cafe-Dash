using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private Transform scoreDisplay;
    [SerializeField] private GameObject moneyPrefab;
    
    private IScoreService _scoreService;

    public void Initialize(IScoreService scoreService)
    {
        _scoreService = scoreService;
        _scoreService.OnScoreChanged += UpdateScoreDisplay;
        UpdateScoreDisplay(_scoreService.CurrentScore);
    }

    public void AnimateMoney(Vector3 fromPosition)
    {
        GameObject money = Instantiate(moneyPrefab, fromPosition, Quaternion.identity);
        StartCoroutine(MoveMoneyToScore(money));
        _scoreService.AddScore(1);
    }

    private void UpdateScoreDisplay(int score)
    {
        scoreText.text = $"Score: {score}";
    }

    private System.Collections.IEnumerator MoveMoneyToScore(GameObject money)
    {
        float t = 0;
        Vector3 start = money.transform.position;
        Vector3 end = scoreDisplay.position;
        
        while (t < 1f)
        {
            money.transform.position = Vector3.Lerp(start, end, t);
            t += Time.deltaTime * 2f;
            yield return null;
        }
        Destroy(money);
    }

    private void OnDestroy()
    {
        if (_scoreService != null)
        {
            _scoreService.OnScoreChanged -= UpdateScoreDisplay;
        }
    }
}