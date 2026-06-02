using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using TMPro;
using UnityEngine.InputSystem;
using System.Runtime.InteropServices;
using System;

public class ScoreSender : MonoBehaviour
{
    [Header("Debug")]
    public bool isTestMode = false;

    private int lastSentScore = 0;
    private float lastScore = -1f;
    private float stableTimer = 0f;

    private bool alreadyFinished = false;

    [DllImport("__Internal")]
    private static extern void sendMultiScore(string roomId, string playerName, int score, string difficulty);

    private string fetchUrl;
    public int enemyScore = 0;
    public int myScore;

    public Action<int> OnEnemyScoreChanged;
    public Action<int> OnMyScoreChanged;

    void Start()
    {
        fetchUrl = ServerConfig.BaseUrl + "mp_fetch.php";

        var match = FindObjectOfType<MatchState>();

        if (match != null) { match.OnMyNameSet += OnNameReady; }
    }


    void Update()
    {
        if (alreadyFinished) return;

        float current = ScoreManager.instance.SoloResultScore();

        if (current <= 0) return;

       // Debug.Log($"現在TotalScore = {current}");

        // ✅ スコアが変化してるかチェック
        if (Mathf.Abs(current - lastScore) < 0.01f)
        {
            stableTimer += Time.deltaTime;
        }
        else
        {
            stableTimer = 0f;
        }

        lastScore = current;

        // ✅ 0.5秒止まったら終了
        if (stableTimer > 0.5f)
        {
            alreadyFinished = true;

          //  Debug.Log("✅ TotalScore安定＝終了");

            StartCoroutine(SendScoreNextFrame());
        }
    }

    void OnNameReady(string playerName)
    {
        Debug.Log($"🚀 名前確定: {playerName}");

        fetchUrl = ServerConfig.BaseUrl + "mp_fetch.php";

        InvokeRepeating(nameof(FetchEnemyScore), 1f, 1f);
    }

    void FetchEnemyScore()
    {
        if (GameManager.instance.currentMode != GameMode.Multi) return;

        StartCoroutine(FetchEnemyScoreCoroutine());
    }


    IEnumerator FetchEnemyScoreCoroutine()
    {
        string roomId = MainModeManager.CurrentRoomId;

        var match = FindObjectOfType<MatchState>();
        string playerName = match != null ? match.MyName : "";

        // ❗ 自分の名前チェック
        if (string.IsNullOrEmpty(playerName))
        {
            Debug.LogError("❌ 自分の名前が空");
            yield break;
        }

        string url =
            $"{fetchUrl}?room_id={roomId}" +
            $"&difficulty={GameManager.instance.currentLevel}" +
            $"&player_name={playerName}";

        using (UnityWebRequest req = UnityWebRequest.Get(url))
        {
            yield return req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success)
            {
              //  Debug.LogError("[FETCH ERROR] " + req.error);
                yield break;
            }

            PlayerState[] states =
                JsonHelper.FromJson<PlayerState>(req.downloadHandler.text);

            foreach (var ps in states)
            {
                if (ps.player_name == playerName) continue;

                if (enemyScore != ps.score)
                {
                    enemyScore = ps.score;

                    Debug.Log($"👾 敵スコア更新: {enemyScore}");

                    OnEnemyScoreChanged?.Invoke(enemyScore);
                }
            }
        }
    }


    IEnumerator SendScoreNextFrame()
    {
        yield return new WaitForSeconds(0.2f);

        int finalScore = Mathf.RoundToInt(ScoreManager.instance.SoloResultScore());
        myScore = finalScore;
        
        Debug.Log($"自分の最終スコア{myScore}");

        OnMyScoreChanged?.Invoke(myScore);

     //   Debug.Log($"🔥 確定後スコア = {finalScore}");

        var match = FindObjectOfType<MatchState>();
        var multi = FindObjectOfType<MultiSyncManager>();

        //名前取得
        string name = "UNKNOWN";
        if (match != null && !string.IsNullOrEmpty(match.MyName))
        {
            name = match.MyName;
        }

        //モード取得
        string mode = GameManager.instance.currentLevel.ToString().ToLower();

        //RoomID取得
        string roomId = MainModeManager.CurrentRoomId;

     //   Debug.Log($"📦 送信データ room={roomId} name={name} mode={mode}");

        //マルチ送信
        if (multi != null)
        {
            multi.SendScoreIfHigher(finalScore);
        }

        //ランキング送信
        SendScore(name, finalScore, mode);

#if UNITY_WEBGL && !UNITY_EDITOR
    sendMultiScore(roomId, name, finalScore, GameManager.instance.currentLevel.ToString());
#endif
    }

    /// <summary>
    /// スコア送信（外部から呼ぶ用）
    /// </summary>
    public void SendScore(string name, int score, string mode)
    {
      
        if (isTestMode)
        {
            name = "TEST";
          //  Debug.Log("[TEST MODE ENABLED]");
        }

       // Debug.Log(
         //   $"[SEND DEBUG] name='{name}', score={score}, mode='{mode}'"
     //   );

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
               // ScoreView_text.text = $"MyScore{score}";
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

        if (multi != null)
        {
            multi.SendScoreIfHigher(finalScore);
        }

        SendScore(name, finalScore, "normal");

   
    }
}
