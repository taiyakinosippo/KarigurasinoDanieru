using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class LocalRankingManager : MonoBehaviour
{
    [System.Serializable]
    public class ScoreData
    {
        public string name;
        public int score;
    }

    public Text rankText;

    List<ScoreData> scoreList = new List<ScoreData>();

    // スコア追加 or 上書き
    public void AddOrUpdateScore(string name, int score, string mode)
    {

        if (ModeManager.IsMultiMode)
        {
            return;
        }

        ScoreData existing = scoreList.Find(x => x.name == name);

        if (existing != null)
        {
            if (score > existing.score)
                existing.score = score;
        }
        else
        {
            scoreList.Add(new ScoreData
            {
                name = name,
                score = score
            });
        }

        UpdateResultView(name, mode);
    }

    //演出用表示
    void UpdateResultView(string playerName, string mode)
    {
        // スコア降順ソート
        scoreList.Sort((a, b) => b.score.CompareTo(a.score));

        int rank = -1;

        for (int i = 0; i < scoreList.Count; i++)
        {
            if (scoreList[i].name == playerName)
            {
                rank = i + 1;
                break;
            }
        }

        // 表示テキスト構築
        if (rank > 0 && rank <= 10)
        {
            rankText.text =
                $"{mode.ToUpper()} MODE\n" +
                $"{playerName}\n" +
                $"No.{rank}\n" +
                "Rankin!";
        }
        else
        {
            rankText.text =
                $"{mode.ToUpper()} MODE\n" +
                $"{playerName}\n" +
                "Out of ranking...";
        }
    }
}