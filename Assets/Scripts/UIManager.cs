using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public Transform scoreDisplay;
    public GameObject moneyPrefab;
    private int score = 0;

    public void AnimateMoney(Vector3 fromPosition)
    {
        GameObject money = Instantiate(moneyPrefab, fromPosition, Quaternion.identity);
        StartCoroutine(MoveMoneyToScore(money));
        score += 1;
        scoreText.text = "Score: " + score;
    }

    System.Collections.IEnumerator MoveMoneyToScore(GameObject money)
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
}
