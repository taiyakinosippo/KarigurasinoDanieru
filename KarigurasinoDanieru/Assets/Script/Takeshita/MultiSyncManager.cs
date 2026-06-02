using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

public class MultiSyncManager : MonoBehaviour
{
    [Header("Server URLs")]
    private string updateUrl;
    private string fetchUrl;
    private string joinUrl;
   
    [Header("Player Info")]
    public string playerName;
    public int currentScore;

    public System.Action OnScoreSent;

    // 内部状態
    private string roomId;
    private bool matched;
    private bool joined;
    private bool isFetching;
    private bool isSending;

    [SerializeField] private MatchState matchState;
    [SerializeField] private MainModeManager MainmodeManager;

    public string opponentName = "";
    public int opponentScore = 0;
    private int lastEnemyScore = -1;
    private int lastSentScore = 0;

    private bool enemyPreviouslyPresent = false;

    public Action<int> OnEnemyScoreChanged;

    void Awake()
    {
        updateUrl = ServerConfig.BaseUrl + "mp_update.php";
        fetchUrl = ServerConfig.BaseUrl + "mp_fetch.php";
        joinUrl = ServerConfig.BaseUrl + "mp_join.php";
       
        MainmodeManager = FindObjectOfType<MainModeManager>();
    }

    void Start()
    {
        // 明示的に停止状態で開始
        StopAllSync();
    }

    void Update()
    {
        if (matchState == null)
        {
            matchState = FindObjectOfType<MatchState>();
        }

        if (MainmodeManager == null)
        {
            MainmodeManager = FindObjectOfType<MainModeManager>();
        }
    }

    /* ======================
       マルチ開始
    ====================== */
    public void BeginMultiSync()
    {
        DontDestroyOnLoad(gameObject);
        StartCoroutine(BeginMultiSyncCoroutine());
    }

    IEnumerator BeginMultiSyncCoroutine()
    {
        enabled = true;

        roomId = MainModeManager.CurrentRoomId;

        // ✅ 名前がセットされるまで待つ
        while (matchState == null || string.IsNullOrEmpty(matchState.MyName))
        {
            matchState = FindObjectOfType<MatchState>();
            yield return null;
        }

        playerName = matchState.MyName;

       // Debug.Log($"[SYNC START] player={playerName}");

        if (string.IsNullOrEmpty(roomId))
        {
         //   Debug.LogError("[SYNC INIT ERROR] roomId NULL!");
            yield break;
        }

        matched = false;
        lastEnemyScore = -1;

        SendJoinOnce();
        currentScore = matchState.MyScore;

        SendState();
        InvokeRepeating(nameof(FetchState), 0.5f, 0.5f);
    }


    /* ======================
       完全停止
    ====================== */
    private void StopAllSync()
    {
        CancelInvoke();
        StopAllCoroutines();
        enabled = false;
    }

   
    IEnumerator SendStateCoroutine()
    {
       
        isSending = true;

        WWWForm form = new WWWForm();
        form.AddField("room_id", roomId);
        form.AddField("player_name", playerName);
        form.AddField("score", currentScore);
        form.AddField("difficulty", GameManager.instance.currentLevel.ToString());

        using (UnityWebRequest req = UnityWebRequest.Post(updateUrl, form))
        {
            req.timeout = 10;
            yield return req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success)
            {
              //  Debug.LogError("[MultiSync] Send error: " + req.error);
            }
        }
       // Debug.Log($"[SEND] room={roomId}, name={playerName}, score={currentScore}");

        isSending = false;
    }

    /* ======================
       状態取得
    ====================== */
    private void FetchState()
    {
        //Debug.Log("[FetchState] called");
        if (isFetching) return;
        StartCoroutine(FetchStateCoroutine());
    }

    IEnumerator FetchStateCoroutine()
    {
        isFetching = true;

      
        string url =
            $"{fetchUrl}?room_id={roomId}" +
            $"&difficulty={GameManager.instance.currentLevel}" +
            $"&player_name={playerName}";

        using (UnityWebRequest req = UnityWebRequest.Get(url))
        {
            req.timeout = 10;
            yield return req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(
                    "[FETCH FAILED]\n" +
                    "url: " + url + "\n" +
                    "result: " + req.result + "\n" +
                    "error: " + req.error
                );
                isFetching = false;
                yield break;
            }

           // Debug.Log("[RAW JSON] " + req.downloadHandler.text);

            if (string.IsNullOrEmpty(req.downloadHandler.text))
            {
               // Debug.LogWarning("[FETCH] Empty response");
                isFetching = false;
                yield break;
            }

            PlayerState[] states =
                JsonHelper.FromJson<PlayerState>(req.downloadHandler.text);

          //  Debug.Log("===== PLAYER LIST =====");

            foreach (var ps in states)
            {
                if (ps == null) continue;

              //  Debug.Log(
                //    $"name={ps.player_name}, score={ps.score}" +
               //     (ps.player_name == playerName ? " ← YOU" : " ← ENEMY候補")
             //   );
            }

         //   Debug.Log("=======================");

            if (states == null || states.Length == 0)
            {
              //  Debug.LogWarning("[MULTI] states null or empty");
                isFetching = false;
                yield break;
            }

            UpdateRemoteUI(states);
            CheckMatchSuccess(states);
        }

        isFetching = false;
    }

    void UpdateRemoteUI(PlayerState[] states)
    {
        bool enemyFound = false;

        foreach (var ps in states)
        {
            if (ps == null) continue;

            // 自分除外
            if (ps.player_name == playerName)
                continue;

            enemyFound = true;

            // ✅ スコアが変わったときだけ更新
            if (lastEnemyScore != ps.score)
            {
                lastEnemyScore = ps.score;

                opponentName = ps.player_name;
                opponentScore = ps.score;

                matchState.SetEnemy(ps.player_name, ps.score);

                OnEnemyScoreChanged?.Invoke(ps.score);

               // Debug.Log($"[REALTIME UPDATE] {opponentName} : {opponentScore}");
            }
        }

        // 敵いなくなった場合
        if (!enemyFound && enemyPreviouslyPresent)
        {
            enemyPreviouslyPresent = false;
            opponentName = "";
            opponentScore = 0;
        }

        enemyPreviouslyPresent = enemyFound;


    }


    /* ======================
       マッチ成立判定
    ====================== */
    void CheckMatchSuccess(PlayerState[] states)
    {
        //Debug.Log($"[CHECK] myName={playerName}, count={(states == null ? -1 : states.Length)}");
        //if (states != null)
        //{
        //    foreach (var ps in states)
        //    {
        //        Debug.Log($"[STATE] {ps.player_name}");
        //    }
        //}

        if (matched || states == null) return;

        foreach (var ps in states)
        {
            if (ps.player_name != playerName)
            {
                matched = true;
                enemyPreviouslyPresent = true;
               return;
            }
        }
    }


    /* ======================
       Join
    ====================== */
    public void SendJoinOnce()
    {
        if (joined) return;
        joined = true;
        StartCoroutine(SendJoinCoroutine());
    }

    IEnumerator SendJoinCoroutine()
    {
        if (string.IsNullOrEmpty(roomId) || string.IsNullOrEmpty(playerName))
        {
          //  Debug.LogError("[MULTI ERROR] roomId or playerName is NULL");
            yield break;
        }

        WWWForm form = new WWWForm();
        form.AddField("room_id", roomId);
        form.AddField("player_name", playerName);
        form.AddField("difficulty", GameManager.instance.currentLevel.ToString());

        using (UnityWebRequest req = UnityWebRequest.Post(joinUrl, form))
        {
            req.timeout = 10;
            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
            {
                //Debug.Log("[MultiSync] Joined room");
            }
            else
            {
              // Debug.LogError("[MultiSync] Join failed: " + req.error);
            }
        }
    }


    void SendState()
    {

        if (string.IsNullOrEmpty(roomId) || string.IsNullOrEmpty(playerName))
        {
         //   Debug.LogError("[SEND ERROR] roomId or playerName NULL!");
           return;
        }

        if (isSending) return;
        StartCoroutine(SendStateCoroutine());
    }

    public void StopMultiSync()
    {
        CancelInvoke();
        StopAllCoroutines();
        enabled = false;

       // Debug.Log("[MULTI] Sync stopped");
    }

    // =====================
    // 手動スコア送信（ボタン用）
    // =====================
    public void SendScoreManually()
    {
        if (GameManager.instance.currentMode != GameMode.Multi) return;

        if (isSending) return;

        // ✅ MatchState を読むだけ
        currentScore = matchState.MyScore;

       // Debug.Log(
          //  $"[MULTI SEND] name={matchState.MyName}, score={currentScore}"
       // );

        StartCoroutine(SendScoreManuallyCoroutine());
    }

    IEnumerator SendScoreManuallyCoroutine()
    {
        
        yield return StartCoroutine(SendStateCoroutine());
        OnScoreSent?.Invoke(); // ✅ 通信後
    }

    public void ResetJoinState()
    {
        joined = false;
        matched = false;
        enemyPreviouslyPresent = false;
        lastEnemyScore = -1;
    }

    public void SendScoreIfHigher(int currentTotalScore)
    {
      //  Debug.Log($"[CHECK SEND] current={currentTotalScore}, last={lastSentScore}");

        if (currentTotalScore <= lastSentScore)
        {
         //  Debug.Log("[SKIP SEND]");
            return;
        }

        lastSentScore = currentTotalScore;
        currentScore = currentTotalScore;

      //  Debug.Log($"[MULTI SEND] new high score = {currentScore}");
        StartCoroutine(SendStateCoroutine());
    }
}

/* ======================
   通信用データ
====================== */
[System.Serializable]
public class PlayerState
{
    public string player_name;
    public int score;
}