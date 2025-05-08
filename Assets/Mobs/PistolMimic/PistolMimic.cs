using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class PistolMimic : MonoBehaviour, IInventoryItem, IHurtable
{
    public float Health { get; private set; } = 20;

    public float MaxHealth => 20;

    private DamageInfo closeRangeDamage = new DamageInfo(5, DamageType.ProjectileDamage);

    public UnityEvent<float, float> OnHealthDecrease => throw new System.NotImplementedException();

    public UnityEvent<float, float> OnHealthIncrease => throw new System.NotImplementedException();

    public CardInventory CardInventory { get; private set; } = new CardInventory(5);

    // temporary (before GUI is in use)
    [SerializeField]
    private List<Spell> spells;

    private Collider colliderForDetection;

    private IUser user;

    private StateMachine stateMachine = new StateMachine();

    // other

    [SerializeField]
    private bool useInspectorSpellList = false;

    [SerializeField]
    private Transform tipOfTheGun;

    private ProjectileFactory projectileFactory;

    [SerializeField]
    private Texture2D spriteTexture;

    private Sprite sprite;

    [SerializeField]
    private GameObject avatar;

    private Animator animator;

    [SerializeField]
    private GameObject gorePrefab;

    private void Awake()
    {
        colliderForDetection = GetComponent<Collider>();
        animator = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        stateMachine.ChangeState(new SleepingPistolMimicState(this));

        var projectileFactoryGameObject = GameObject.FindGameObjectWithTag("ProjectileFactory");
        if (projectileFactoryGameObject == null || !projectileFactoryGameObject.TryGetComponent(out projectileFactory))
        {
            throw new System.Exception("mimic needs a projectile factory on scene; (use prefab)");
        }

        if (spriteTexture == null)
        {
            throw new Exception("sprite texture not found");
        }

        sprite = Sprite.Create(spriteTexture,
            new Rect(0.0f, 0.0f, spriteTexture.width, spriteTexture.height),
                new Vector2(0.5f, 0.5f)
            );

        if (avatar == null)
        {
            throw new Exception("avatar not set");
        }

        if (gorePrefab == null)
        {
            throw new Exception("gore prefab not set");
        }
    }

    public void SetCards(CardInventory cardInventory)
    {
        CardInventory = cardInventory;
    }

    public void SetUseCardsFromInventory(bool useCardsFromInventory)
    {
        useInspectorSpellList = !useCardsFromInventory;
    }

    // Update is called once per frame
    void Update()
    {
        stateMachine.Update();
    }

    public void ConsumeDamage(float amount)
    {
        throw new System.NotImplementedException();
    }

    public void TakeDamage(DamageInfo damageInfo)
    {
        Health -= damageInfo.Amount;
        if (Health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Instantiate(gorePrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    private void PerformDistantAttack()
    {
        // do sth
        if (!TryPrepareProjectileForest(out var projectileForest))
        {
            return;
        }
        DoShooting(projectileForest);
    }

    private bool TryPrepareProjectileForest(out List<IProjectileTreeNode> projectileForest)
    {
        var spellList = spells;

        if (!useInspectorSpellList)
        {
            spellList = CardInventory.Cards.Where(card => card != null).Select(card => card.Spell).ToList(); // crime agains humanity
        }

        projectileForest = projectileFactory.AssembleProjectileForest(spellList);

        if (projectileForest.Count == 0)
        {
            Debug.Log("insert spell please");
            return false;
        }
        return true;
    }

    private void DoShooting(List<IProjectileTreeNode> projectileForest)
    {
        foreach (var tree in projectileForest)
        {
            var instance = tree.InstantiateProjectile();
            if (instance.TryGetComponent<IProjectile>(out var projectile))
            {
                Vector3 shootDirection = transform.forward;
                Vector3 startPosition = transform.position + shootDirection * 0.5f;

                if (projectile is GunShot)
                {
                    (projectile as GunShot).SetVisibleRayBeginning(tipOfTheGun.position);
                }

                if (projectile.TryGetModificationInterface<IUserSecure>(out var userSecure))
                {
                    userSecure.EnsureProtectionOfObjectWith(gameObject.GetInstanceID());
                }

                projectile.Fire(startPosition, shootDirection);
            }
            else
            {
                throw new System.Exception("WTF?! Instance is not a projectile. Thats forbidden by law!");
            }
        }
    }

    public void DropOut()
    {
        transform.parent = null;
        transform.rotation = Quaternion.identity;

        avatar.SetActive(true);

        colliderForDetection.enabled = true;

        stateMachine.ChangeState(new UnhandledAttackMimicState(this));
    }

    public Sprite GetItemAvatarSprite()
    {
        return sprite;
    }

    public void OnSelect()
    {
        avatar.SetActive(true);
        stateMachine.ChangeState(new HandledAttackMimicState(this));
    }

    public void OnUnselect()
    {
        avatar.SetActive(false);
        stateMachine.ChangeState(new SleepingPistolMimicState(this));
    }

    public void SetUser(IUser user)
    {
        this.user = user;
        colliderForDetection.enabled = false;
        transform.parent = user.CameraTransform;
        transform.localPosition = Vector3.zero + new Vector3(0.5f, -0.1f, 1f);

        transform.rotation = user.CameraTransform.rotation;
        transform.Rotate(Vector3.right * 180, Space.Self);

        stateMachine.ChangeState(new HandledAttackMimicState(this));
    }

    public bool TryUsePrimaryAction()
    {
        // nothing happens
        return false;
    }

    public bool TryUseSecondaryAction()
    {
        // nothing happens
        return false;
    }

    private class SleepingPistolMimicState : IState
    {
        private PistolMimic mimic;

        public SleepingPistolMimicState(PistolMimic pistolMimic)
        {
            mimic = pistolMimic;
        }

        public void Execute()
        {
            // do nothing until picked up
        }
    }

    private class HandledAttackMimicState : IState
    {
        private float timeLeftUntilAttack = 0.6f;

        private PistolMimic mimic;

        public HandledAttackMimicState(PistolMimic pistolMimic)
        {
            mimic = pistolMimic;
        }

        public void Execute()
        {
            timeLeftUntilAttack -= Time.deltaTime;
            if (timeLeftUntilAttack <= 0)
            {
                // attack here
                mimic.animator.SetTrigger("TrRecoil");
                if (mimic.user.UserGameObject.TryGetComponent<IHurtable>(out var hurtable))
                {
                    hurtable.TakeDamage(mimic.closeRangeDamage);
                }

                mimic.stateMachine.ChangeState(new HandledAttackMimicState(mimic));
            }
        }
    }

    private class UnhandledAttackMimicState : IState
    {
        private float timeLeftUntilAttack = 2f;

        private float rotationSpeedParameter = 5f;      // seems that this is not speed, but they correlate

        private PistolMimic mimic;

        public UnhandledAttackMimicState(PistolMimic pistolMimic)
        {
            mimic = pistolMimic;
        }

        public void Execute()
        {
            if (mimic.user == null)
            {
                mimic.stateMachine.ChangeState(new SleepingPistolMimicState(mimic));
                return;
            }

            //rotate
            var dir = mimic.user.SelfTransform.position - mimic.transform.position;
            if (dir.magnitude > 0)
            {
                var rotation = Quaternion.LookRotation(dir);
                mimic.transform.rotation = Quaternion.Lerp(mimic.transform.rotation, rotation, rotationSpeedParameter * Time.deltaTime);
            }

            timeLeftUntilAttack -= Time.deltaTime;
            if (timeLeftUntilAttack <= 0)
            {
                // attack
                mimic.animator.SetTrigger("TrRecoil");
                mimic.PerformDistantAttack();
                mimic.stateMachine.ChangeState(new UnhandledAttackMimicState(mimic));
            }
        }
    }
}
