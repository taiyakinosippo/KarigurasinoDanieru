using Mono.Cecil.Cil;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;
    public TextMeshProUGUI score_text;
    [SerializeField] BackGroundMover backGroundMover;
    [SerializeField]private ScoreCalculation scoreCalculation;
    [field: SerializeField] public float GroundTime { get; private set; } = 5;
    [field: SerializeField] public float SkyTime { get; private set; } = 10;
    [field: SerializeField] public float AtmospheresTime { get; private set; } = 15;
    [field: SerializeField] public float SpaceTime { get; private set; } = 30;
    private float totalScore = 0; 
    private float balanceBarScore = 0;
    private int timingBarScore = 0;
    private int mashButtonScore = 0;
    private float addScore = 0;
    private bool isCounting = false;
  

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }
    private void Update()
    {
        if (!isCounting) return;

        float score = scoreCalculation.UpdateScore();
        score_text.text = score.ToString("N2") + "m";

        if (scoreCalculation.IsFinished())
        {
            score_text.text = GetScore().ToString("N2") + "m";
            backGroundMover.ScrollEnd();
            isCounting = false;
        }
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
        totalScore = (mashButtonScore * timingBarScore * balanceBarScore / 1000);
        Debug.Log("score:" + totalScore);
        return totalScore;
    }

    public void StartFinalScorePresentation()
    {
        totalScore = GetScore();
        addScore = GetArriveTime(totalScore);
        isCounting = true;
    }
    public float GetArriveTime(float score)
    {
        return score switch
        {
            <= 0 => 0,
            <= 1000 => GroundTime,
            <= 10000 => SkyTime,
            <= 100000 =>AtmospheresTime,
            _ =>SpaceTime
        };
    }

}
