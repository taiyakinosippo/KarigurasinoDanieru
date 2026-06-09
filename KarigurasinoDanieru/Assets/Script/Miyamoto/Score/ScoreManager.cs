using System;
using UnityEngine;
using YourNamespace;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;        
    [SerializeField] private StageManager stageManager;
    [SerializeField] private BackGroundMover soloBackGroundMover;
    [SerializeField] private BackGroundMover multiBackGroundMover;
    [SerializeField]private ScoreController soloScoreController;
    [SerializeField]private ScoreController multiScoreController;
    [SerializeField] private ScoreDebug scoreDebug;
    [SerializeField] private ScoreSender scoreSender;
    [SerializeField] private VFX_FireController soloFireController;
    [SerializeField] private VFX_FireController multiFireController;

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
            scoreSender = FindFirstObjectByType<ScoreSender>();
        }
    }

    private void Start()
    {
        if(scoreSender != null)
        {
            scoreSender.GetMultiScore += GetMultiScore;
        }
    }
    private void OnDestroy()
    {
        if (scoreSender != null)
        {
            scoreSender.GetMultiScore -= GetMultiScore;
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
    /// <summary>
    /// 相手のスコアを取得する関数
    /// </summary>

    private void GetMultiScore(float totalScore)
    {
        if (scoreSender == null)
        {
            multiTotalScore = 0;
            Debug.Log("Matchstateがありません");
        }
        multiTotalScore = totalScore;
    }

    /// <summary>
    /// 取得した値をほかのスクリプトに渡すための関数
    /// </summary>

    public float MultiResultScore()
    {
        Debug.Log("敵スコア受信:" + totalScore);
        return multiTotalScore;
    }

    public void StartSoloFinalScorePresentation()
    {
        totalScore = SoloResultScore();
        soloScoreController.StartPresentation(totalScore);
        BGM_Manager.Instance.PlayRocketBGM();
        soloBackGroundMover.StartMoving(soloScoreController.CurrentSettings.scrollSpeed, soloScoreController.CurrentSettings.decelerationRate);
        soloFireController.PlayFire();
    }

    public void StartMultiFinalScorePresentation()
    {
        multiTotalScore = MultiResultScore();
        multiScoreController.StartPresentation(multiTotalScore);
        multiBackGroundMover.StartMoving(multiScoreController.CurrentSettings.scrollSpeed, multiScoreController.CurrentSettings.decelerationRate);
        multiFireController.PlayFire();
    }

}
