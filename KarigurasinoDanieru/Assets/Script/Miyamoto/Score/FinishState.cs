using UnityEngine;

public class FinishState : IFlightState
{
    public void Enter(ScoreController controller)
    {
        Debug.Log("終了しました");
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
