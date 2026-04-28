using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;
    private int score = 0;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public void BalanceBarScore(float meterValue, int baseScore, float multiplier)
    {
        int calculatedScore = Mathf.RoundToInt(meterValue * baseScore * multiplier);

        AddScore(calculatedScore);

        Debug.Log($"スコア計算：メーター:{meterValue}*ベーススコア:{baseScore}*倍率:{multiplier} 合計:{score}");
    }

    public void AddScore(int amount)
    {
        score += amount;
    }

    public int GetScore()
    {
        return score;
    }
}
