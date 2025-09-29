using UnityEngine;
public class PlayerPrefsScoreRepository
{
    private const string SCORE_KEY = "PlayerScore";
    
    public int LoadScore()
    {
        return PlayerPrefs.GetInt(SCORE_KEY, 0);
    }
    
    public void SaveScore(int score)
    {
        PlayerPrefs.SetInt(SCORE_KEY, score);
        PlayerPrefs.Save();
    }
}