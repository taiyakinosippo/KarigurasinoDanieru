using UnityEngine;

public class FinishState : IFlightState
{
    public void Enter(ScoreController controller)
    {
        controller.StopPresentation();
        controller.OnFinished?.Invoke();
    }

    public void Update(ScoreController controller)
    {

    }

    public void Exit(ScoreController controller)
    {

    }
}
