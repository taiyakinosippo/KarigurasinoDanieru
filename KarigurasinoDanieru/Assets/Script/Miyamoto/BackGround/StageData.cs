using UnityEngine;

[System.Serializable]
public class StageData
{
    [Header("ステージの種類")]
    public StageGroup group;
    [Header("ステージのビジュアル")]
    public StageVisual[] visuals;

    [Header("表示開始高度")]
    public float minHeight;

    [Header("非表示高度")]
    public float maxHeight;
}