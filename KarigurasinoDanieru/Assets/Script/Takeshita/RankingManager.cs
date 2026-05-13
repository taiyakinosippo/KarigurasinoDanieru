using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class RankingInputManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private InputField nameInput;
    [SerializeField] private InputField scoreInput;
    [SerializeField] private Text resultText;

    private LocalRankingManager localRanking;
    private ScoreSender scoreSender;

    // =====================
    // ランキング取得用データ
    // =====================
    [System.Serializable]
    public class RankingData
    {
        public string name;
        public int score;
    }

    public void Start()
    {
        nameInput.text = "";
        nameInput.characterLimit = 10;
        scoreInput.text = "";
        //scoreInput.characterLimit = 6;
    }

    void Awake()
    {
        nameInput.text = "";
        scoreInput.text = "";

        localRanking = FindObjectOfType<LocalRankingManager>();
        scoreSender = FindObjectOfType<ScoreSender>();

        if (localRanking == null)
            Debug.LogError("[Ranking] LocalRankingManager not found");

        if (scoreSender == null)
            Debug.LogError("[Ranking] ScoreSender not found");

        if (resultText == null)
            Debug.LogError("[Ranking] ResultText is not assigned");

        
    }

    void OnEnable()
    {
        UpdateUI();

        // 前回結果が残らないようにする
        if (resultText != null)
            resultText.gameObject.SetActive(false);
    }

    /// <summary>
    /// モードに応じてUI切り替え
    /// </summary>
    private void UpdateUI()
    {
        if (nameInput != null)
        {
            // マルチでは名前入力は使わない
            nameInput.gameObject.SetActive(!ModeManager.IsMultiMode);
        }
    }

    // =====================
    // ソロ用ランキング送信
    // =====================
    public void OnClickSendScore()
    {
        Debug.Log("bbb");
        // マルチ中は送らない
        if (ModeManager.IsMultiMode) return;

        string name = nameInput.text.Trim();
        if (string.IsNullOrEmpty(name)) return;

        if (!int.TryParse(scoreInput.text, out int score)) return;

        // ★ 難易度は ModeManager から取得
        string mode =
            ModeManager.CurrentDifficulty == SoloDifficulty.Normal
                ? "normal"
                : "hard";

        // ローカル演出（任意）
        localRanking?.AddOrUpdateScore(name, score, mode);

        // DB送信
        scoreSender.SendScore(name, score, mode);

        // 結果表示
        ShowMyRankingResult(name, mode);
    }


    private void SendScoreInternal(string mode)
    {
        if (ModeManager.IsMultiMode) return;

        string name = nameInput.text.Trim();
        if (string.IsNullOrEmpty(name)) return;

        if (!int.TryParse(scoreInput.text, out int score)) return;

        // ローカル演出用
        localRanking?.AddOrUpdateScore(name, score, mode);

        // DB送信
        scoreSender.SendScore(name, score, mode);

        // 結果表示
        ShowMyRankingResult(name, mode);
    }


    // =====================
    // ★ 結果順位表示（ソロ / マルチ共通）
    // =====================
    public void ShowMyRankingResult(string playerName, string mode)
    {
        if (resultText == null) return;
        StartCoroutine(GetRankingAndShowResult(playerName, mode));
    }

    private IEnumerator GetRankingAndShowResult(
     string playerName,
     string mode)
    {
        string url = ServerConfig.BaseUrl + $"get_ranking.php?mode={mode}";

        using (UnityWebRequest req = UnityWebRequest.Get(url))
        {
            yield return req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("[RANKING] Get failed");
                yield break;
            }

            RankingData[] list =
                JsonHelper.FromJson<RankingData>(req.downloadHandler.text);

            // ✅ 入力したスコアを取得
            if (!int.TryParse(scoreInput.text, out int myScore))
                myScore = 0;

            for (int i = 0; i < list.Length; i++)
            {
                // ✅ スコア一致で順位決定
                if (list[i].score == myScore)
                {
                    ShowResultText(playerName, i + 1);
                    yield break;
                }
            }

            // 圏外
            resultText.text = $"{playerName} ランキング圏外";
            resultText.gameObject.SetActive(true);
        }
    }

    private void ShowResultText(string name, int rank)
    {
        if (resultText == null) return;

        if (rank == 1)
        {
            resultText.text = $"{name}　No.1！！";
        }
        else if (rank <= 3)
        {
            resultText.text = $"{name}　No.{rank}！";
        }
        else
        {
            resultText.text = $"{name}　No.{rank}";
        }

        resultText.gameObject.SetActive(true);
    }

    public void SendScoreDuringPlay(int score, string mode)
    {
      
        // マルチでは絶対送らない
        if (ModeManager.IsMultiMode) return;

        if (string.IsNullOrEmpty(nameInput.text))
            return;

        string name = nameInput.text.Trim();

        // ローカル演出ランキング
        if (localRanking != null)
            localRanking.AddOrUpdateScore(name, score, mode);

        // DB送信
        scoreSender.SendScore(name, score, mode);

        Debug.Log("[RANKING] Score sent during play");
    }

    public void SendScore(string name, int score, string mode)
    {
        if (string.IsNullOrEmpty(name))
        {
            Debug.LogWarning("[RANKING] name is empty");
            return;
        }

        Debug.Log($"[RANKING SEND] {name} score={score} mode={mode}");

        // ✅ ローカル演出
        localRanking?.AddOrUpdateScore(name, score, mode);

        // ✅ DB送信（既存の ScoreSender を使う）
        scoreSender.SendScore(name, score, mode);

        // ✅ 結果表示
        ShowMyRankingResult(name, mode);
    }
}
