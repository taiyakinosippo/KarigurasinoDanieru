using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using TMPro;

public class ScoreSender : MonoBehaviour
{
    [Header("Debug")]
    public bool isTestMode = false;

    public TextMeshProUGUI ScoreView_text;
    private int lastSentScore = 0;

    void Start()
    {
        var controller = FindObjectOfType<ScoreController>();

        if (controller != null)
        {
            controller.OnFinished += OnScoreFinishedSend;
        }
    }

    /// <summary>
    /// スコア送信（外部から呼ぶ用）
    /// </summary>
    public void SendScore(string name, int score, string mode)
    {
      
        if (isTestMode)
        {
            name = "TEST";
            Debug.Log("[TEST MODE ENABLED]");
        }

        Debug.Log(
            $"[SEND DEBUG] name='{name}', score={score}, mode='{mode}'"
        );

        StartCoroutine(PostScoreCoroutine(name, score, mode));
    }

    /// <summary>
    /// PHPへPOST送信
    /// </summary>
    private IEnumerator PostScoreCoroutine(string name, int score, string mode)
    {
        WWWForm form = new WWWForm();
        form.AddField("name", name);
        form.AddField("score", score);
        form.AddField("mode", mode);

        string url = ServerConfig.BaseUrl + "save_score.php";

        using (UnityWebRequest req = UnityWebRequest.Post(url, form))
        {
            req.timeout = 10;

            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("✅ スコア送信成功: " + req.downloadHandler.text);
                ScoreView_text.text = $"MyScore{score}";
            }
            else
            {
                Debug.LogError(
                    $"❌ スコア送信失敗\n" +
                    $"URL: {url}\n" +
                    $"Error: {req.error}\n" +
                    $"Response: {req.downloadHandler.text}"
                );
            }
        }
    }

    void OnScoreFinishedSend()
    {
        int finalScore = Mathf.RoundToInt(ScoreManager.instance.SoloResultScore());

        var match = FindObjectOfType<MatchState>();
        var multi = FindObjectOfType<MultiSyncManager>();

        string name = "SINGLE";

        if (GameManager.instance.currentMode == GameMode.Multi && match != null)
        {
            name = match.MyName;
        }

        string mode = GameManager.instance.currentLevel.ToString().ToLower();

        // ✅ マルチ用同期
        if (multi != null)
        {
            multi.SendScoreIfHigher(finalScore);
        }

        // ✅ ランキング送信（ここが本命）
        SendScore(name, finalScore, mode);

       
    }
}
