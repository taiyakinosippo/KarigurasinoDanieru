using UnityEngine;

public class MatchScoreManager: MonoBehaviour
{

    public int CurrentScore { get; private set; }

    public void AddScore(int value)
    {
        CurrentScore += value;
    }

    public void ResetScore()
    {
        CurrentScore = 0;
    }

    public void SetScore(int value)
    {
        CurrentScore = value;
    }
}
