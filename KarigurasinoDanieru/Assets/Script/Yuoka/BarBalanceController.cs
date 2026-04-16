using UnityEngine;
using UnityEngine.InputSystem;

public class BarBalanceController : MonoBehaviour
{
    public RectTransform bar;

    [Header("速度設定")]
    public float minSpeed = 10f;    //最低速度
    public float maxSpeed = 100f;   //最高速度
    public float acceleration = 20f;//加速度
    public float currentSpeed = 0f; //現在の速度

    [Header("移動範囲")]
    public float moveRange = 10f;

    [Header("スコア設定")]
    public int baseScore = 1000; //基礎スコア
    public float multiplier;     //倍率
    public int currentScore;

    private float centerY; //初期位置
    private bool isFirstFrame = true;

    void Start()
    {
        centerY = bar.anchoredPosition.y;
    }

    void Update()
    {
        bool isPressed = GetInput();

        MoveBar(isPressed);

        CulculateScore();
    }

    //入力取得
    bool GetInput()
    {
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
        float direction = isPressed ? 1f : -1f;

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

        bar.anchoredPosition = pos;
    }

    void CulculateScore()
    {
        float distance = Mathf.Abs(bar.anchoredPosition.y - centerY);

        float normalized = distance / moveRange;

        multiplier = 1f - normalized;

        multiplier = Mathf.Clamp01(multiplier);

        currentScore = Mathf.RoundToInt(baseScore * multiplier);
    }
}