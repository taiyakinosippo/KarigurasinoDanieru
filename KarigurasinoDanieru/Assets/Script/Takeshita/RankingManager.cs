using UnityEngine;

public class RankingInputManager : MonoBehaviour
{
    private LocalRankingManager localRanking;
    private ScoreSender scoreSender;

    void Awake()
    {
        localRanking = FindObjectOfType<LocalRankingManager>();
        scoreSender = FindObjectOfType<ScoreSender>();

        if (localRanking == null)
            Debug.LogError("[Ranking] LocalRankingManager not found");

        if (scoreSender == null)
            Debug.LogError("[Ranking] ScoreSender not found");
    }

    // =====================
    // ランキング送信（唯一の役割）
    // =====================
    public void SendScore(string name, int score, string mode)
    {
        if (string.IsNullOrEmpty(name))
        {
            Debug.LogWarning("[RANKING] name is empty");
            return;
        }

        Debug.Log($"[RANKING SEND] {name} score={score} mode={mode}");

        // ✅ ローカル演出（必要なら）
        localRanking?.AddOrUpdateScore(name, score, mode);

        // ✅ DB送信
        scoreSender.SendScore(name, score, mode);
    }
}
