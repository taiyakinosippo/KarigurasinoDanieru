using UnityEngine;

public class ScoreHolder : MonoBehaviour
{
    public static ScoreHolder instance;

    public int FinalScore;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    public void SetScore(float score)
    {
        FinalScore = Mathf.RoundToInt(score);
    }
}
