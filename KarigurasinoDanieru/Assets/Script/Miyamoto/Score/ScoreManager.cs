using Mono.Cecil.Cil;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;
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
        UI_Manager.instance.StartCount();
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
    public float UpdatePresentationScore()
    {
        return scoreCalculation.UpdateScore();
    }

    public bool IsPresentationFinished()
    {
        return scoreCalculation.IsFinished();
    }
}
