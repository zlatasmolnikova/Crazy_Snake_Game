using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;
using Random = UnityEngine.Random;

public class Broccoli : CreatureBase
{
    private const float BoomRadius = 4;
    private const float InAirHeight = 0.05f;
    
    private bool wasInAir;
    private bool startBooming;
    private float creationTime;
    private float boomTimer;
    private SoundController soundController;
    
    public Broccoli() : base(2, 100)
    {
    }
    
    void Start()
    {
        boomTimer = Random.Range(1.5f, 3f);
        creationTime = Time.time;
        soundController = GameObject.FindGameObjectWithTag("SoundController").GetComponent<SoundController>();
    }
    
    void Update()
    {
        if (GetDistanceToGround(gameObject) > InAirHeight)
        {
            wasInAir = true;
        }

        if (Time.time - creationTime >= boomTimer && !startBooming)
        {
            startBooming = true;
            PerformAttack();
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public override void PerformAttack()
    {
        var collidersArray = new Collider[50];
        var thisCollider = gameObject.GetComponent<Collider>();
        Physics.OverlapSphereNonAlloc(transform.position, BoomRadius, collidersArray);
        
        var creatureEntities = collidersArray
            .Where(x => x && x != thisCollider 
                          && (x.CompareTag("Creature") || x.CompareTag("Player")))
            .Select(x => x.gameObject.GetComponent<IHurtable>());
        
        soundController.PlaySound("BroccoliBoom", 0.6f, transform.position);
        
        foreach (var creature in creatureEntities)
        {
            creature.TakeDamage(new DamageInfo(Damage));
        }
        Die();
    }
    
    private float GetDistanceToGround(GameObject obj)
    {
        const float groundLevel = 0f;
        var distance = obj.transform.position.y - groundLevel - gameObject.transform.localScale.y / 2;
        
        return distance;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (wasInAir && other.gameObject.CompareTag("Player"))
        {
            PerformAttack();
            Die();
        }
    }
}
