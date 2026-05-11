using UnityEngine;

[System.Serializable]
public class StageData
{
    [Header("プレイヤーの状態")]
    public FlightState state;
    [Header("ステージの種類")]
    public StageGroup group;
    [Header("ステージのビジュアル")]
    public Sprite[] visual;

    [Header("表示開始高度")]
    public float minHeight;

    [Header("非表示高度")]
    public float maxHeight;
}