using UnityEngine;
//飛行状態のインターフェース
public interface IFlightState
{
    void Enter(ScoreController controller);

    void Update(ScoreController controller);

    void Exit(ScoreController controller);
}
