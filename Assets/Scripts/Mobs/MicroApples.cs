using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Android;
using static UnityEngine.GraphicsBuffer;
using Random = UnityEngine.Random;

public class MicroApples : CreatureBase, IMob
{
    private GameObject player { get; set; }
    private const float AttackDistance = 2f;
    public StateMachine StateMachine { get; set; }
    public new float MaxHealth;
    public float CriticalHealthPercentage = 30f;

    [SerializeField] private LayerMask obstructionMask;
    private float fieldOfViewAngle = 90f;

    private float criticalDistance = 0f;

    private NavMeshAgent agent;
    private NavMeshPath path;

    [SerializeField] private float randomPointRadius = 5f;

    [SerializeField]
    private MicroAppleEnemyDamageDetector damageDetector;

    [SerializeField]
    private GameObject deadBodyPrefab;


    public float rotationSpeed = 1.0f;
    public float rotationAngle = 90.0f;
    public float rotationInterval = 2.0f;
    private float currentAngle = 0.0f;
    private bool isRotating = false;
    private float timer = 0.0f;

    private Animator animator;

    private bool isDead = false;


    private DamageInfo attackDamage = new DamageInfo(5, DamageType.MeleeDamage);

    public MicroApples() : base(20, 1)
    {
    }
    
    private void Start()
    {
        animator = gameObject.GetComponentInParent<Animator>();

        agent = GetComponentInParent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");
        StateMachine = new StateMachine();
        StateMachine.ChangeState(new IdleState(this));
        path = new NavMeshPath();

        if (damageDetector == null)
        {
            throw new Exception("damage detector not set");
        }
        damageDetector.OnTriggerEvent += OnPossiblyDamageRecieverDetection;
    }

    private void Update()
    {
        timer += Time.deltaTime;

        StateMachine.Update();

        //Debug.Log(CanSeePlayer());

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
            animator.SetTrigger("Attack");
            var dir = player.transform.position - agent.transform.position;
            if (dir.magnitude > 0)
            {
                var rotation = Quaternion.LookRotation(dir);
                agent.transform.rotation = Quaternion.Lerp(agent.transform.rotation, rotation, 5 * Time.deltaTime);
            }
        }
        else if (StateMachine.currentState is PanicState)
        {
            animator.SetBool("IsWalking", false);
            animator.SetBool("IsRunning", false);
            animator.SetBool("Idle", true);
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

    void RotateMob()
    {
        float maxRadians = rotationAngle * Mathf.Deg2Rad;
        currentAngle += rotationSpeed * Time.deltaTime;
        float radians = currentAngle * Mathf.Deg2Rad;

        // Используем синус для модуляции скорости
        float sinusoidalSpeed = Mathf.Sin(Mathf.PI * radians / maxRadians);

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
        //Debug.Log("MicroApple Atakin");
        //player.ConsumeDamage(Damage);
    }

    public bool CanSeePlayer()
    {
        var directionToTarget = (player.transform.position - transform.position).normalized;
        //Debug.Log(Vector3.Angle(transform.forward, directionToTarget));
        if (Vector3.Angle(transform.forward, directionToTarget) >= fieldOfViewAngle)
        {
            var distanceToTarget = Vector3.Distance(transform.position, player.transform.position);
            var let1 = Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask);
            //Debug.Log(let1);
            if (!let1)
            {
                return true;
            }
        }

        return false;
    }

    public bool CanAttackPlayer()
    {
        var distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        return distanceToPlayer <= AttackDistance ? true : false;
    }

    public float GetHealthPercentage()
    {
        return (Health / MaxHealth) * 100;
    }

    public void RunAway()
    {
        agent.SetDestination(Vector3.Normalize(transform.position - player.transform.position) * 5);
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
        agent.SetDestination(player.transform.position);
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

    public IState PreviousState { get; set; }
    public bool IsBower { get; set; }


    private void OnPossiblyDamageRecieverDetection(Collider collider)
    {
        if (StateMachine.currentState is not AttackState)
        {
            return;
        }
        if (! collider.gameObject.TryGetComponent<IHurtable>(out var hurtable))
        {
            return;
        }
        hurtable.TakeDamage(attackDamage);
        if (collider.gameObject.TryGetComponent<IPushable>(out var pushable))
        {
            pushable.Push(transform.forward * 3);
        }
    }


    /*    private void UpdateDestination()
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
        }*/

    

    public override void Die()
    {
        Destroy(transform.parent.gameObject);
    }

    public void DieCinematically(DamageInfo damageInfo)
    {
        if (isDead) return;
        isDead = true;
        var deadBody = Instantiate(deadBodyPrefab);
        deadBody.transform.position = transform.position;
        var impulseModified = damageInfo.Impulse.normalized * Mathf.Min(10, damageInfo.Impulse.magnitude);
        deadBody.GetComponent<Rigidbody>().AddForce(impulseModified, ForceMode.Impulse);
        Die();
    }

    public override void TakeDamage(DamageInfo damageInfo)
    {
        Health -= damageInfo.Amount;
        if (Health <= 0)
        {
            DieCinematically(damageInfo);
        }

    }
}
