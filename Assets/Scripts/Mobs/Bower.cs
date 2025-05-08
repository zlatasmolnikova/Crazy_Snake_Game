using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class Bower : CreatureBase, IMob
{
    private GameObject player { get; set; }
    private const float AttackDistance = 15f;
    private const float ViewDistance = 30f;
    
    public StateMachine StateMachine { get; set; }
    public float MaxHealth;
    public float CriticalHealthPercentage = 30f;

    [SerializeField] private LayerMask obstructionMask;
    private float fieldOfViewAngle = 90f;

    private float criticalDistance = 8f;

    private NavMeshAgent agent;
    private NavMeshPath path;

    [SerializeField] private float randomPointRadius = 5f;

    public IState PreviousState { get; set; }
    public bool IsBower { get; set; } = true;

    public GameObject projectilePrefab;
    public float rotationSpeed = 1.0f;
    public float rotationAngle = 90.0f;
    public float rotationInterval = 2.0f;
    
    private float currentAngle;
    private bool isRotating;
    private float timer;
    private const float attackCooldown = 2.5f;
    private float lastAttackTime = 0;

    private Animator animator;


    public Bower() : base(20, 1)
    {
        ViewAngle = 110;
    }
    
    private void Start()
    {
        animator = gameObject.GetComponentInParent<Animator>();

        agent = GetComponentInParent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");
        StateMachine = new StateMachine();
        StateMachine.ChangeState(new IdleState(this));
        path = new NavMeshPath();
    }
    
    private void Update()
    {
        timer += Time.deltaTime;

        StateMachine.Update();

        //Debug.Log((PreviousState, StateMachine.currentState, StateMachine.StateBrandNew));
        if (StateMachine.currentState is IdleState && PreviousState is RunBackState && StateMachine.StateBrandNew)
        {
            //Debug.Log(agent.remainingDistance);
            
            //Debug.Log("ROTAAAAAAAAAAATION");
            transform.parent.transform.Rotate(0, 180, 0);
            agent.ResetPath();
            StateMachine.StateBrandNew = false;
            
        }

        if (StateMachine.currentState is IdleState)
        {
            animator.SetBool("IsWalking", false);
            animator.SetBool("IsRunning", false);
            animator.SetBool("Idle", true);
            TryPatrolInPlace();
        }
        else if (StateMachine.currentState is WanderState)
        {
            animator.SetBool("Idle", false);
            animator.SetBool("IsRunning", false);
            animator.SetBool("IsWalking", true);
            TryPickRandomDestination();
        }
        else if (StateMachine.currentState is ChaseState)
        {
            animator.SetBool("IsWalking", false);
            animator.SetBool("Idle", false);
            animator.SetBool("IsRunning", true);
            ChasePlayer();
        }
        else if (StateMachine.currentState is AttackState)
        {
            animator.SetBool("IsWalking", false);
            animator.SetBool("Idle", false);
            animator.SetBool("IsRunning", false);
            //animator.SetTrigger("Attack");
            if (Time.time - lastAttackTime > attackCooldown)
                PerformAttack();
        }
        else if (StateMachine.currentState is RunBackState)
        {
            animator.SetBool("IsWalking", false);
            animator.SetBool("IsRunning", true);
            animator.SetBool("Idle", false);
        }
    }
    
    private void TryPatrolInPlace()
    {
        if (timer >= rotationInterval && !isRotating)
        {
            isRotating = true;
            currentAngle = 0.0f;
            timer = 0.0f;
        }
        if (isRotating)
        {
            RotateMob();
        }
    }

    private void RotateMob()
    {
        var maxRadians = rotationAngle * Mathf.Deg2Rad;
        currentAngle += rotationSpeed * Time.deltaTime;
        var radians = currentAngle * Mathf.Deg2Rad;

        // »спользуем синус дл€ модул€ции скорости
        var sinusoidalSpeed = Mathf.Sin(Mathf.PI * radians / maxRadians);

        // ѕримен€ем скорость поворота
        transform.Rotate(Vector3.up, sinusoidalSpeed * Time.deltaTime * rotationSpeed);
        
        // ѕровер€ем, достигли ли мы конечного угла
        if (radians >= maxRadians)
        {
            isRotating = false;
        }
    }

    public override void PerformAttack()
    {
        var projectile = Instantiate(projectilePrefab, transform.position + new Vector3(0, 1, 0), Quaternion.identity);
        var rb = projectile.GetComponent<Rigidbody>();

        var targetDir = player.transform.position - transform.position;
        var h = targetDir.y; // высота
        targetDir.y = 0; // рассто€ние на плоскости xz
        var distance = targetDir.magnitude;
        var a = 22 * Mathf.Deg2Rad;
        targetDir.y = distance * Mathf.Tan(a);
        distance += h / Mathf.Tan(a);

        // –ассчитываем начальную скорость
        var velocity = Mathf.Sqrt(distance * Physics.gravity.magnitude / Mathf.Sin(2 * a));
        rb.velocity = velocity * targetDir.normalized;

        lastAttackTime = Time.time;
    }

    public bool CanSeePlayer()
    {
        var dirToPlayer = player.transform.position - transform.position;
        var angleToPlayer = Vector3.Angle(transform.parent.transform.forward, dirToPlayer);

        if (!(angleToPlayer < ViewAngle / 2) || !(dirToPlayer.magnitude < ViewDistance)) 
            return false;
        
        if (!Physics.Raycast(transform.position, dirToPlayer.normalized, out var hit, ViewDistance)) 
            return false;
        
        return hit.transform == player.transform;
    }

    public bool CanAttackPlayer()
    {
        var distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        
        return distanceToPlayer <= AttackDistance;
    }

    public float GetHealthPercentage()
    {
        return Health / MaxHealth * 100;
    }

    public void RunAway()
    {
        var toPlayerReversed = transform.position - player.transform.position;
        //toPlayerReversed.Normalize();
        toPlayerReversed *= 5;
        var goodPosition = transform.position + toPlayerReversed;

        agent.SetDestination(goodPosition);
    }

    public float GetDistanceToPlayer()
    {
        return Vector3.Distance(transform.position, player.transform.position);
    }

    public float GetCriticalDistance()
    {
        return criticalDistance;
    }

    public float GetCriticalHealthPercentage()
    {
        return CriticalHealthPercentage;
    }

    public void ChasePlayer()
    {
        var toPlayer = transform.position - player.transform.position;
        toPlayer.Normalize();
        var goodLengthToPlayer = AttackDistance * toPlayer;
        var goodPosition = player.transform.position + goodLengthToPlayer;

        agent.SetDestination(goodPosition);
    }

    public void TryPickRandomDestination()
    {
        var randomPoint = transform.position + Random.insideUnitSphere * randomPointRadius;
        NavMesh.SamplePosition(randomPoint, out var hit, randomPointRadius, 1);
        var destination = hit.position;
        
        if (hit.position.x > 10e9 || hit.position.y > 10e9 || hit.position.z > 10e9)
            return;
        
        agent.CalculatePath(destination, path);

        if (path.status == NavMeshPathStatus.PathComplete)
            agent.SetPath(path);

        //agent.SetPath(path);
    }


    private void UpdateDestination()
    {
        var check = CanSeePlayer();
        Debug.Log(check);
        if (check)
        {
            agent.stoppingDistance = 2;
            agent.SetDestination(player.transform.position);
        }
        else
            agent.stoppingDistance = 0;

        if (!agent.hasPath || (agent.hasPath && !check))
            TryPickRandomDestination();
    }
}
