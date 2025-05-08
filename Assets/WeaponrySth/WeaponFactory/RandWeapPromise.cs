using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandWeapPromise : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GenerateAndDestroy());
    }

    private IEnumerator GenerateAndDestroy()
    {
        yield return new WaitForEndOfFrame();       // this is crusial

        var go = GameObject.FindGameObjectWithTag("WeaponFactory");
        //Debug.Log(go);
        var wf = go.GetComponent<WeaponFactory>();
        Debug.Log(wf);
        Debug.Log(transform.position);
        Debug.Log(wf.CreateRandomWeaponLevelOne(transform.position));
        //wf.CreateRandomWeaponLevelOne(transform.position);
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
