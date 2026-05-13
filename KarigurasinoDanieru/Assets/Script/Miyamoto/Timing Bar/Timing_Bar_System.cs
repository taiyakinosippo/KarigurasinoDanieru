

using UnityEngine;

public class Timing_Bar_System
{
    private float _duration;
    private float _slowdown_factor;
    private float _slowdown_start;             
    private float _slowdown_end;               
    private float _first_goodline_min;
    private float _first_goodline_max;
    private float _last_goodline_min;
    private float _last_goodline_max;

    private float _first_greatline_min;
    private float _first_greatline_max;
    private float _last_greatline_min;
    private float _last_greatline_max;

    private float _first_perfectline;
    private float _last_perfectline;
    private float time = 0;
    public Timing_Bar_System(
    float duration, float slowdown_factor, float slowdown_start, float slowdown_end,
    float first_goodline_min, float first_goodline_max, float last_goodline_min, float last_goodline_max,
    float first_greatline_min, float first_greatline_max, float last_greatline_min, float last_greatline_max,
    float first_perfectline, float last_perfectline)
    {
        _duration = duration;
        _slowdown_factor = slowdown_factor;
        _slowdown_start = slowdown_start;
        _slowdown_end = slowdown_end;
        _first_goodline_min = first_goodline_min;
        _first_goodline_max = first_goodline_max;
        _last_goodline_min = last_goodline_min;
        _last_goodline_max = last_goodline_max;
        _first_greatline_min = first_greatline_min;
        _first_greatline_max = first_greatline_max;
        _last_greatline_min = last_greatline_min;
        _last_greatline_max = last_greatline_max;
        _first_perfectline = first_perfectline;
        _last_perfectline = last_perfectline;
    }

    //タイミングバーの移動を管理するメソッド
    public float MoveTimingBar(float deltaTime)
    {
        float progress = time / _duration; 
        float speedMultiplier = 1f;
        if (progress >= _slowdown_start && progress <= _slowdown_end)
        {
            speedMultiplier = _slowdown_factor; 
        }
        time += deltaTime * speedMultiplier;
        float move = (time / _duration);
        move = Mathf.Clamp01(move);
        return move;
    }

    //判定の結果を管理するメソッド
    public JudgeType judgeScore(float judge)
    {
        if (judge >= _first_perfectline && judge <= _last_perfectline)
        {
            return JudgeType.Perfect;
        }
        else if ((judge >= _first_greatline_min && judge < _first_greatline_max) || (judge > _last_greatline_min && judge <= _last_greatline_max))
        {
            return JudgeType.Great;
        }
        else if ((judge >= _first_goodline_min && judge < _first_goodline_max) || (judge > _last_goodline_min && judge <= _last_goodline_max))
        {
            return JudgeType.Good;      
        }
        else
        {
            return JudgeType.Miss;
        }

    }

    public void ResetJudge()
    {
        time = 0;
    }
}
