using System;
using System.Collections.Generic;
using UnityEngine;

public class Player_State_Result : MonoBehaviour
{
    public Action<FlightState> state;
    private FlightState currentState;
    private Dictionary<FlightState, FlightBehavior> behaviors;
    private StageManager stageManager;
    private GameObject rocket;
    int score = 0;
    void Start()
    {
        stageManager = GetComponent<StageManager>();
        rocket = GetComponent<GameObject>();
        behaviors = new Dictionary<FlightState, FlightBehavior>()
    {
        { FlightState.Miss, new MissBehavior() },
        { FlightState.Ground, new GroundBehavior() },
        { FlightState.Sky, new SkyBehavior() },
        { FlightState. Atmosphere, new  AtmosphereBehavior() },
        { FlightState.Space, new SpaceBehavior() }
    };

        state += PlayerPresentation;
    }


    public void SetScore(int value)
    {
        score = value;

          FlightState newState = stageManager.GetFlightState(score);

        if (newState != currentState)
        {
            currentState = newState;
            state?.Invoke(currentState);
        }
    }

    void PlayerPresentation(FlightState state)
    {
        if (behaviors.ContainsKey(state))
        {
            behaviors[state].Execute(rocket);
        }
    }
}
