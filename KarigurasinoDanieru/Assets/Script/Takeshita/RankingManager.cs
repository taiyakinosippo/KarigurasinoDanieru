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

    void Awake()
    {
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
    public void OnClickSendNormal()
    {
        SendScoreInternal("normal");
    }

    public void OnClickSendHard()
    {
        SendScoreInternal("hard");
    }

    private void SendScoreInternal(string mode)
    {
        // マルチ中はここでは送らない（終了時に送る）
        if (ModeManager.IsMultiMode)
        {
            Debug.Log("[RANKING] Skip send (Multi Mode)");
            return;
        }

        if (nameInput == null || scoreInput == null)
            return;

        string name = nameInput.text.Trim();
        if (string.IsNullOrEmpty(name))
            return;

        if (!int.TryParse(scoreInput.text, out int score))
            return;

        // ローカル順位（演出用）
        if (localRanking != null)
            localRanking.AddOrUpdateScore(name, score, mode);

        // DB保存
        scoreSender.SendScore(name, score, mode);

        // DB保存後に順位表示
        ShowMyRankingResult(name, mode);

        nameInput.text = "";
        scoreInput.text = "";
    }

    // =====================
    // ★ 結果順位表示（ソロ / マルチ共通）
    // =====================
    public void ShowMyRankingResult(string playerName, string mode)
    {
        if (resultText == null) return;
        StartCoroutine(GetRankingAndShowResult(playerName, mode));
    }

    private IEnumerator GetRankingAndShowResult(string playerName, string mode)
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

            for (int i = 0; i < list.Length; i++)
            {
                // NOTE: 名前重複時は最初に見つかった順位を使用
                if (list[i].name == playerName)
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
}
