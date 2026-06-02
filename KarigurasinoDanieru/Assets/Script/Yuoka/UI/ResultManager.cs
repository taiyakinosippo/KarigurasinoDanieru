using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;

public class ResultManager : MonoBehaviour
{
    [Serializable]
    public struct TitleRank
    {
        public string titleName;
        public float minScore;
        public float maxScore;
    }

    [SerializeField] private ScoreController scoreController;
    [SerializeField] private BackGroundMover backgroundMover;

    [Header("リザルトUI(ソロ用)")]
    [SerializeField] private GameObject soloResultObject; // 表示させるリザルトのImageオブジェクト
    [SerializeField] private TextMeshProUGUI soloResultScoreText;
    [SerializeField] private TextMeshProUGUI soloResultTitleText;

    [Header("リザルトUI(マルチ用)")]
    [SerializeField] private GameObject multiResultObject;
    [SerializeField] private TextMeshProUGUI multiResultScoreText;
    [SerializeField] private TextMeshProUGUI multiResultTitleText;

    [Header("アニメーション設定")]
    [SerializeField] private Animator rootAnimator;
    [SerializeField] private string showTriggerName = "Show";

    [SerializeField] private TitleRank[] titleSettings;

    [Header("リザルト表示の待機時間設定")]
    [SerializeField] private float normalDelay = 3f;
    [SerializeField] private float highScoreDelay = 10f;
    private float highScoreThreshold = 100000f;

    private bool isScoreFinished = false;
    private bool isBackgroundFinished = false;
    private bool isMulti;
    private bool isDelaying = false;

    private void Start()
    {
        // 初期状態はどちらも非アクティブ
        if (soloResultObject != null) soloResultObject.SetActive(false);
        if (multiResultObject != null) multiResultObject.SetActive(false);

        // モード判定
        if (GameManager.instance != null)
        {
            isMulti = (GameManager.instance.currentMode == GameMode.Multi);
        }

        // イベント登録
        if (scoreController != null)
        {
            scoreController.OnFinished += OnScorePresentationFinished;
            scoreController.OnEndSpeedEnd += OnBackgroundScrollFinished;
        }
    }

    private void OnDestroy()
    {
        if (scoreController != null)
        {
            scoreController.OnFinished -= OnScorePresentationFinished;
            scoreController.OnEndSpeedEnd -= OnBackgroundScrollFinished;
        }
    }

    private void OnScorePresentationFinished()
    {
        isScoreFinished = true;
        CheckAndShowResult();
    }

    private void OnBackgroundScrollFinished()
    {
        isBackgroundFinished = true;
        CheckAndShowResult();
    }

    private void CheckAndShowResult()
    {
        if (isScoreFinished && isBackgroundFinished && !isDelaying)
        {
            StartCoroutine(DelayShowResultRoutine());
        }
    }

    // スコアに応じて待機時間を変えるコルーチン
    private IEnumerator DelayShowResultRoutine()
    {
        isDelaying = true;

        // デフォルトの待機時間を設定
        float currentDelay = normalDelay;

        // ScoreManagerからスコアを取得して条件分岐
        if (ScoreManager.instance != null)
        {
            float currentScore = ScoreManager.instance.SoloResultScore();

            if (currentScore > highScoreThreshold)
            {
                currentDelay = highScoreDelay;
            }
        }

        // 設定された秒数待機
        yield return new WaitForSeconds(currentDelay);

        // リザルトを表示
        ShowResult();
    }

    private void ShowResult()
    {
        if (ScoreManager.instance == null) return;

        // スコアと称号のデータを取得
        float finalScore = ScoreManager.instance.SoloResultScore();
        string scoreStr = finalScore.ToString("F2") + "m";
        string finalTitle = GetTitle(finalScore);

        // 現在のモードに応じて、捜査対象のUIセットを決定
        GameObject targetObject         = isMulti ? multiResultObject : soloResultObject;
        TextMeshProUGUI targetScoreText = isMulti ? multiResultScoreText : soloResultScoreText;
        TextMeshProUGUI targetTitleText = isMulti ? multiResultTitleText : soloResultTitleText;

        // UIに処理を適用
        if (targetObject    != null) targetObject.SetActive(true);
        if (targetScoreText != null) targetScoreText.text = scoreStr;
        if (targetTitleText != null) targetTitleText.text = finalTitle;

        // アニメーション再生
        if (rootAnimator != null && rootAnimator.gameObject.activeInHierarchy)
        {
            rootAnimator.SetTrigger(showTriggerName);
        }
    }

    private string GetTitle(float score)
    {
        foreach (var setting in titleSettings)
        {
            if (score >= setting.minScore && score <= setting.maxScore)
            {
                return setting.titleName;
            }
        }
        return "判定不能";
    }
}
