using UnityEngine;
using UnityEngine.UI;

public class RankingInputManager : MonoBehaviour
{
    [Header("UI")]
    public InputField nameInput;
    public InputField scoreInput;

    private LocalRankingManager localRanking;
    private ScoreSender scoreSender;

    void Awake()
    {
        localRanking = FindObjectOfType<LocalRankingManager>();
        scoreSender = FindObjectOfType<ScoreSender>();

        if (localRanking == null)
        {
            Debug.LogError("[RankingInputManager] LocalRankingManager not found");
        }

        if (scoreSender == null)
        {
            Debug.LogError("[RankingInputManager] ScoreSender not found");
        }
    }

    void OnEnable()
    {
        UpdateUI();
    }

    /// <summary>
    /// モードに応じてUI切り替え
    /// </summary>
    void UpdateUI()
    {
        if (nameInput != null)
        {
            // マルチモードでは名前入力を非表示
            nameInput.gameObject.SetActive(!ModeManager.IsMultiMode);
        }
    }

    // ノーマルモード用ボタン
    public void OnClickSendNormal()
    {
        SendScoreInternal("normal");
    }

    // ハードモード用ボタン
    public void OnClickSendHard()
    {
        SendScoreInternal("hard");
    }

    /// <summary>
    /// スコア送信共通処理
    /// </summary>
    private void SendScoreInternal(string mode)
    {
        if (ModeManager.IsMultiMode)
        {
            Debug.Log("[RANKING] Skip send (Multi Mode)");
            return;
        }

        if (nameInput == null || scoreInput == null)
        {
            Debug.LogError("[RANKING] InputField is missing");
            return;
        }

        string name = nameInput.text;
        if (string.IsNullOrEmpty(name))
        {
            Debug.LogWarning("[RANKING] Name is empty");
            return;
        }

        if (!int.TryParse(scoreInput.text, out int score))
        {
            Debug.LogWarning("[RANKING] Score parse failed");
            return;
        }

        if (localRanking == null || scoreSender == null)
        {
            Debug.LogError("[RANKING] Manager reference missing");
            return;
        }

        // ローカルランキング更新
        localRanking.AddOrUpdateScore(name, score, mode);

        // Web送信
        scoreSender.SendScore(name, score, mode);

        // 入力欄クリア
        nameInput.text = "";
        scoreInput.text = "";
    }
}
