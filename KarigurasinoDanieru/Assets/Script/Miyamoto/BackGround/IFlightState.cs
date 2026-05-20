using UnityEngine;
/// <summary>
/// 飛行状態のインターフェース
/// </summary>

public interface IFlightState
{
    void Enter(ScoreController controller);

    void Update(ScoreController controller);

    void Exit(ScoreController controller);
}
