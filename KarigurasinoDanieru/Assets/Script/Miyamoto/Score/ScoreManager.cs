using UnityEngine;
using System;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;        
    [SerializeField] private StageManager stageManager;
    [SerializeField] private BackGroundMover soloBackGroundMover;
    [SerializeField] private BackGroundMover multiBackGroundMover;
    [SerializeField]private ScoreController soloScoreController;
    [SerializeField]private ScoreController multiScoreController;
    [SerializeField] private ScoreDebug scoreDebug;
    [SerializeField] private MatchState matchState;
    private float totalScore = 0;
    private float multiTotalScore = 0;
    private float balanceBarScore = 0;
    private int timingBarScore = 0;
    private int mashButtonScore = 0;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        if (GameManager.instance.currentMode == GameMode.Multi)
        {
            matchState = FindFirstObjectByType<MatchState>();
        }
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

    public float SoloResultScore()
    {
        //if (scoreDebug.useDebugScore)
        //{
        //    Debug.Log("デバッグスコア使用: " + scoreDebug.debugTotalScore);
        //    return scoreDebug.debugTotalScore;
        //}
        Debug.Log("mashButtonScore:" + mashButtonScore);
        Debug.Log("timingBarScore:" + timingBarScore);
        Debug.Log("balanceBarScore" + balanceBarScore);
        totalScore = (mashButtonScore * timingBarScore * balanceBarScore / 1000);
        Debug.Log("score:" + totalScore);
        return totalScore;
    }
    public float MultiResultScore()
    {
        if (matchState != null)
        {
            multiTotalScore = 0;
            Debug.Log("Matchstateがありません");
            return 0;
        }
        multiTotalScore = matchState.EnemyScore;
        return multiTotalScore;
    }
    public void StartSoloFinalScorePresentation()
    {
        totalScore = SoloResultScore();
        soloScoreController.StartPresentation(totalScore);
        BGM_Manager.Instance.PlayRocketBGM();
        soloBackGroundMover.StartMoving(soloScoreController.CurrentSettings.scrollSpeed, soloScoreController.CurrentSettings.decelerationRate);
    }

    public void StartMultiFinalScorePresentation()
    {
        multiTotalScore = MultiResultScore();
        multiScoreController.StartPresentation(multiTotalScore);
        multiBackGroundMover.StartMoving(multiScoreController.CurrentSettings.scrollSpeed, multiScoreController.CurrentSettings.decelerationRate);
    }

}
