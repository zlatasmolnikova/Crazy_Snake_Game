using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Experimental.GraphView.GraphView;

public class LaggyPistol : MonoBehaviour, ICardBasedItem, IChargeable
{
    [SerializeField]
    private GameObject onSceneAvatar;

    [SerializeField]
    private GameObject inHandAvatar;
    
    [SerializeField]
    private Sprite sprite;

    [SerializeField]
    private Transform tipOfTheGun;

    // temporary (before GUI is in use)
    [SerializeField]
    private List<Spell> spells;

    public CardInventory CardInventory { get; private set; }

    public ChargeInfo ChargeInfo { get; private set; } 

    public event Action<ChargeInfo> OnChargeChanged;

    private ProjectileFactory projectileFactory;

    private IUser user;

    private Collider colliderForDetection;

    private SoundController soundController;

    private Animator animator;

    private float cooldown = 1f / 5f;
    private float lastShotTime;

    private Vector3 inHandAvatarInitialEulerAngles;

    [SerializeField]
    private bool useInspectorSpellList = true;

    // animation ruling logic
    private bool recharging = false;
    private bool shooting = false;
    private Coroutine rechargeCoroutine;
    private Coroutine shootCoroutine;
    private float rechargeTime = 5;
    // end

    private void Awake()
    {
        colliderForDetection = GetComponent<Collider>();

        CardInventory = new CardInventory();    //yep. empty and with full capasity

        this.ChargeInfo = new ChargeInfo(10);
    }

    private void Start()
    {
        animator = inHandAvatar.GetComponent<Animator>();

        inHandAvatarInitialEulerAngles = inHandAvatar.transform.localEulerAngles;

        var soundControllerObject = GameObject.FindGameObjectWithTag("SoundController");
        if (soundControllerObject == null || !soundControllerObject.TryGetComponent<SoundController>(out soundController))
        {
            Debug.LogWarning("Sound Controller not found - sounds may not work");
        }

        var projectileFactoryGameObject = GameObject.FindGameObjectWithTag("ProjectileFactory");
        if (projectileFactoryGameObject == null || !projectileFactoryGameObject.TryGetComponent(out projectileFactory))
        {
            throw new System.Exception("weapon needs a projectile factory on scene; (use prefab)");
        }
    }

    public void DropOut()
    {
        transform.parent = null;
        transform.eulerAngles = Vector3.zero;
        onSceneAvatar.SetActive(true);
        colliderForDetection.enabled = true;
        OnChargeChanged = null; // to prevent "memory leaks" (probably not needed)
    }

    public void OnSelect()
    {
        EnsureInHandAvatarPosision();
        inHandAvatar.SetActive(true);
    }

    private void EnsureInHandAvatarPosision()
    {
        //animator.StopPlayback();
        transform.localPosition = Vector3.zero + new Vector3(0.2f, -0.2f, 1f);

        transform.forward = user.CameraTransform.forward;

        inHandAvatar.transform.localPosition = Vector3.zero;     // to remove movement caused by animation
        inHandAvatar.transform.localEulerAngles = inHandAvatarInitialEulerAngles;

    }

    public void OnUnselect()
    {
        inHandAvatar.SetActive(false);
        AbortRecharge();
    }

    public void SetUser(IUser user)
    {
        colliderForDetection.enabled = false;
        this.user = user;
        onSceneAvatar.SetActive(false);

        transform.parent = user.CameraTransform;
        EnsureInHandAvatarPosision();
    }

    public bool TryUsePrimaryAction()
    {
/*        if (Time.time - lastShotTime < cooldown)
            return false;*/

        if (recharging || shooting)
            return false;

        if (ChargeInfo.CurrentCharge <= 0)
        {
            Debug.Log("no more charges");
            return false;
        }

        EnsureInHandAvatarPosision();

/*        var spellList = spells;
        if (CardInventory.Count > 0)
        {
            spellList = CardInventory
                        .Cards
                        .Where(c => c != null)
                        .Select(c => c.Spell)
                        .ToList();
        }*/

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

        foreach (var tree in projectileForest)
        {
            if (tree == null)
            {
                Debug.Log("insert spell please");
                return false;
            }

            var instance = tree.InstantiateProjectile();
            if (instance.TryGetComponent<IProjectile>(out var projectile))
            {
                Vector3 shootDirection = user.CameraTransform.forward;
                var delta = (user.CameraTransform.right - user.CameraTransform.up) * 0.02f;
                Vector3 startPosition = user.CameraTransform.position + shootDirection * 0.1f;

                if (projectile is GunShot)
                {
                    (projectile as GunShot).SetVisibleRayBeginning(tipOfTheGun.position);
                }

                projectile.Fire(startPosition, shootDirection, user.Velocity);

                //if (soundController != null)
                //{
                //    soundController.PlaySound("PistolShot", startPosition + user.CameraTransform.forward, 0.8f);
                //}
            }
            else
            {
                throw new System.Exception("WTF?! Instance is not a projectile. Thats forbidden by law!");
            }
        }

        // this is "feature"
        shootCoroutine = StartCoroutine(ShootDelayPerform(projectileForest.Count));
        
        return true;
    }

    public bool TryUseSecondaryAction()
    {
        Debug.Log("Shoot yourself if the leg. Now. And Recharge :D. Slowly");
        if (shooting || recharging)
        {
            return false;
        }
        rechargeCoroutine = StartCoroutine(RefillCharge());
        return true;
    }

    private IEnumerator RefillCharge()
    {
        recharging = true;
        animator.SetTrigger("TrReload");
        yield return new WaitForSeconds(rechargeTime);
        recharging = false;
        ChargeInfo.CurrentCharge = ChargeInfo.MaxCharge;
        OnChargeChanged?.Invoke(ChargeInfo);
        rechargeCoroutine = null;
    }

    private IEnumerator ShootDelayPerform(int projectileCount)
    {
        shooting = true;
        animator.SetTrigger("IsShooting");
        ChargeInfo.CurrentCharge -= 1;
        Debug.Log($"shot performed; cur charge = {ChargeInfo.CurrentCharge}");
        OnChargeChanged?.Invoke(ChargeInfo);
        yield return new WaitForSeconds(cooldown);
        shooting = false;
        shootCoroutine = null;
    }

    private void AbortRecharge()
    {
        if (recharging)
        {
            StopCoroutine(rechargeCoroutine);
            rechargeCoroutine = null;
            recharging = false;
        }
    }

    public Sprite GetItemAvatarSprite() => sprite;

    public void SetCards(CardInventory cardInventory)
    {
        this.CardInventory = cardInventory;
    }

    public void SetUseCardsFromInventory(bool useCardsFromInventory)
    {
        useInspectorSpellList = !useCardsFromInventory;
    }
}
