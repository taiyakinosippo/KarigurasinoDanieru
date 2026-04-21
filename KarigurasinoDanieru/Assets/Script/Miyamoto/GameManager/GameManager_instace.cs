using UnityEngine;

public interface IGameState
{
    void Enter(GameManager manager);
    void Update(GameManager manager);
    void Exit(GameManager manager);
}
