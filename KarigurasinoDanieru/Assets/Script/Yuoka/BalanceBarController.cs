using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class BalanceBarController : MonoBehaviour
{
    public RectTransform bar;       //バー本体
    public RectTransform pointArea; //標的
    public Image MeterImage;        //メーターUI

    [Header("速度設定")]
    public float minSpeed = 10f;    //最低速度
    public float maxSpeed = 100f;   //最高速度
    public float acceleration = 20f;//加速度
    [SerializeField]private float currentSpeed = 0f; //現在の速度

    [Header("移動範囲")]
    public float moveRange = 10f; //移動範囲

    [Header("メーター設定")]
    public float meter = 0f;    //現在のメーター
    public float maxMeter = 1f; //メーターの最大値

    public float increaseSpeed = 0.5f; //エリア内でのメーターの増加速度
    public float decreaseSpeed = 0.5f; //エリア内でのメーターの減少速度

    [Header("スコア設定")]
    public int baseScore = 1000;   //基礎スコア
    public float multiplier = 1f;       //倍率

    private float centerY;            //初期位置
    private bool isFirstFrame = true; //初回だけの処理
    private bool isGameOver = false;

    void Start()
    {
        //初期位置を中心として保存
        centerY = bar.anchoredPosition.y;
    }

    void Update()
    {
        if (isGameOver)return;

        //入力取得
        bool isPressed = GetInput();

        //バーの移動処理
        MoveBar(isPressed);

        //メーター更新
        UpdateMeter();

        //Debug.Log(meter);
    }

    //入力取得
    bool GetInput()
    {
        //入力処理
        bool isPressed = Keyboard.current.leftShiftKey.isPressed;

        //スタート直後にバーが下に下がるのを防ぐ
        if (isFirstFrame)
        {
            isPressed = true;
            isFirstFrame = false;
        }

        return isPressed;
    }

    //バーの移動処理
    void MoveBar(bool isPressed)
    {
        //移動方向の設定
        float direction = isPressed ? 1f : -1f;

        //目標速度設定
        float targetSpeed = direction * maxSpeed;

        //最低速度を設定
        if (Mathf.Sign(currentSpeed) != direction)
        {
            currentSpeed = direction * minSpeed;
        }

        //加速処理
        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.deltaTime);

        //位置更新
        Vector2 pos = bar.anchoredPosition;
        pos.y += currentSpeed * Time.deltaTime;

        //範囲制限
        pos.y = Mathf.Clamp(pos.y, centerY - moveRange, centerY + moveRange);

        //反映
        bar.anchoredPosition = pos;
    }

    //メーターの更新処理
    void UpdateMeter()
    {
        //エリア内の場合は増加
        if (IsInPointArea())
        {
            meter += increaseSpeed * Time.deltaTime;
        }
        //エリア外の場合は減少
        else
        {
            meter -= decreaseSpeed * Time.deltaTime;
        }
        meter = Mathf.Clamp(meter, 0f, maxMeter);
        
        //UIに反映
        MeterImage.fillAmount = meter / maxMeter;

        //色の変更
        float t = meter / maxMeter;
        Color color = Color.Lerp(Color.yellow, Color.red, t);
        MeterImage.color = color;
    }

    //バーがエリア内にあるか判定
    bool IsInPointArea()
    {
        //バーとエリアのY座標
        float barY = bar.anchoredPosition.y;
        float areaY = pointArea.anchoredPosition.y;

        //エリアの半分の高さ
        float areaHalfHeight = pointArea.sizeDelta.y / 2f;

        //距離が半分以内ならtrue
        return Mathf.Abs(barY - areaY) <= areaHalfHeight;
    }

    //外部から止めるためのメソッド
    public void StopBar()
    {
        isGameOver = true;
        currentSpeed = 0f;
    }
}