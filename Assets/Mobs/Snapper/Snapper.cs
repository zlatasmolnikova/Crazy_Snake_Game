using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class Snapper : MonoBehaviour, IHurtable
{
    public float Health { get; private set; } = 20;

    public float MaxHealth { get; private set; } = 20;

    public UnityEvent<float, float> OnHealthDecrease => throw new System.NotImplementedException();

    public UnityEvent<float, float> OnHealthIncrease => throw new System.NotImplementedException();

    [SerializeField]
    private GameObject avatar;

    public Animator animator { get; private set; }

    public NavMeshAgent Agent { get; private set; }

    private NavMeshPath path;

    public StateMachine StateMachine { get; private set; }

    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
        StateMachine = new StateMachine();
        path = new NavMeshPath();
    }

    private void Start()
    {
        if (avatar == null)
        {
            throw new System.Exception("avatar not set");
        }
        animator = avatar.GetComponent<Animator>();
        StateMachine.ChangeState(new SnapperIdleState(this));
    }

    public bool TryPickRandomDestination()
    {
        var randomPointRadius = 10;
        var randomPoint = transform.position + Random.insideUnitSphere * randomPointRadius;
        NavMesh.SamplePosition(randomPoint, out var hit, randomPointRadius, 1);
        var destination = hit.position;

        if (hit.position.x > 10e9 || hit.position.y > 10e9 || hit.position.z > 10e9)
            return false;

        Agent.CalculatePath(destination, path);

        if (path.status == NavMeshPathStatus.PathComplete)
            Agent.SetPath(path);
        return true;
    }

    public void ConsumeDamage(float amount)
    {
        throw new System.NotImplementedException();
    }

    public void TakeDamage(DamageInfo damageInfo)
    {
        throw new System.NotImplementedException();
    }
}
