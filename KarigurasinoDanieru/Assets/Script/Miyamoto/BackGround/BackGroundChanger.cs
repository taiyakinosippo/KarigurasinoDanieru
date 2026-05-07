using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;


public class BackGroundChanger : MonoBehaviour
{
    [SerializeField] private StageData[] stages;
    [SerializeField] private BackGroundMover backGroundMover;
    private Dictionary<StageGroup, List<StageData>> groupMap;
    private float fly = 0;
    void Awake()
    {
        groupMap = new Dictionary<StageGroup, List<StageData>>();

        foreach (var s in stages)
        {
            if (s == null) continue;

            if (!groupMap.ContainsKey(s.group))
            {
                groupMap[s.group] = new List<StageData>();
            }
            groupMap[s.group].Add(s);
        }
    }

    void Update()
    {
        
    }
    public Sprite GetRandomBackground()
    {
        foreach (var s in stages)
        {
            if (s == null) continue;
            if (fly >= s.minHeight && fly < s.maxHeight)
            {
                if (!groupMap.ContainsKey(s.group)) return null;
                return GetRandomFromList(s);
            }
        }
        return null;
    }

    Sprite GetRandomFromList(StageData stage)
    {
        if (stage.visuals == null || stage.visuals.Length == 0) return null;
        var visual = stage.visuals[Random.Range(0, stage.visuals.Length)];
        return visual.sprite;
    }
}


