using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Timing_Bar_Logic : MonoBehaviour
{
    [Header("Timing Bar 設定")]
    [SerializeField] private GameLevel gameLevel = GameLevel.Normal;     // ゲームレベルの設定
    [SerializeField] private float duration = 1f;                     // タイミングバーが移動する時間（秒）
    [SerializeField] private float maxwindth = 400f;                  // タイミングバーの最大幅
    [SerializeField] private float reset_time = 1f;                   //リセットするまでの時間
    [SerializeField] private float slowdown_start = 0.4f;             //スローダウンが始まる位置
    [SerializeField] private float slowdown_end = 0.6f;               //スローダウンが終わる位置
    [SerializeField] private float slowdown_factor = 0.5f;            //スローダウンの速度の割合
    [Header("判定ライン設定")]
    [SerializeField] private float first_goodline_min = 0.1f;           //最初のグッドラインの最小値
    [SerializeField] private float first_goodline_max = 0.3f;           //最初のグッドラインの最大値
    [SerializeField] private float last_goodline_min = 0.7f;            //最後のグッドラインの最小値
    [SerializeField] private float last_goodline_max = 0.9f;            //最後のグッドラインの最大値
    [SerializeField] private float first_greatline_min = 0.3f;          //最初のグレートラインの最小値
    [SerializeField] private float first_greatline_max = 0.45f;          //最初のグレートラインの最大値
    [SerializeField] private float last_greatline_min = 0.55f;           //最後のグレートラインの最小値
    [SerializeField] private float last_greatline_max = 0.7f;           //最後のグレートラインの最大値
    [SerializeField] private float first_perfectline = 0.45f;            //最初のパーフェクトライン
    [SerializeField] private float last_perfectline = 0.55f;             //最後のパーフェクトライン
    [Header("点数")]
    [SerializeField] private int miss_score = 0;　　　　　　　　　　　//ミスの点数
    [SerializeField] private int good_score = 100;                    //グッドの点数
    [SerializeField] private int great_score = 200;                   //グレートの点数
    [SerializeField] private int perfect_score = 300;                 //パーフェクトの点数
    [SerializeField] private UI_Barview barView;     //タイミングバーのUIを管理するクラス
    [SerializeField] private UI_Judge barText;        //タイミングバーのテキストを管理するクラス
    private Timing_Bar_System timingBarSystem;　　　//タイミングバーのシステムを管理するクラ
    public int MissScore => miss_score;             //外部からミスの点数を取得できるプロパティ
    public int GoodScore => good_score;             //外部からグッドの点数を取得できるプロパティ
    public int GreatScore => great_score;           //外部からグレートの点数を取得できるプロパティ
    public  int PerfectScore => perfect_score;       //外部からパーフェクトの点数を取得できるプロパティ
    private float judge = 0;                         //判定の結果を格納する変数
    private bool is_playing = true;
    private bool isGameOver = false;
    void Start()
    {
        gameLevel = GameManager.instance.currentLevel;
        if (gameLevel == GameLevel.Hard)
        {
            duration *= 0.5f; 
            slowdown_factor *= 2f;
            miss_score = (int)(miss_score * 0.5f);          
            good_score = (int)(good_score * 0.5f);                   
            great_score = (int)(great_score * 0.5f);                  
            perfect_score = (int)(perfect_score * 0.5f);     
            
        }

        timingBarSystem = new Timing_Bar_System(
        duration, slowdown_factor, slowdown_start, slowdown_end,
        first_goodline_min, first_goodline_max, last_goodline_min, last_goodline_max,
        first_greatline_min, first_greatline_max, last_greatline_min, last_greatline_max,
        first_perfectline, last_perfectline);
    }
     void Update()
     {
        if (isGameOver) return;
        if (!is_playing) return;

        judge = timingBarSystem.MoveTimingBar(Time.deltaTime);
        barView.barPosition(judge, maxwindth);

        if (judge >= 1f)
        {
            HandleJudge(JudgeType.Miss);
            return;
        }
        
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            JudgeType result = timingBarSystem.judgeScore(judge);
            HandleJudge(result);
            Debug.Log(result);
            Debug.Log(judge);
        }
    }   
    void HandleJudge(JudgeType result)
    {
        is_playing = false;
        barText.SetScoreText(result);
        StartCoroutine(ResetBar());
    }
    public IEnumerator ResetBar()
    {
        barView.HideBar();
        yield return new WaitForSeconds(reset_time);
        barView.ShowBar();
        timingBarSystem.ResetJudge();
        is_playing = true;
    }
    public void StopTimingBar()
    {
        isGameOver = true;
    }
}
