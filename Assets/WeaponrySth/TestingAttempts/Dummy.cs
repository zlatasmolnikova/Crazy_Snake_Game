using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Dummy : MonoBehaviour, IHurtable
{
    public float Health { get; private set; } = 100;

    public float MaxHealth => throw new NotImplementedException();

    public UnityEvent<float, float> OnHealthDecrease => throw new NotImplementedException();

    public UnityEvent<float, float> OnHealthIncrease => throw new NotImplementedException();

    public string Name = "MagicMan";

    public void ConsumeDamage(float amount)
    {
        throw new System.NotImplementedException();
    }

    public void TakeDamage(DamageInfo damageInfo)
    {
        Health -= damageInfo.Amount;
        Debug.Log($"{Name} hurt. {Health} health left");
        if (Health < 0)
        {
            Die();
        }
    }

    public void Die()
    {
        var children = new List<GameObject>();
        for (int i = 0; i < transform.childCount; i++)
        {
            children.Add(transform.GetChild(i).gameObject);
        }
        foreach (var child in children)
        {
            child.transform.parent = null;
            child.GetComponent<Collider>().enabled = true;
            child.AddComponent<Rigidbody>()
                .AddExplosionForce(45, transform.position, 3, -1);
        }
        //StartCoroutine(StopTime());
        Destroy(gameObject);
    }

/*    public IEnumerator StopTime()
    {
        yield return new WaitForSeconds(0.03f);
        Debug.Log("stop time");
        Time.timeScale = 0;
        Destroy(gameObject);
    }*/

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.GetComponent<Collider>().enabled = false;
        }
    }
}
