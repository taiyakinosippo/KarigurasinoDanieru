using UnityEngine;

[System.Serializable]
public class StageData
{
    [Header("プレイヤーの状態")]
    public FlightState state;
    [Header("ステージの種類")]
    public StageGroup group;
    [Header("ステージのビジュアル")]
    public StageVisual[] visuals;

    [Header("表示開始高度")]
    public int minHeight;

    [Header("非表示高度")]
    public int maxHeight;
}