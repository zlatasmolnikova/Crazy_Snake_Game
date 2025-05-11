using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Turret : MonoBehaviour, IUser, IHurtable
{
    [SerializeField]
    private Transform turretHandTransform;

    public Transform HandTransform => turretHandTransform;

    public Transform CameraTransform => turretHandTransform;

    public Transform SelfTransform => transform;

    public Vector3 Velocity => rb.velocity;

    public float Health { get; private set; } = 50;

    public UserType UserType => UserType.Turret;

    public float MaxHealth => throw new System.NotImplementedException();

    public UnityEvent<float, float> OnHealthDecrease => throw new System.NotImplementedException();

    public UnityEvent<float, float> OnHealthIncrease => throw new System.NotImplementedException();

    public GameObject UserGameObject => gameObject;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (turretHandTransform == null)
        {
            throw new System.Exception("turret hand is absent");
        }


    }

/*    private bool TryPickUpItem()
    {
        //use layers or tags to optimize
        foreach (var item in Physics.OverlapSphere(transform.position, ItemPickUpRadius))
        {
            //Debug.Log(item);
            if (item.TryGetComponent<ItemAvatar>(out var avatar))
            {
                if (currentItem == null)
                {
                    currentItem = avatar.PickUp();
                    currentItem.SetUser(this);
                    currentItem.OnSelect();
                    return true;
                }
            }
        }
        return false;
    }*/

    // Update is called once per frame
    void Update()
    {
        
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
