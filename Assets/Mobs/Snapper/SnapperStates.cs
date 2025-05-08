using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


/*public abstract class SnapperStateBase : IState
{
    private Snapper snapper;

    public SnapperStateBase(Snapper snapper)
    {
        this.snapper = snapper;
    }

    public abstract void Execute();
}*/

public class SnapperIdleState : IState
{
    private float timeLeft;

    private Snapper snapper;

    public SnapperIdleState(Snapper snapper)
    {
        this.snapper = snapper;
        timeLeft = 1f;
    }

    public void Execute()
    {
        timeLeft -= Time.deltaTime;
        if (timeLeft < 0f)
        {
            snapper.StateMachine.ChangeState(new SnapperWanderState(snapper));
        }
    }
}

public class SnapperWanderState : IState
{
    private float timeLeft;

    private Snapper snapper;

    private bool pathPicked = false;

    public SnapperWanderState(Snapper snapper)
    {
        this.snapper = snapper;
        timeLeft = 1f;
    }

    public void Execute()
    {
        timeLeft -= Time.deltaTime;

        if (! pathPicked )
        {
            pathPicked = snapper.TryPickRandomDestination();
        }

        if (timeLeft < 0f || snapper.Agent.remainingDistance < 1f)
        {
            snapper.StateMachine.ChangeState(new SnapperIdleState(snapper));
        }
    }
}
