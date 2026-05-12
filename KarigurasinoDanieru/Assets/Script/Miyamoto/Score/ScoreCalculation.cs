using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class ScoreCalculation : MonoBehaviour
{
    [SerializeField] private ScoreManager scoreManager;  //ステージの情報を保持している
    [SerializeField] private BackGroundMover backGroundMover;  //背景を動かすスクリプト
    [SerializeField] private float LastGroundSpeed = 10f;    //地面の時間
    [SerializeField] private float LastSkySpeed = 100f;    //空の時間
    [SerializeField] private float LastAtmospheresSpeed = 200f;    //大気圏の時間
    [SerializeField] private float LastSpaceSpeed = 500f;    //宇宙の時間
    public bool isLastSecond { get; private set; } = false;
    private float arriveTime = 0;    //ゴールまでの時間
    private float lastSpeed = 0;  //最後のスピード
    private float currentScore = 0;
    private float addScore = 0;
    private float targetScore = 0;
    private float remainTime = 0;
    private float elapsedTime = 0;

    private bool isPlaying;

    public void StartScore(float totalScore)
    {
        targetScore = totalScore;
        arriveTime = scoreManager.GetArriveTime(totalScore);
        lastSpeed = LastSpeed(arriveTime);
        addScore = targetScore / arriveTime;
        isPlaying = true;
        backGroundMover.StartMoving();
    }
    public float LastSpeed(float time)
    {
        if (time <= 0)
            return 0;

        if (time <= scoreManager.GroundTime)
            return LastGroundSpeed;

        if (time <= scoreManager.SkyTime)
            return LastSkySpeed;

        if (time <= scoreManager.AtmospheresTime)
            return LastAtmospheresSpeed;

        return LastSpaceSpeed;
    }

    public float UpdateScore()
    {
        if (!isPlaying) return currentScore;
        elapsedTime += Time.deltaTime;
        remainTime = arriveTime - elapsedTime;
        if (remainTime <= 1f)
        {
            addScore = lastSpeed;
            isLastSecond = true;
        }
        if (currentScore >= targetScore)
        {
            currentScore = targetScore;
            isPlaying = false;
        }
        currentScore += addScore * Time.deltaTime;
        return currentScore;
    }

    public float GetScoreSpeed()
    {
        return addScore;
    }

    public bool IsFinished()
    {
        return !isPlaying;
    }
}
