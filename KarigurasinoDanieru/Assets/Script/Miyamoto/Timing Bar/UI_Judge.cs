using UnityEngine;
using TMPro;
public class UI_Judge : MonoBehaviour
{
    public TextMeshProUGUI judge_text;
    public Timing_Bar_Logic timing_bar_Logic;
    int score = 0;

    public void SetScoreText(JudgeType result)
    {
        switch (result)
        {
            case JudgeType.Miss:
                judge_text.text = "Miss";
                judge_text.color = Color.red;
                SESourceManager.instance.PlaySE(SEType.missSE);
                score += timing_bar_Logic.MissScore;
                break;
                
            case JudgeType.Good:
                judge_text.text = "Good";
                judge_text.color = Color.green;
                SESourceManager.instance.PlaySE(SEType.goodSE);
                score += timing_bar_Logic.GoodScore;

                break;

            case JudgeType.Great:
                judge_text.text = "Great";
                judge_text.color = Color.yellow;
                SESourceManager.instance.PlaySE(SEType.greatSE);
                score += timing_bar_Logic.GreatScore;
                break;
            case JudgeType.Perfect:
                judge_text.text = "Perfect";
                judge_text.color = Color.orange;
                SESourceManager.instance.PlaySE(SEType.perfectSE);
                score += timing_bar_Logic.PerfectScore;
                break;
        }
        ScoreManager.instance.TimingBarScore(score);
        score = 0;
    }
}
