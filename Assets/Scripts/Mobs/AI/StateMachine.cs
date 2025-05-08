using UnityEngine;

public class StateMachine
{
    public IState currentState;

    public bool StateBrandNew { get; set; } = true;

    public void ChangeState(IState newState)
    {
        currentState = newState;
        StateBrandNew = true;
    }

    public void Update()
    {
        if (currentState != null)
            currentState.Execute();
    }
}