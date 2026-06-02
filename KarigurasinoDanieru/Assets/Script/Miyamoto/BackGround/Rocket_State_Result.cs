using System;
using System.Collections.Generic;
using UnityEngine;
/// =============================================
///スコアに応じてロケットの演出を切り替える管理スクリプト
/// =============================================
public class Rocket_State_Result : MonoBehaviour
{
    [SerializeField]private StageManager stageManager; 
    [SerializeField]private Rocket_Mover SoloRocket;
    [SerializeField]private Rocket_Mover MultiRocket;
    public Action<FlightState> soloState;
    public Action<FlightState> multiState;
    private FlightState currentState;
    private Dictionary<FlightState, FlightBehavior> behaviors;
    int score = 0;
    void Start()
    {
        behaviors = new Dictionary<FlightState, FlightBehavior>()
        {
          { FlightState.Miss, new MissBehavior() },
          { FlightState.Sky, new SkyBehavior() },
          { FlightState. Atmosphere, new  AtmosphereBehavior() },
          { FlightState.Space, new SpaceBehavior() },
          {FlightState.Galaxy, new GalaxyBehavior() }
        };
        soloState += SoloEffect;
        multiState += MultiEffect;

        UI_Manager.OnSoloCountFinished += SoloSelectState;
        if (GameManager.instance.currentMode == GameMode.Multi)
        {
            UI_Manager.OnMultiScoreFinished += MultiSelectState;
        }
    }
    private void OnDisable()
    {
        soloState -= SoloEffect;
        multiState -= MultiEffect;

        UI_Manager.OnSoloCountFinished -= SoloSelectState;
        UI_Manager.OnMultiScoreFinished -= MultiSelectState;
    }

    void SoloSelectState()
    {
        score = (int)ScoreManager.instance.SoloResultScore();

        FlightState newState = stageManager.GetFlightState(score);

        currentState = newState;

        soloState?.Invoke(currentState);

    }

    void MultiSelectState()
    {
        score = (int)ScoreManager.instance.MultiResultScore();
        FlightState newState = stageManager.GetFlightState(score);
        currentState = newState;
        multiState?.Invoke(currentState);
    }

    void SoloEffect(FlightState state)
    {
        if (behaviors.ContainsKey(state))
        {
            behaviors[state].Execute(SoloRocket);
        }
    }
    
    void MultiEffect(FlightState state)
    {
        if (behaviors.ContainsKey(state))
        {
            behaviors[state].Execute(MultiRocket);
        }
    }


}
