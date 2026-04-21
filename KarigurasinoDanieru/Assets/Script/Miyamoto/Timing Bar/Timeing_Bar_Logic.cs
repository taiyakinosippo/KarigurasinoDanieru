using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Timeing_Bar_Logic : MonoBehaviour
{
    [Header("Timing Bar 設定")]
    [SerializeField] private GameMode gameMode = GameMode.Normal;     // ゲームモードの設定
    [SerializeField] private float speed = 500;                       // タイミングバーの移動速度
    [SerializeField] private float maxwindth = 400f;                  // タイミングバーの最大幅
    [Header("判定ライン設定")]
    [SerializeField] private float first_goodline_min = 10;           //最初のグッドラインの最小値
    [SerializeField] private float first_goodline_max = 30;           //最初のグッドラインの最大値
    [SerializeField] private float last_goodline_min = 70;            //最後のグッドラインの最小値
    [SerializeField] private float last_goodline_max = 90;            //最後のグッドラインの最大値
    [SerializeField] private float first_greatline_min = 30;          //最初のグレートラインの最小値
    [SerializeField] private float first_greatline_max = 45;          //最初のグレートラインの最大値
    [SerializeField] private float last_greatline_min = 55;           //最後のグレートラインの最小値
    [SerializeField] private float last_greatline_max = 70;           //最後のグレートラインの最大値
    [SerializeField] private float first_perfectline = 45;            //最初のパーフェクトライン
    [SerializeField] private float last_perfectline = 55;             //最後のパーフェクトライン
    [Header("点数")]
    [SerializeField] private int miss_score = 0;　　　　　　　　　　　//ミスの点数
    [SerializeField] private int good_score = 100;                    //グッドの点数
    [SerializeField] private int great_score = 200;                   //グレートの点数
    [SerializeField] private int perfect_score = 300;                 //パーフェクトの点数
    [HideInInspector]
    public float judge = 0;                         //判定の結果を格納する変数
    [SerializeField] private UI_Barview barView;     //タイミングバーのUIを管理するクラス
    [SerializeField] private UI_Judge barText;        //タイミングバーのテキストを管理するクラス
    [SerializeField] private Timing_Bar_System timingBarSystem;　　　//タイミングバーのシステムを管理するクラス
    private bool is_playing = true;
    [SerializeField] private float reset_time = 0.3f;  //リセットするまでの時間
    public int MissScore => miss_score;             //外部からミスの点数を取得できるプロパティ
    public int GoodScore => good_score;             //外部からグッドの点数を取得できるプロパティ
    public int GreatScore => great_score;           //外部からグレートの点数を取得できるプロパティ
    public  int PerfectScore => perfect_score;       //外部からパーフェクトの点数を取得できるプロパティ
    

    void Start()
    {
        if(gameMode == GameMode.Hard)
        {
            speed *= 2f; 
            miss_score = (int)(miss_score * 0.5f);          
            good_score = (int)(good_score * 0.5f);                   
            great_score = (int)(great_score * 0.5f);                  
            perfect_score = (int)(perfect_score * 0.5f);                  
        }

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
