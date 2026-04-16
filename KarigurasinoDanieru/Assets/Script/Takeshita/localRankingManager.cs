using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class localRankingManager : MonoBehaviour
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
    public void AddOrUpdateScore(string name, int score)
    {
        // 同名を探す
        ScoreData existing = scoreList.Find(x => x.name == name);

        if (existing != null)
        {
            // 高いスコアなら上書き
            if (score > existing.score)
            {
                existing.score = score;
            }
        }
        else
        {
            // 新規追加
            scoreList.Add(new ScoreData
            {
                name = name,
                score = score
            });
        }

        UpdateRankingView();
    }

    void UpdateRankingView()
    {
        // スコア降順
        scoreList.Sort((a, b) => b.score.CompareTo(a.score));

        rankText.text = "";

        int currentRank = 0;
        int prevScore = int.MinValue;

        // 上位10位まで
        for (int i = 0; i < scoreList.Count && i < 10; i++)
        {
            if (scoreList[i].score != prevScore)
            {
                currentRank = i + 1;
                prevScore = scoreList[i].score;
            }

            rankText.text +=
                currentRank + "位  " +
                scoreList[i].name + "  " +
                scoreList[i].score + "\n";
        }
    }
}