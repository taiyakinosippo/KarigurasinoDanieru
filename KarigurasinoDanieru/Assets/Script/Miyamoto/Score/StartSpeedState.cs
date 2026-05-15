using UnityEngine;

public class StartSpeedState : IFlightState
{
    private float timer;

    //スタートスピードの処理を開始するための処理
    public void Enter(ScoreController controller)
    {
        SpeedSettings setting = controller.CurrentSettings;

        float speed =
        controller.TargetScore * setting.startSpeedRate / setting.startSpeedTime;

        controller.SetScoreSpeed(speed);

        controller .OnStartSpeedStart ?.Invoke();

        Debug.Log("高速開始");
    }
    //真ん中のスピードに移行するかの判定の処理
    public void Update(ScoreController controller)
    {
        timer += Time.deltaTime;
        if (timer >= controller.CurrentSettings.startSpeedTime)
        {
            controller.ChangeState(new MiddleSpeedState());
        }
    }

    //スタートスピードの処理を終了するための処理
    public void Exit(ScoreController controller)
    {
        controller.OnStartSpeedEnd?.Invoke();

        Debug.Log("高速終了");
    }
}
