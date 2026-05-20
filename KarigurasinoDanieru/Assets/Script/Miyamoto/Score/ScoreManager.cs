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
        Debug.Log("ScoreManager instance is set.");
    }


    public void MashButtonScore(int baseScore)
    {
        mashButtonScore = baseScore;
        Debug.Log("mashButtonScore:" + mashButtonScore);
    }

    public void TimingBarScore(int amount)
    {
        timingBarScore += amount;
    }

    public void BalanceBarScore(float meterValue, float baseScore, float multiplier)
    {
        balanceBarScore = meterValue * baseScore * multiplier;
    }

    public float GetScore()
    {
        Debug.Log("mashButtonScore:" + mashButtonScore);
        Debug.Log("timingBarScore:" + timingBarScore);
        Debug.Log("balanceBarScore" + balanceBarScore);
        totalScore = (mashButtonScore * timingBarScore * balanceBarScore / 1000);
        Debug.Log("score:" + totalScore);
        return totalScore;
    }

    public void StartFinalScorePresentation()
    {
        totalScore = GetScore();
        scoreController.StartPresentation(totalScore);
        if(scoreController.CurrentSettings == null)
        {
            Debug.Log("CurrentSettings ‚ªŒ©‚Â‚©‚è‚Ü‚¹‚ñ");
            return;
        }
        backGroundMover.StartMoving(scoreController.CurrentSettings.scrollSpeed, scoreController.CurrentSettings.decelerationRate);
    }
}
