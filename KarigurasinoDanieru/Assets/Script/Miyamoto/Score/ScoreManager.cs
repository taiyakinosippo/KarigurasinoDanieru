using UnityEngine;
using System.Collections;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;
    public TextMeshProUGUI score_text;
    private float totalScore = 0;
    private float balanceBarScore = 0;
    private int timingBarScore = 0;
    private int mashButtonScore = 0;

    [SerializeField] private float countUpDuration = 5.0f;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public void MashButtonScore(int baseScore)
    {
        mashButtonScore += baseScore;
        Debug.Log("mashButtonScore:" + mashButtonScore);
    }

    public void TimingBarScore(int amount)
    {
        timingBarScore += amount;
        Debug.Log("timingBarScore:" + timingBarScore);
    }

    public void BalanceBarScore(float meterValue, float baseScore, float multiplier)
    {
        balanceBarScore = meterValue * baseScore * multiplier;

        Debug.Log("balanceBarScore"+ balanceBarScore);
    }

    public float GetScore()
    {
        totalScore = mashButtonScore * timingBarScore * balanceBarScore;
        Debug.Log("score:" + totalScore);
        return totalScore;
    }

    public void StartFinalScorePresentation()
    {
        totalScore = mashButtonScore * timingBarScore * balanceBarScore;

        StartCoroutine(CountUpScoreRoutine());
    }

    private IEnumerator CountUpScoreRoutine()
    {
        float elapsed = 0f;
        float startScore = 0;

        while (elapsed < countUpDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / countUpDuration;

            float currentDisplayScore = Mathf.Lerp(startScore, totalScore, t);

            if (score_text != null)
            {
                float displayScore = currentDisplayScore / 100f;
                score_text.text = displayScore.ToString("N2") + "m";
            }

            yield return null;
        }

        float finalScore = totalScore / 100f;
        score_text.text = finalScore.ToString("N2")+"m";
    }
}
