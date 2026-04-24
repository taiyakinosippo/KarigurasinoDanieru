using UnityEngine;
using UnityEngine.UI;

public class RankingInputManager : MonoBehaviour
{
    public InputField nameInput;
    public InputField scoreInput;

    LocalRankingManager localRanking;
    ScoreSender scoreSender;

    void Start()
    {
        localRanking = FindObjectOfType<LocalRankingManager>();
        scoreSender = FindObjectOfType<ScoreSender>();
    }

    // ノーマルモード用ボタン
    public void OnClickSendNormal()
    {
        Send("normal");
    }

    // ハードモード用ボタン
    public void OnClickSendHard()
    {
        Send("hard");
    }

    void Send(string mode)
    {
        if (ModeManager.IsMultiMode)
        {
            return;
        }
        string name = nameInput.text;
        int score;

        if (string.IsNullOrEmpty(name))
            return;

        if (!int.TryParse(scoreInput.text, out score))
            return;

        //Unity内ランキング（必要なら mode 別にもできる）
        localRanking.AddOrUpdateScore(name, score,mode);

        //Webへ送信（mode付き）
        scoreSender.SendScore(name, score, mode);


        //入力欄クリア
        nameInput.text = "";
        scoreInput.text = "";
    }
}