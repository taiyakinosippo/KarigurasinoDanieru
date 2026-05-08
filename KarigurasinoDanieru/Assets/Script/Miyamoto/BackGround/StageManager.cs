using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    [Header("ステージ設定")]
    [SerializeField] private StageData[] stageDatas;

    private List<StageInfo> stages;

    void Awake()
    {
        stages = new List<StageInfo>();

        foreach (var data in stageDatas)
        {
            if (data == null) continue;

            StageInfo info = new StageInfo(
                data.state,
                data.group,
                data.minHeight,
                data.group == StageGroup.Space
                ? Mathf.Infinity
        :       data.maxHeight,
                data.visual
            );

            stages.Add(info);
        }
    }

    // 高さからステージ取得
    public StageInfo GetStage(float height)
    {
        foreach (var stage in stages)
        {
            if (stage.InRange(height))
            {
                return stage;
            }
        }

        return null;
    }

    // ランダム背景取得
    public Sprite GetRandomBackground(float height)
    {
        StageInfo stage = GetStage(height);

        if (stage == null)
            return null;

        return stage.GetRandomSprite();
    }


    public FlightState GetFlightState(float height)
    {
        StageInfo stage = GetStage(height);

        if (stage == null)
            return FlightState.Miss;

        return stage.state;
    }
}
