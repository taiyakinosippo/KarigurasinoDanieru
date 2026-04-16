using UnityEngine;
using UnityEngine.UI;

public class RankingInputManager : MonoBehaviour
{
    public InputField nameInput;
    public InputField scoreInput;

    localRankingManager localRanking;
    ScoreSender scoreSender;

    void Start()
    {
        localRanking = FindObjectOfType<localRankingManager>();
        scoreSender = FindObjectOfType<ScoreSender>();
    }

    // ボタンから呼ぶ
    public void OnClickSend()
    {
        string name = nameInput.text;
        int score;

        if (!int.TryParse(scoreInput.text, out score))
            return;

        // ✅ Unity内ランキングに追加
        localRanking.AddOrUpdateScore(name, score);

        // ✅ Webへ送信（保存用）
        scoreSender.SendScore(name, score);

        // ✅ 入力欄クリア
        nameInput.text = "";
        scoreInput.text = "";
    }
}
