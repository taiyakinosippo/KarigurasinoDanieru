using System;
using System.Collections.Generic;
using UnityEngine;
/// =============================================
///スコアに応じてロケットの演出を切り替える管理スクリプト
/// =============================================
public class Player_State_Result : MonoBehaviour
{
    [SerializeField]private StageManager stageManager; 
    [SerializeField]private Rocket_Mover rocket;
    public Action<FlightState> state;
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
        state += PlayerEffect;

        UI_Manager.OnCountFinished += SelectBackground;
    }


    void SelectBackground()
    {
        score = (int)ScoreManager.instance.GetScore();

        FlightState newState = stageManager.GetFlightState(score);

        currentState = newState;

        state?.Invoke(currentState);

    }

void PlayerEffect(FlightState state)
    {
        if (behaviors.ContainsKey(state))
        {
            behaviors[state].Execute(rocket);
        }
    }
}
