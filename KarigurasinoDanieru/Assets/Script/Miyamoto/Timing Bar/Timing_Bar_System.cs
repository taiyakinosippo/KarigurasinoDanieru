

using UnityEngine;

public class Timing_Bar_System
{
    public float _speed;
    public float _judge;

    public float _first_goodline_min;
    public float _first_goodline_max;
    public float _last_goodline_min;
    public float _last_goodline_max;

    public float _first_greatline_min;
    public float _first_greatline_max;
    public float _last_greatline_min;
    public float _last_greatline_max;

    public float _first_perfectline;
    public float _last_perfectline;
 
    public Timing_Bar_System(
    float speed, 
    float first_goodline_min, float first_goodline_max, float last_goodline_min, float last_goodline_max,
    float first_greatline_min, float first_greatline_max, float last_greatline_min, float last_greatline_max,
    float first_perfectline, float last_perfectline)
    {
        _speed = speed;
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
        _judge += _speed * deltaTime;           
        float move = Mathf.PingPong(_judge, 100f);
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
        _judge = 0;
    }
}
