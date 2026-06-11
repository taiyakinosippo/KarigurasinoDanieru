using System;
using UnityEngine;
using System.Collections.Generic;



public class ScoreController : MonoBehaviour
{
    [SerializeField] private StageManager stageManager;

    [Header("Speed Settingsを設定する")]
    [SerializeField] private List<SpeedSettings> settings;

    private SpeedSettings currentSettings;
    public SpeedSettings CurrentSettings
    {
        get
        {
            return currentSettings;
        }
    }
    public float TargetScore
    {
        get;
        private set;
    }

    private float currentScore;
    private float addScore;
    private bool isPlaying;
    [SerializeField] private bool isMultiScoreController = false;  //ソロ用か、マルチ用かの判定
    private IFlightState currentState;

    // ========================================
    // イベントを定義
    // ========================================
    public Action<float> OnScoreChanged;
    public Action<float> OnMultiScoreChanged;

    public Action OnStartSpeedStart;
    public Action OnStartSpeedEnd;

    public Action OnMiddleSpeedStart;
    public Action OnMiddleSpeedEnd;

    public Action OnEndSpeedStart;
    public Action OnEndSpeedEnd;

    public Action OnFinished;

    // ========================================
    // 初期化
    // ========================================

    public void StartPresentation(float targetScore)
    {
        currentScore = 0;
        TargetScore = targetScore;
        FlightState newState = stageManager.GetFlightState(targetScore);
        currentSettings = GetSettings(newState);
        if (currentSettings == null)
        {
            Debug.Log("Settings が見つかりません");
            return;
        }
        if (TargetScore <= 0)
        {
            if (!isMultiScoreController)
            {
                UI_Manager.instance.StartSoloScoreEvent();
            }
            if (GameManager.instance.currentMode == GameMode.Multi && isMultiScoreController)
            {
                UI_Manager.instance.StartMultiScoreEvent();
            }
            ChangeState(new FinishState());
            return;
        }
        isPlaying = true;

        if (!isMultiScoreController)
        {
            UI_Manager.instance.StartSoloScoreEvent();
        }

        else if (GameManager.instance.currentMode == GameMode.Multi && isMultiScoreController)
        {
            UI_Manager.instance.StartMultiScoreEvent();
        }


        ChangeState(new StartSpeedState());
    }

    // ========================================
    // Update
    // ========================================

    private void Update()
    {
        if (!isPlaying)
            return;

        currentState?.Update(this);

        UpdateScore();
    }

    // ========================================
    // スコアの更新
    // ========================================
    private void UpdateScore()
    {
        currentScore +=　addScore * Time.deltaTime;
        if (currentScore >= TargetScore)
        {
            currentScore = TargetScore;
        }
        //if(OnScoreChanged == null)
        //{
        //    Debug.Log("イベントが登録されていません");
        //}

        if (!isMultiScoreController)
        {
            OnScoreChanged?.Invoke(currentScore);
        }

        else if (GameManager.instance.currentMode == GameMode.Multi && isMultiScoreController)
        {
            OnMultiScoreChanged?.Invoke(currentScore);
        }
   
    }

    public void SetScoreSpeed(float speed)
    {
        addScore = speed;
    }

    // ========================================
    // スピードを状態を変更する
    // ========================================

    public void ChangeState(
        IFlightState nextState)
    {
        currentState?.Exit(this);

        currentState = nextState;

        currentState.Enter(this);
    }

    // ========================================
    // 終わったときに呼び出される
    // ========================================

    public void StopPresentation()
    {
        isPlaying = false;
        addScore = 0;
    }

    // ========================================
    // 得たスコアを返す
    // ========================================

    public float GetCurrentScore()
    {
        return currentScore;
    }
    public SpeedSettings GetSettings(FlightState state)
    {
        return settings.Find(x => x.flightState == state);
    }
}
