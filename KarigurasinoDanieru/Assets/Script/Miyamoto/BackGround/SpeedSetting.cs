using UnityEngine;

//背景のスクロール速度とテキストの数値の上がる速度を管理するオブジェクト
[CreateAssetMenu(menuName = "menu/SpeedSettings")]
public class SpeedSettings : ScriptableObject
{
    [Header("どの飛行状態")]
    public FlightState flightState;

    [Header("到着時間")]
    public float arriveTime;

    [Header("スクロール速度")]
    public float scrollSpeed;
    [Header("スクロール速度減速率")]
    public float decelerationRate;

    [Header("各速度時間")]
    public float startSpeedTime;
    public float middleSpeedTime;
    public float endSpeedTime;

    [Header("各速度にする割合")]
    public float startSpeedRate;
    public float middleSpeedRate;
    public float endSpeedRate;

}
