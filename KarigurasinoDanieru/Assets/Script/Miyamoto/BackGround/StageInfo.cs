using UnityEngine;

[System.Serializable]
public class StageInfo
{
    public FlightState state;
    public StageGroup group;

    public float minHeight;
    public float maxHeight;

    public Sprite[] visuals;

    public StageInfo(
        FlightState state,
        StageGroup group,
        float minHeight,
        float maxHeight,
        Sprite[] visuals)
    {
        this.state = state;
        this.group = group;
        this.minHeight = minHeight;
        this.maxHeight = maxHeight;
        this.visuals = visuals;
    }

    // ”ÍˆÍ”»’è
    public bool InRange(float height)
    {
        return height >= minHeight && height < maxHeight;
    }

    // ƒ‰ƒ“ƒ_ƒ€”wŒiŽæ“¾
    public Sprite GetRandomSprite()
    {
        if (visuals == null || visuals.Length == 0)
            return null;

        Sprite visual =
            visuals[Random.Range(0, visuals.Length)];

        return visual;
    }
}

