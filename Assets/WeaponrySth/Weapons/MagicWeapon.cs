using System.Collections.Generic;
using UnityEngine;

public class MagicWeapon : MonoBehaviour, IInventoryItem
{
    private IUser user;

    private Collider colliderForDetection;

    private ItemAvatar avatar;

    public List<string> Spells;

    public GameObject GunShotPrefab;

    public GameObject ExplosionPrefab;

    public GameObject GrenadePrefab;

    public Instantiator Instantiator;     // this one needed to create instances of spells

    public void Awake()
    {
        colliderForDetection = GetComponent<Collider>();
        avatar = GetComponent<ItemAvatar>();
    }

    public void DropOut()
    {
        transform.parent = null;
        colliderForDetection.enabled = true;
        gameObject.SetActive(true); //to lazy to lay my hands on actual model.
                                    //Probably should have used MeshRenderer component
        avatar.enabled = true;
    }

    public void OnSelect()
    {
        gameObject.SetActive(true);
    }

    public void OnUnselect()
    {
        gameObject.SetActive(false);
    }

    public void SetUser(IUser user)
    {
        avatar.enabled=false;
        colliderForDetection.enabled=false;
        this.user = user;
        gameObject.SetActive(true);
        transform.parent = user.HandTransform;
        transform.localPosition = Vector3.zero;
        transform.localEulerAngles = Vector3.zero;
    }

    public bool TryUsePrimaryAction()
    {
        var instance = AssembleSpellTree()?.InstantiateProjectile();

        if (instance == null)
        {
            Debug.Log("insert spell please");
            return false;
        }

        if (instance.TryGetComponent<IProjectile>(out var projectile))
        {
            Vector3 shootDirection = user.CameraTransform.forward;
            Vector3 startPosition = user.CameraTransform.position + shootDirection * 0.1f;
            projectile.Fire(startPosition, shootDirection, user.Velocity);
        }
        else
        {
            throw new System.Exception("WTF?! Instance is not a projectile. Thats forbidden by law!");
        }
        return true;
    }

    private IProjectileTreeNode AssembleSpellTree()
    {
        ProjectileTreeNode parentNode = null;
        ProjectileTreeNode root = null;
        foreach (var spell in Spells)
        {
            var projectilePrefab = GetProjectilePrefab(spell);

            if (projectilePrefab != null)
            {
                var nextNode = new ProjectileTreeNode(projectilePrefab, Instantiator, parentNode);
                if (parentNode != null)
                {
                    parentNode.OnProjectileEvent += (info) =>
                    {
                        if (info.TryGetProjectileInfo<IHitSomethingInfo>(out var hitSomethingInfo))
                        {
                            var instance = nextNode.InstantiateProjectile();
                            var projectile = instance.GetComponent<IProjectile>();

                            var d = hitSomethingInfo.ProjectileDirection.normalized;
                            var n = hitSomethingInfo.SurfaceNormal.normalized;

                            var reflectionDirection = d - 2 * Vector3.Dot(d, n) * n;    // applied algebra
                            projectile.Fire(hitSomethingInfo.Position, reflectionDirection.normalized);
                        }
                        else if (info.TryGetProjectileInfo<IExpirationInfo>(out var expirationInfo))
                        {
                            var instance = nextNode.InstantiateProjectile();
                            var projectile = instance.GetComponent<IProjectile>();
                            projectile.Fire(expirationInfo.Position, expirationInfo.ProjectileDirection);
                        }
                    };
                }

                parentNode = nextNode;

                if (root == null)
                {
                    root = parentNode;
                }

                continue;
            }

            var modifier = GetModifier(spell);
            if (modifier != null)
            {
                parentNode.AddModifiers(new[] {modifier});      //kinda bad
                continue;
            }

            throw new System.Exception($"spell {spell} not found");
        }
        return root;
    }

    public GameObject GetProjectilePrefab(string spell)
    {
        if (spell.ToLower() == "gunshot")
        {
            return GunShotPrefab;
        }
        else if (spell.ToLower() == "explosion")
        {
            return ExplosionPrefab;
        }
        else if (spell.ToLower() == "grenade")
        {
            return GrenadePrefab;
        }

        return null;
    }

    public IModifier GetModifier(string spell)
    {
        if (spell.ToLower() == "piercing")
        {
            return new PiercingModifier();
        }
        if (spell.ToLower() == "damageincrease")
        {
            return new ConstantDamageIncreaseModifier();
        }
        if (spell.ToLower() == "bouncyincrease")
        {
            return new BouncyModifier();
        }

        return null;
    }

    public bool TryUseSecondaryAction()
    {
        Debug.Log("action never took place. Or did it?");
        return true;
    }

    public Sprite GetItemAvatarSprite()
    {
        throw new System.NotImplementedException();
    }
}
