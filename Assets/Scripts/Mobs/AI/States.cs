using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class WanderState : IState
{
    private IMob mob;
    private float timer;
    private float wanderTime;

    public WanderState(IMob mob)
    {
        this.mob = mob;
        wanderTime = Random.Range(5, 15);
        timer = 0;
        mob.TryPickRandomDestination();
    }

    public void Execute()
    {
        if (mob.IsBower && mob.GetDistanceToPlayer() < mob.GetCriticalDistance())
        {
            mob.StateMachine.ChangeState(new RunBackState(mob));
            
            mob.PreviousState = this;
        }
        
        if (mob.GetHealthPercentage() <= mob.GetCriticalHealthPercentage() && mob.GetDistanceToPlayer() <= mob.GetCriticalDistance())
        {
            mob.StateMachine.ChangeState(new PanicState(mob));
            
            mob.PreviousState = this;
            return;
        }

        mob.TryPickRandomDestination();

        timer += Time.deltaTime;
        if (timer > wanderTime)
        {
            mob.StateMachine.ChangeState(new IdleState(mob));
            
            mob.PreviousState = this;
        }

        if (mob.CanSeePlayer())
        {
            if (mob.CanAttackPlayer())
            {
                mob.StateMachine.ChangeState(new AttackState(mob));
                
                mob.PreviousState = this;
            }
            else
            {
                mob.StateMachine.ChangeState(new ChaseState(mob));
                
                mob.PreviousState = this;
            }
        }
    }
}


public class IdleState : IState
{
    private IMob mob;
    private float timer;
    private float idleTime;

    public IdleState(IMob mob)
    {
        this.mob = mob;
        idleTime = Random.Range(3, 10);
        timer = 0;
    }

    public void Execute()
    {
        if (mob.IsBower && mob.GetDistanceToPlayer() < mob.GetCriticalDistance())
        {
            mob.StateMachine.ChangeState(new RunBackState(mob));
            
            mob.PreviousState = this;
        }
        
        if (mob.GetHealthPercentage() <= mob.GetCriticalHealthPercentage() && mob.GetDistanceToPlayer() <= mob.GetCriticalDistance())
        {
            mob.StateMachine.ChangeState(new PanicState(mob));
            
            mob.PreviousState = this;
            return;
        }

        timer += Time.deltaTime;
        if (timer > idleTime)
        {
            mob.StateMachine.ChangeState(new WanderState(mob));
            
            mob.PreviousState = this;
        }

        if (mob.CanSeePlayer())
        {
            if (mob.CanAttackPlayer())
            {
                mob.StateMachine.ChangeState(new AttackState(mob));
                
                mob.PreviousState = this;
            }
            else
            {
                mob.StateMachine.ChangeState(new ChaseState(mob));
                
                mob.PreviousState = this;
            }
        }
    }
}


public class AttackState : IState
{
    private IMob mob;

    public AttackState(IMob mob)
    {
        this.mob = mob;
    }

    public void Execute()
    {
        if (mob.IsBower && mob.GetDistanceToPlayer() < mob.GetCriticalDistance())
        {
            mob.StateMachine.ChangeState(new RunBackState(mob));
            
            mob.PreviousState = this;
        }
        
        if (mob.GetHealthPercentage() <= mob.GetCriticalHealthPercentage() && mob.GetDistanceToPlayer() <= mob.GetCriticalDistance())
        {
            mob.StateMachine.ChangeState(new PanicState(mob));
            
            mob.PreviousState = this;
            return;
        }

        if (!mob.CanAttackPlayer())
        {
            if (mob.CanSeePlayer())
            {
                mob.StateMachine.ChangeState(new ChaseState(mob));
                
                mob.PreviousState = this;
            }
            else
            {
                mob.StateMachine.ChangeState(new IdleState(mob));
                
                mob.PreviousState = this;
            }
        }
    }
}


public class ChaseState : IState
{
    private IMob mob;
    private float chaseTimer;
    private float maxChaseTime = 30f;

    public ChaseState(IMob mob)
    {
        this.mob = mob;
        //chaseTimer = 0f;
    }

    public void Execute()
    {
        if (mob.IsBower && mob.GetDistanceToPlayer() < mob.GetCriticalDistance())
        {
            mob.StateMachine.ChangeState(new RunBackState(mob));
            
            mob.PreviousState = this;
        }
        
        if (mob.GetHealthPercentage() <= mob.GetCriticalHealthPercentage() && mob.GetDistanceToPlayer() <= mob.GetCriticalDistance())
        {
            mob.StateMachine.ChangeState(new PanicState(mob));
            
            mob.PreviousState = this;
            return;
        }

        if (mob.CanSeePlayer())
        {
            mob.ChasePlayer();

            if (mob.CanAttackPlayer())
            {
                mob.StateMachine.ChangeState(new AttackState(mob));
                
                mob.PreviousState = this;
                return;
            }

            //chaseTimer += Time.deltaTime;
            //if (chaseTimer >= maxChaseTime)
            //{
            //    mob.StateMachine.ChangeState(new WanderState(mob));
            //}
        }
        else
        {
            mob.StateMachine.ChangeState(new WanderState(mob));
            
            mob.PreviousState = this;
        }
    }
}


public class PanicState : IState
{
    private IMob mob;

    public PanicState(IMob mob)
    {
        this.mob = mob;
        mob.RunAway();
    }

    public void Execute()
    {
        if (mob.GetHealthPercentage() > mob.GetCriticalHealthPercentage() || mob.GetDistanceToPlayer() > mob.GetCriticalDistance())
        {
            mob.StateMachine.ChangeState(new IdleState(mob));
            
            mob.PreviousState = this;
        }
    }
}

public class RunBackState : IState
{
    private IMob mob;

    public RunBackState(IMob mob)
    {
        this.mob = mob;
        mob.RunAway();
    }
    
    public void Execute()
    {
        if (mob.GetDistanceToPlayer() > mob.GetCriticalDistance())
        {
            mob.StateMachine.ChangeState(new IdleState(mob));
            
            mob.PreviousState = this;
        }
    }
}