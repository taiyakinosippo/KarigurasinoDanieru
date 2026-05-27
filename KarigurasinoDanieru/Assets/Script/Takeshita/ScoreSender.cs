using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ScoreSender : MonoBehaviour
{
    [Header("Debug")]
    public bool isTestMode = false; // ✅ Inspectorで切り替え

 
    void Start()
    {
       
        }
    /// <summary>
    /// スコア送信（外部から呼ぶ用）
    /// </summary>
    public void SendScore(string name, int score, string mode)
    {
        Debug.Log($"[SCORE INPUT] name={name}");

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

    public void SendTestScoreButton()
    {
        // ✅ 正しいスコア取得
        int score = Mathf.RoundToInt(ScoreManager.instance.GetScore());

        // ✅ マルチ同期
        var match = FindObjectOfType<MatchState>();
        var multi = FindObjectOfType<MultiSyncManager>();

        if (match != null)
        {
            match.SetMyScore(score);
        }

        if (multi != null)
        {
            multi.SendScoreIfHigher(score);
            Debug.Log($"[ENEMY DEBUG] Name={multi.opponentName}, Score={multi.opponentScore}");
        }

        Debug.Log($"[REAL SCORE SEND] {score}");

        // ✅ 名前取得（安全版）
        string name = "UNKNOWN";

        if (GameManager.instance.currentMode == GameMode.Multi)
        {
            if (match != null && !string.IsNullOrEmpty(match.MyName))
            {
                name = match.MyName;
            }
        }
        else
        {
            name = "SINGLE_PLAYER";
        }

        // ✅ モード取得（安全化）
        string mode = GameManager.instance.currentLevel.ToString().ToLower();
        if (string.IsNullOrEmpty(mode))
        {
            mode = "normal";
        }

        Debug.Log($"[FINAL SEND DATA] name={name}, score={score}, mode={mode}");

        SendScore(name, score, mode);
    }
}
