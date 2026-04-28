using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.CullingGroup;

public class Player_State_Result : MonoBehaviour
{
    public Action<FlightState> state;
    private FlightState currentState;
    private Dictionary<FlightState, FlightBehavior> behaviors;
    int score = 0;
    void Start()
    {
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

        var newState = GetStateFromHeight(score);

        if (newState != currentState)
        {
            currentState = newState;
            state?.Invoke(currentState);
        }
    }

    FlightState GetStateFromHeight(int height)
    {
        if (height == 0) return FlightState.Miss;
        if (height >= 1 && height < 1000) return FlightState.Ground;
        if (height < 10000) return FlightState.Sky;
        if (height < 100000) return FlightState.Atmosphere;
        return FlightState.Space;
    }



    void PlayerPresentation(FlightState state)
    {
        if (behaviors.ContainsKey(state))
        {
            behaviors[state].Execute(gameObject);
        }
    }
}
