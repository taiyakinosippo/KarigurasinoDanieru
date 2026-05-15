using UnityEngine;

public class MiddleSpeedState : IFlightState
{
    private float timer;

    //中速の処理を開始するための処理
    public void Enter( ScoreController controller)
    {
        SpeedSettings setting = controller.CurrentSettings;

        float speed = controller.TargetScore * setting.middleSpeedRate / setting.middleSpeedTime;

        controller.SetScoreSpeed(speed);

        controller.OnMiddleSpeedStart?.Invoke();

        Debug.Log("中速開始");
    }

    //中速の処理からゆっくりのスピードに変更するための処理
    public void Update( ScoreController controller)
    {
        timer += Time.deltaTime;

        if (timer >= controller.CurrentSettings.middleSpeedTime)
        {
            controller.ChangeState(new EndSpeedState());
        }
    }

    public void Exit( ScoreController controller)
    {
        controller.OnMiddleSpeedEnd?.Invoke();

        Debug.Log("中速終了");
    }
}
