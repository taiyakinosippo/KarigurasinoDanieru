using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class RankingManager : MonoBehaviour
{
    [Header("InputField")]
    public InputField NumberInput_1; 
    public InputField NumberInput_2; 
    public InputField NumberInput_3;
    public InputField NameInput_1;
    public InputField NameInput_2;
    public InputField NameInput_3;

    public Text Rank_text;

    // InputField が変わるたびに呼ばれる
    public void OnInputChanged()
    {
        int score1 = GetNumber(NumberInput_1);
        int score2= GetNumber(NumberInput_2);
        int score3 = GetNumber(NumberInput_3);

        string name1 = GetName(NameInput_1, "Player1");
        string name2 = GetName(NameInput_2, "Player2");
        string name3 = GetName(NameInput_3, "Player3");

        UpdateRanking(name1,score1,name2,score2,name3,score3);
    }

    private int GetNumber(InputField inputField)
    {
        if (string.IsNullOrEmpty(inputField.text))
            return 0;

        int value;
        if (int.TryParse(inputField.text, out value))
            return value;

        return 0;
    }

    private string GetName(InputField inputField,string defaltname)
    {
        if (string.IsNullOrEmpty(inputField.text))
            return defaltname;

        return inputField.text;
    }

    // ✅ 同率順位対応ランキング
    private void UpdateRanking(string name1,int score1,string name2,int score2,string name3,int score3)
    {
        List<(string name, int score)> list = new List<(string, int)>
        {
            (name1,score1),
            (name2,score2),
            (name3,score3)
        };

        // スコアの高い順に並べ替え
        list.Sort((a, b) => b.score.CompareTo(a.score));

        string result = "";
        int rank = 1;
        int sameCount = 0;
        int prevScore = int.MinValue;

        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].score == prevScore)
            {
                // 同点 → 同じ順位
                sameCount++;
            }
            else
            {
                // 違う点数 → 順位を進める
                rank += sameCount;
                sameCount = 1;
            }

            result += "No. "+rank +"  "+ list[i].name +" "+list[i].score +  "\n";
            prevScore = list[i].score;
        }

        Rank_text.text = result;
    }

    public void SendWinnerToWeb(string name, int score)
    {
        WebRankingManager web =
            FindObjectOfType<WebRankingManager>();

      
    }
}