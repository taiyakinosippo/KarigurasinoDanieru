using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Timeing_Bar_Logic : MonoBehaviour
{
    [Header("Timing Bar 設定")]
    [SerializeField] public float speed = 500;      // タイミングバーの移動速度
    [SerializeField] public float maxwindth = 400f; // タイミングバーの最大幅
    [Header("判定ライン設定")]
    public float first_goodline_min = 10;           //最初のグッドラインの最小値
    public float first_goodline_max = 30;           //最初のグッドラインの最大値
    public float last_goodline_min = 70;            //最後のグッドラインの最小値
    public float last_goodline_max = 90;            //最後のグッドラインの最大値
    public float first_greatline_min = 30;          //最初のグレートラインの最小値
    public float first_greatline_max = 45;          //最初のグレートラインの最大値
    public float last_greatline_min = 55;           //最後のグレートラインの最小値
    public float last_greatline_max = 70;           //最後のグレートラインの最大値
    public float first_perfectline = 45;            //最初のパーフェクトライン
    public float last_perfectline = 55;             //最後のパーフェクトライン
    [Header("点数")]
    public int miss_score = 0;　　　　　　　　　　　//ミスの点数
    public int good_score = 100;                    //グッドの点数
    public int great_score = 200;                   //グレートの点数
    public int perfect_score = 300;                 //パーフェクトの点数
    [HideInInspector]
    public float judge = 0;                         //判定の結果を格納する変数

    [SerializeField] public UI_Barview barView;     //タイミングバーのUIを管理するクラス
    [SerializeField]public UI_Judge barText;        //タイミングバーのテキストを管理するクラス
    private Timing_Bar_System timingBarSystem;　　　//タイミングバーのシステムを管理するクラス
    private bool is_playing = true;
    [SerializeField]private float reset_time = 0.3f;  //リセットするまでの時間

    void Start()
    {
        timingBarSystem = new Timing_Bar_System(
        speed, 
        first_goodline_min, first_goodline_max, last_goodline_min, last_goodline_max,
        first_greatline_min, first_greatline_max, last_greatline_min, last_greatline_max,
        first_perfectline, last_perfectline);
    }
     void Update()
     {
        if (!is_playing) return;
      
        judge = timingBarSystem.MoveTimingBar(Time.deltaTime);
        barView.barPosition(judge, maxwindth);

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            JudgeType result = timingBarSystem.judgeScore(judge);
            barText.SetScoreText(result);

            StartCoroutine(ResetBar());
        }
    }   

    public IEnumerator ResetBar()
    {
        is_playing = false;
        barView.HideBar();
        yield return new WaitForSeconds(reset_time);
        barView.ShowBar();
        timingBarSystem.ResetJudge();
        is_playing = true;
    }
}
