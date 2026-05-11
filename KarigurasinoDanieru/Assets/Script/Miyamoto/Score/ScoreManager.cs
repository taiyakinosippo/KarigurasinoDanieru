//using Mono.Cecil.Cil;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;
    public TextMeshProUGUI score_text;
    [SerializeField] BackGroundMover backGroundMover;
    [SerializeField] private int Groudspeed = 10;        //�n��ł̉�����
    [SerializeField] private int SkySpeed = 100;         //��ł̉�����
    [SerializeField] private int AtmospheresSeed = 1000; //��C���ł̉�����
    [SerializeField] private int SpaceSpeed = 10000;     //�F���ł̉�����
    private float totalScore = 0;
    private float balanceBarScore = 0;
    private int timingBarScore = 0;
    private int mashButtonScore = 0;
    private float addScore = 0;
    private float startScore = 0;
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

        startScore += addScore * Time.deltaTime;

        if (startScore >= totalScore)
        {
            startScore = totalScore;

            isCounting = false;

            score_text.text = totalScore.ToString("N2") + "m";

            backGroundMover.ScrollEnd();

            return;
        }
        score_text.text = startScore.ToString("N2") + "m";
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
    public float GetCurrentScore()
    {
        return startScore;
    }

    public float GetScore()
    {
        totalScore = (mashButtonScore * timingBarScore * balanceBarScore / 1000);
        Debug.Log("score:" + totalScore);
        return totalScore;
    }

    public void StartFinalScorePresentation()
    {
        totalScore = (mashButtonScore * timingBarScore * balanceBarScore /1000);
        addScore = GetAddScore(totalScore);
        isCounting = true;
    }
    public float GetAddScore(float score)
    {
        return score switch
        {
            <= 0 => 0,
            <= 1000 =>
                Mathf.Floor(score / 100f) * Groudspeed,
            <= 10000 =>
                Mathf.Floor(score / 1000f) * SkySpeed,
            <= 100000 =>
               Mathf.Floor(score / 10000f) * AtmospheresSeed,
            _ =>
                Mathf.Floor(score / 100000f) * SpaceSpeed
        };
    }

}
