

using UnityEngine;

public class Timing_Bar_System
{
    public float _speed;
    public float _judge;
    public float _first_missline_min;  @//Å‰‚Ìmiss•\¦‚ÌÅ¬’l
    public float _first_missline_max;
    public float _last_missline_min;
    public float _last_missline_max;

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
 
    public int _miss_score;
    public int _good_score;
    public int _great_score;
    public int _perfect_score;
    public Timing_Bar_System(
    float speed, float judge,
    float first_missline_min, float first_missline_max, float last_missline_min, float last_missline_max,
    float first_goodline_min, float first_goodline_max, float last_goodline_min, float last_goodline_max,
    float first_greatline_min, float first_greatline_max, float last_greatline_min, float last_greatline_max,
    float first_perfectline, float last_perfectline,
    int miss_score, int good_score, int great_score, int perfect_score)
    {
        _speed = speed;
        _judge = judge;
        _first_missline_min = first_missline_min;
        _first_missline_max = first_missline_max;
        _last_missline_min = last_missline_min;
        _last_missline_max = last_missline_max;
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
        _miss_score = miss_score;
        _good_score = good_score;
        _great_score = great_score;
        _perfect_score = perfect_score;
    }

    public float MoveTimingBar(float deltaTime)
    {
        _judge += _speed * deltaTime;
        _judge %= 100f; 
        return _judge;
    }
    public void judgeScore(float judge)
    {
        if ((judge >= _first_missline_min && judge < _first_missline_max) || (judge > _last_missline_min && judge <= _last_missline_max))
        {
            Debug.Log("Miss! Score: " + _miss_score);
        }
        else if ((judge >= _first_goodline_min && judge < _first_goodline_max) || (judge > _last_goodline_min && judge <= _last_goodline_max))
        {
            Debug.Log("Good! Score: " + _good_score);
        }
        else if ((judge >= _first_greatline_min && judge < _first_greatline_max) || (judge > _last_greatline_min && judge <= _last_greatline_max))
        {
            Debug.Log("Great! Score: " + _great_score);
        }
        else if (judge >= _first_perfectline && judge <= _last_perfectline)
        {
            Debug.Log("Perfect! Score: " + _perfect_score);
        }
        else
        {
            Debug.Log("Out of range!");
        }
    }
}
