using UnityEngine;
using UnityEngine.InputSystem;

public class Timeing_Bar_Logic : MonoBehaviour
{
    [Header("Timing Bar 設定")]
    [SerializeField] public float speed = 500;
    [SerializeField] public float maxwindth = 400f;
    [Header("判定ライン設定")]
    public float first_missline_min = 0;  　//最初のmiss表示の最小値
    public float first_missline_max = 10;
    public float last_missline_min = 90;
    public float last_missline_max = 100;
    public float first_goodline_min = 10;
    public float first_goodline_max = 30;
    public float last_goodline_min = 70;
    public float last_goodline_max = 90;
    public float first_greatline_min = 30;
    public float first_greatline_max = 45;
    public float last_greatline_min = 55;
    public float last_greatline_max = 70;
    public float first_perfectline = 45;
    public float last_perfectline = 55;
    [Header("点数")]
    public int miss_score = 0;
    public int good_score = 100;
    public int great_score = 200;
    public int perfect_score = 300;
    [HideInInspector]
    public float judge = 0;

    public BarView barView;
    private Timing_Bar_System timingBarSystem;


    void Start()
    {
        timingBarSystem = new Timing_Bar_System(
        speed, judge,
        first_missline_min, first_missline_max, last_missline_min, last_missline_max,
        first_goodline_min, first_goodline_max, last_goodline_min, last_goodline_max,
        first_greatline_min, first_greatline_max, last_greatline_min, last_greatline_max,
        first_perfectline, last_perfectline,
        miss_score, good_score, great_score, perfect_score);
    }
     void Update()
     { 
        float move = timingBarSystem.MoveTimingBar(Time.deltaTime);
        barView.barPosition(move, maxwindth);

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            timingBarSystem.judgeScore(move);
        }
    }   
}
