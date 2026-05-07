using UnityEngine;

[System.Serializable]
public class StageInfo
{
    public FlightState state;
    public StageGroup group;

    public int minHeight;
    public int maxHeight;

    public StageVisual[] visuals;

    // コンストラクタ
    public StageInfo(
        FlightState state,
        StageGroup group,
        int minHeight,
        int maxHeight,
        StageVisual[] visuals)
    {
        this.state = state;
        this.group = group;
        this.minHeight = minHeight;
        this.maxHeight = maxHeight;
        this.visuals = visuals;
    }

    // 範囲判定
    public bool InRange(int height)
    {
        return height >= minHeight && height < maxHeight;
    }

    // ランダム背景取得
    public Sprite GetRandomSprite()
    {
        if (visuals == null || visuals.Length == 0)
            return null;

        StageVisual visual =
            visuals[Random.Range(0, visuals.Length)];

        return visual.sprite;
    }
}

