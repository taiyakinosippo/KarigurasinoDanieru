using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ScoreSender : MonoBehaviour
{
    [Header("Debug")]
    public bool isTestMode = false; // ✅ Inspectorで切り替え

    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
    /// <summary>
    /// スコア送信（外部から呼ぶ用）
    /// </summary>
    public void SendScore(string name, int score, string mode)
    {
        // ✅ テストモードなら名前を強制変更
        if (isTestMode)
        {
            name = "TEST";
            Debug.Log("[TEST MODE ENABLED]");
        }

        // ✅ デバッグ表示
        Debug.Log(
            $"[SEND DEBUG] name='{name}', score={score}, mode='{mode}'"
        );

        // ✅ 送信開始
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

    // =====================
    // ✅ デバッグ用（ボタンで直接送れる）
    // =====================
    public void SendTestScoreButton()
    {
        int score = ScoreHolder.instance != null
            ? ScoreHolder.instance.FinalScore
            : 9999;

        SendScore("DEBUG", score, "normal");
    }
}
