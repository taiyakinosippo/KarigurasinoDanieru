using System.Threading;
using UnityEngine;

public class PointAreaController : MonoBehaviour
{
    public RectTransform area; //ポイントエリアUI

    [Header("移動設定")]
    public float moveRange = 100f;//移動範囲
    public float moveDuration = 0.5f;//移動にかかる時間(どんな距離でも一定)

    [Header("難易度設定")]
    [SerializeField] private GameLevel gameLevel = GameLevel.Normal; //難易度切替

    public float nomalMoveInterval = 2f;   //通常時の移動間隔
    public float hardMoveInterval  = 0.5f; //ハードモード時の移動間隔
    private float currentInterval;

    private float centerY; //初期位置
    private float timer;   //移動タイミング管理用

    private Vector2 targetPos;     //次の目的地
    private Vector2 startPos;      //移動開始位置
    private float moveTimer;       //移動経過時間
    private bool isMoving = false; //移動中かどうか
    private bool isGameOver = false;

    void Start()
    {
        //初期位置を中心として保存
        centerY = area.anchoredPosition.y;

        //最初の目的地を設定
        SetNewTarget();

        //難易度によって移動間隔を切り替える
        if (gameLevel == GameLevel.Normal)
        {
            currentInterval = nomalMoveInterval;
        }
        else
        {
            currentInterval = hardMoveInterval;

        }
    }

    void Update()
    {
        if (isGameOver) return;

        timer += Time.deltaTime;

        
        //一定時間ごとに移動開始
        if (timer >= currentInterval)
        {
            timer = 0f;
            StartMove();
        }

        //移動中の処理
        if (isMoving)
        {
            moveTimer += Time.deltaTime;
            float t = moveTimer / moveDuration;

            area.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);

            //到達したら移動終了
            if (t >= 1f)
            {
                isMoving = false;
            }
        }
    }

    //移動開始処理
    void StartMove()
    {
        startPos = area.anchoredPosition;
        SetNewTarget();
        moveTimer = 0f;
        isMoving = true;
    }

    //ランダムな移動先を設定
    void SetNewTarget()
    {
        float randomY = Random.Range(centerY - moveRange, centerY + moveRange);
        targetPos = new Vector2(area.anchoredPosition.x, randomY);
    }

    public void StopPointArea()
    {
        isGameOver = true;
        isMoving = false;
    }
}
