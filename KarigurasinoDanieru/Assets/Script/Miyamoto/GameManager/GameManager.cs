using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    

    private IGameState currentState;

    void Start()
    {
        ChangeState(new Opening_State());
    }

    void Update()
    {
        currentState?.Update(this);
    }

    public void ChangeState(IGameState newState)
    {
        currentState?.Exit(this);
        currentState = newState;
        currentState.Enter(this);
    }

}
