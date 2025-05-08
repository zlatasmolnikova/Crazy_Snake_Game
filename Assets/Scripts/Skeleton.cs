using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class Skeleton : CreatureBase, IMob, IPlacer
{
    private GameObject player { get; set; }
    private const float AttackDistance = 2f;
    private const float ViewDistance = 30f;

    public StateMachine StateMachine { get; set; }
    public float MaxHealth;
    public float CriticalHealthPercentage = 30f;

    [SerializeField] private LayerMask obstructionMask;

    private float criticalDistance = 8f;

    private NavMeshAgent agent;
    private NavMeshPath path;

    [SerializeField] private float randomPointRadius = 5f;

    public IState PreviousState { get; set; }
    public bool IsBower { get; set; } = false;

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
    private PlayerComponent playerComponent;


    public Skeleton() : base(20, 1)
    {
        ViewAngle = 110;
    }

    private void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        agent = gameObject.GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerComponent = player.GetComponent<PlayerComponent>();
        StateMachine = new StateMachine();
        StateMachine.ChangeState(new IdleState(this));
        path = new NavMeshPath();
    }

    private void Update()
    {
        timer += Time.deltaTime;

        StateMachine.Update();

        //Debug.Log((PreviousState, StateMachine.currentState, StateMachine.StateBrandNew));

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
            if (Time.time - lastAttackTime > attackCooldown)
            {
                PerformAttack();
                animator.SetTrigger("Attack");
            }
            else
                animator.SetBool("Idle", true);
        }
        else if (StateMachine.currentState is PanicState)
        {
            animator.SetBool("IsWalking", false);
            animator.SetBool("IsRunning", true);
            animator.SetBool("Idle", false);
            RunAway();
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

        // Используем синус для модуляции скорости
        var sinusoidalSpeed = Mathf.Sin(Mathf.PI * radians / maxRadians);

        // Применяем скорость поворота
        transform.Rotate(Vector3.up, sinusoidalSpeed * Time.deltaTime * rotationSpeed);

        // Проверяем, достигли ли мы конечного угла
        if (radians >= maxRadians)
        {
            isRotating = false;
        }
    }

    public override void PerformAttack()
    {
        lastAttackTime = Time.time;
        //playerComponent.ConsumeDamage(Damage);
    }

    public bool CanSeePlayer()
    {
        //var dirToPlayer = player.transform.position - transform.position;
        //var angleToPlayer = Vector3.Angle(transform.parent.transform.forward, dirToPlayer);

        //if (!(angleToPlayer < ViewAngle / 2) || !(dirToPlayer.magnitude < ViewDistance))
        //    return false;

        //if (!Physics.Raycast(transform.position, dirToPlayer.normalized, out var hit, ViewDistance))
        //    return false;

        //return hit.transform == player.transform;

        return true;
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

    public override void Die()
    {
        animator.SetBool("IsWalking", false);
        animator.SetBool("Idle", false);
        animator.SetBool("IsRunning", false);
        animator.SetTrigger("Die");
        agent.enabled = false;
        this.enabled = false;

        //animator.enabled = false;
        //DetachChildrenRecursively(gameObject.transform);
    }

    void DetachChildrenRecursively(Transform parent)
    {


        // Проходим по всем детям текущего родителя
        foreach (Transform child in parent)
        {
            // Рекурсивно отсоединяем детей каждого ребёнка
            DetachChildrenRecursively(child);
        }

        parent.AddComponent<Rigidbody>();
        parent.AddComponent<SphereCollider>();

        // После того как все дети были обработаны, отсоединяем текущий объект от его родителя
        //parent.parent = null;
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

    public void Place(PlacementManager manager)
    {
        for (int i = 0; i < 3; i++)
        {
            var position = manager.GetPosition(2);
            Instantiate(gameObject, manager.GetTransformPosition(position), Quaternion.identity);
            manager.AddOnPlacementMap(position, 10);   
        }
    }
}
