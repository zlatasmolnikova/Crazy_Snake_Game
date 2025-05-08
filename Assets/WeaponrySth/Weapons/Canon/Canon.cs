using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Canon : MonoBehaviour, ICardBasedItem, IChargeable
{
    [SerializeField]
    private List<Spell> spells;

    [SerializeField]
    private bool useInspectorSpellList;

    public CardInventory CardInventory { get; private set; } = new CardInventory(10);

    public ChargeInfo ChargeInfo { get; private set; } = new ChargeInfo(1);

    public event Action<ChargeInfo> OnChargeChanged;

    private Collider canonCollider;

    private ProjectileFactory projectileFactory;

    private IUser user;

    [SerializeField]
    private Texture2D spriteTexture;

    private Sprite sprite;

    [SerializeField]
    private GameObject avatar;

    [SerializeField]
    private Renderer indicatorRenderer;

    private Animator animator;

    [SerializeField]
    private Transform tipOfTheCanon;

    private readonly float rechargeTime = 0.834f + 1.5f; //do not change - it is in sync with animation clip

    private readonly float meleeDamageDelay = 0.02f;
    private readonly float meleeAttackTime = 0.833f;  // same here

    private bool performingMeleeAttack = false;

    private readonly float pushImpulse = 50;

    private readonly float meleeDamage = 10;

    private void Awake()
    {
        canonCollider = GetComponent<Collider>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        var projectileFactoryGameObject = GameObject.FindGameObjectWithTag("ProjectileFactory");
        if (projectileFactoryGameObject == null || !projectileFactoryGameObject.TryGetComponent(out projectileFactory))
        {
            throw new System.Exception("weapon needs a projectile factory on scene; (use prefab)");
        }

        if (avatar == null)
        {
            throw new Exception($"avatar for {name} not set");
        }

        if (spriteTexture == null)
        {
            throw new Exception("sprite texture not found");
        }

        sprite = Sprite.Create(spriteTexture,
            new Rect(0.0f, 0.0f, spriteTexture.width, spriteTexture.height),
                new Vector2(0.5f, 0.5f)
        );

        if (indicatorRenderer == null)
        {
            throw new Exception("indicator not set");
        }

        if (tipOfTheCanon == null)
        {
            throw new Exception("tip on the cannon not set");
        }
    }

    public void DropOut()
    {
        transform.parent = null;
        transform.rotation = Quaternion.identity;

        avatar.SetActive(true);

        canonCollider.enabled = true;

        OnChargeChanged = null;

        user = null;
    }

    public Sprite GetItemAvatarSprite()
    {
        return sprite;
    }

    public void OnSelect()
    {
        avatar.SetActive(true);
        EnsureInHandPosition();
    }

    public void OnUnselect()
    {
        avatar.SetActive(false);
    }

    public void SetUser(IUser user)
    {
        canonCollider.enabled = false;

        this.user = user;
        transform.parent = user.CameraTransform;

        avatar.SetActive(true);
        EnsureInHandPosition();
    }

    public bool TryUsePrimaryAction()
    {
        var spellList = spells;

        if (!useInspectorSpellList)
        {
            spellList = CardInventory.Cards.Where(card => card != null).Select(card => card.Spell).ToList(); // crime agains humanity
        }

        var projectileForest = projectileFactory.AssembleProjectileForest(spellList);

        if (projectileForest.Count == 0)
        {
            Debug.Log("insert spell please");
            return false;
        }

        if (ChargeInfo.CurrentCharge <= 0 || performingMeleeAttack)
        {
            return false;
        }
        ChargeInfo.CurrentCharge -= 1;
        OnChargeChanged?.Invoke(ChargeInfo);
        StartCoroutine(StartFireAndRechargeAnimationAndCountdown());

        foreach (var tree in projectileForest)
        {
            var instance = tree.InstantiateProjectile();
            if (instance.TryGetComponent<IProjectile>(out var projectile))
            {
                Vector3 shootDirection = user.CameraTransform.forward;
                Vector3 startPosition = user.CameraTransform.position + shootDirection * 0.1f;

                if (projectile.TryGetModificationInterface<IUserSecure>(out var userSecure))
                {
                    userSecure.EnsureProtectionOfObjectWith(user.UserGameObject.GetInstanceID());
                }

                projectile.Fire(startPosition, shootDirection, user.Velocity);
            }
            else
            {
                throw new Exception("WTF?! Instance is not a projectile. Thats forbidden by law!");
            }
        }

        return true;
    }

    public bool TryUseSecondaryAction()
    {
        if (ChargeInfo.CurrentCharge <= 0 || performingMeleeAttack)
        {
            return false;
        }

        performingMeleeAttack = true;

        StartCoroutine(StartMeleeAttack());
        
        return true;
    }

    private IEnumerator StartMeleeAttack()
    {
        animator.SetTrigger("TrMeleeAttack");
        indicatorRenderer.material.color = Color.blue;
        yield return new WaitForSeconds(meleeDamageDelay);
        DoMeleeAttackDamage();
        yield return new WaitForSeconds(meleeAttackTime - meleeDamageDelay);
        indicatorRenderer.material.color = Color.green;
        performingMeleeAttack = false;
    }

    private void DoMeleeAttackDamage()
    {
        var length = 4;
        var width = 1.3f;
        var height = 1.3f;

        var boxCenter = tipOfTheCanon.position;
        var halfExdends = new Vector3(width / 2, height / 2, length / 2);

        foreach (var collider in Physics.OverlapBox(boxCenter, halfExdends, user.CameraTransform.rotation, LayersStorage.PossiblyHurtables))
        {
            if (user.UserGameObject == collider.gameObject)
            {
                continue;
            }
            if (collider.gameObject.TryGetComponent<IHurtable>(out var hurtable))
            {
                hurtable.TakeDamage(new DamageInfo(meleeDamage, DamageType.MeleeDamage).WithImpulse(user.CameraTransform.forward * pushImpulse));
            }
            if (collider.gameObject == null)
            {
                continue;
            }
            Debug.Log(collider.gameObject);
            if (collider.gameObject.TryGetComponent<IPushable>(out var pushable))
            {
                pushable.Push(user.CameraTransform.forward * pushImpulse);
            }
            else if (collider.gameObject.TryGetComponent<Rigidbody>(out var rb))
            {
                Debug.Log("here");
                rb.AddForce(user.CameraTransform.forward * pushImpulse, ForceMode.Impulse);
            }
        }
    }

    private IEnumerator StartFireAndRechargeAnimationAndCountdown()
    {
        indicatorRenderer.material.color = Color.red;
        animator.SetTrigger("TrPrimaryFire");
        yield return new WaitForSeconds(rechargeTime);
        ChargeInfo.CurrentCharge = ChargeInfo.MaxCharge;
        indicatorRenderer.material.color = Color.green;
        OnChargeChanged?.Invoke(ChargeInfo);
    }

    private void EnsureInHandPosition()
    {
        transform.forward = user.CameraTransform.forward;
        transform.localPosition = new Vector3(0.6f, -0.6f, 1f);
    }

    public void SetCards(CardInventory cardInventory)
    {
        this.CardInventory = cardInventory;
    }

    public void SetUseCardsFromInventory(bool useCardsFromInventory)
    {
        useInspectorSpellList = !useCardsFromInventory;
    }
}
