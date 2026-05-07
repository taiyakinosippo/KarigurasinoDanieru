using System.Collections.Generic;
using UnityEditor;
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
                data.maxHeight,
                data.visuals
            );

            stages.Add(info);
        }
    }

    // 高さからステージ取得
    public StageInfo GetStage(int height)
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
    public Sprite GetRandomBackground(int height)
    {
        StageInfo stage = GetStage(height);

        if (stage == null)
            return null;

        return stage.GetRandomSprite();
    }


    public FlightState GetFlightState(int height)
    {
        StageInfo stage = GetStage(height);

        if (stage == null)
            return FlightState.Miss;

        return stage.state;
    }
}
