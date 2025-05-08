using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public interface IMob
{
    public StateMachine StateMachine { get; set; }
    public bool CanSeePlayer();
    public bool CanAttackPlayer();
    public float GetHealthPercentage();
    public void RunAway();
    public float GetDistanceToPlayer();
    public float GetCriticalDistance();
    public float GetCriticalHealthPercentage();
    public void ChasePlayer();
    public void TryPickRandomDestination();
    
    public IState PreviousState { get; set; }
    public bool IsBower { get; set; }
}
