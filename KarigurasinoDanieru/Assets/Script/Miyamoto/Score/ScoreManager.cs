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

    public void AddScore(int amount)
    {
        score += amount;
    }

    public int GetScore()
    {
        return score;
    }
}
