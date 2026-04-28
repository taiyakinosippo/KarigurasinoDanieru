using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;
    private int score = 0;
    private int balanceBarScore = 0;
    private int timingBarScore = 0;
    private int mashButtonScore = 0;

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

    public void BalanceBarScore(float meterValue, int baseScore, float multiplier)
    {
        int calculatedScore = Mathf.RoundToInt(meterValue * baseScore * multiplier);

        balanceBarScore = calculatedScore;

        Debug.Log($"スコア計算：メーター:{meterValue}*ベーススコア:{baseScore}*倍率:{multiplier} 合計:{meterValue* baseScore* multiplier}");
    }

    public int GetScore()
    {
        score = mashButtonScore * timingBarScore * balanceBarScore;
        Debug.Log("score:" + score);
        return score;
    }
}
