using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;
    [SerializeField] private StageManager stageManager;
    [SerializeField] private BackGroundMover backGroundMover;
    [SerializeField] private ScoreController scoreController;
    private float totalScore = 0;
    private float balanceBarScore = 0;
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

    public void BalanceBarScore(float meterValue, float baseScore, float multiplier)
    {
        balanceBarScore = meterValue * baseScore * multiplier;

        Debug.Log("balanceBarScore" + balanceBarScore);
    }

    public float GetScore()
    {
        totalScore = (mashButtonScore * timingBarScore * balanceBarScore / 1000);
        Debug.Log("score:" + totalScore);
        return totalScore;
    }

    public void StartFinalScorePresentation()
    {
        totalScore = GetScore();
        scoreController.StartPresentation(totalScore);
        backGroundMover.StartMoving(scoreController.CurrentSettings.scrollSpeed, scoreController.CurrentSettings.decelerationRate);
    }
}
