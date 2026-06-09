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

    [SerializeField] private ScoreController soloScoreController;     //ソロ用
    [SerializeField] private ScoreController multiScoreController; 　 //マルチ用
    [SerializeField] private BackGroundMover backgroundMover;
    [SerializeField] private ScoreManager scoreManager;

    [Header("リザルトUI(ソロ用)")]
    [SerializeField] private GameObject soloResultObject; // 表示させるリザルトのImageオブジェクト
    [SerializeField] private TextMeshProUGUI soloResultScoreText;
    [SerializeField] private TextMeshProUGUI soloResultTitleText;

    [Header("リザルトUI(マルチ用)")]
    [SerializeField] private GameObject multiResultObject;
    [SerializeField] private TextMeshProUGUI multiResultScoreText;
    [SerializeField] private TextMeshProUGUI multiResultTitleText;
    [SerializeField] private TextMeshProUGUI matchResultText;

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
        if(!isMulti)
        {
            soloScoreController.OnFinished += OnScorePresentationFinished;
            soloScoreController.OnEndSpeedEnd += OnBackgroundScrollFinished;
        }
        else
        {
            multiScoreController.OnFinished += OnScorePresentationFinished;
            multiScoreController.OnEndSpeedEnd += OnBackgroundScrollFinished;
        }
    }

    private void OnDestroy()
    {
        if (soloScoreController != null)
        {
            soloScoreController.OnFinished -= OnScorePresentationFinished;
            soloScoreController.OnEndSpeedEnd -= OnBackgroundScrollFinished;
        }

        if(multiScoreController != null)
        {
            multiScoreController.OnFinished -= OnScorePresentationFinished;
            multiScoreController.OnEndSpeedEnd -= OnBackgroundScrollFinished;
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
        float finalScore = ScoreManager.instance.SoloResultScore();
        if (isScoreFinished && isBackgroundFinished && !isDelaying)
        {
            StartCoroutine(DelayShowResultRoutine());
        }
        else if(isScoreFinished && finalScore == 0 && !isDelaying)
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
        BGM_Manager.Instance.PlayResultBGM();
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

        if (isMulti && matchResultText != null)
        {
            int myScore = Mathf.RoundToInt(finalScore);

            //敵スコア取得（ScoreSenderから）
            ScoreSender sender = FindObjectOfType<ScoreSender>();
            int enemyScore = sender != null ? sender.enemyScore : 0;

            Debug.Log($"🏆 勝敗判定: 自分={myScore} 敵={enemyScore}");

            if (myScore == enemyScore)
            {
                matchResultText.text = "DRAW";
            }
            else if (myScore > enemyScore)
            {
                matchResultText.text = "YOU WIN";
            }
            else
            {
                matchResultText.text = "YOU LOSE";
            }
        }

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
