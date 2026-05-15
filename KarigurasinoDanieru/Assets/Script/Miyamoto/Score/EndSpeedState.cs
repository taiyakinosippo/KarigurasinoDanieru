using UnityEngine;

public class EndSpeedState　: IFlightState
{
    private float timer;

    //低速の処理を開始するための処理
    public void Enter(ScoreController controller)
    {
        timer = 0;
        SpeedSettings setting = controller.CurrentSettings;
        float speed = controller.TargetScore * setting.endSpeedRate / setting.endSpeedTime;
        controller.SetScoreSpeed(speed);
        controller.OnEndSpeedStart?.Invoke();
        Debug.Log("低速開始");
    }

    //低速の処理を終わらせるための処理
    public void Update(ScoreController controller)
    {
        timer += Time.deltaTime;

        if (timer >=
            controller.CurrentSettings.endSpeedTime)
        {
            controller.ChangeState(new FinishState());
        }
    }

    //低速の処理を終わったあとの処理
    public void Exit(ScoreController controller)
    {
        controller.OnEndSpeedEnd?.Invoke();

        Debug.Log("低速終了");
    }
}
