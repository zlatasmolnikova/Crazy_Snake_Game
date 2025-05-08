using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ProjectileFactory : MonoBehaviour
{
    private Instantiator instantiator;

    /*[SerializeField]
    private List<GameObject> projectilePrefabs;    // no new() here bc unity will replace it*/

    [SerializeField]
    private GameObject explosionPrefab;

    [SerializeField]
    private GameObject gunShotPrefab;

    [SerializeField]
    private GameObject grenadePrefab;

    [SerializeField]
    private GameObject trackerPrefab;

    [SerializeField]
    private GameObject canonBallPrefab;

    private readonly Dictionary<Spell, GameObject> spellToPrefabMap = new();

    private void Awake()
    {
        if (!TryGetComponent<Instantiator>(out instantiator))
        {
            throw new System.Exception("Projectile factory needs instantiator");
        }
    }

    private void Start()
    {
        spellToPrefabMap.Add(Spell.Explosion, explosionPrefab);
        spellToPrefabMap.Add(Spell.GunShot, gunShotPrefab);
        spellToPrefabMap.Add(Spell.Grenade, grenadePrefab);
        spellToPrefabMap.Add(Spell.Tracker, trackerPrefab);
        spellToPrefabMap.Add(Spell.CanonBall, canonBallPrefab);

        /*foreach (var prefab in projectilePrefabs)
        {
            if (prefab.TryGetComponent<IProjectile>(out var proj))
            {
                var spell = proj.GetType().Name.ToLower();
                spellToPrefabMap.Add(spell, prefab);
            }
            else
            {
                throw new System.Exception($"prefab {prefab} is not a projectile");
            }
        }*/
    }

    private List<IProjectileTreeNode> AssembleForest(List<Spell> spells)
    {
        IProjectileTreeNode fakeRoot = new ProjectileTreeNode(null, null, null);
        var curNode = fakeRoot;
        foreach (var spell in spells)
        {
            if (spell == Spell.AscendTree)
            {
                if (curNode != fakeRoot)
                {
                    curNode = curNode.Parent;
                }
                continue;
            }
            else if (spell == Spell.AscendTreeTwice)
            {
                if (curNode != fakeRoot)
                {
                    curNode = curNode.Parent;
                }
                if (curNode != fakeRoot)
                {
                    curNode = curNode.Parent;
                }
                continue;
            }
            
            if (TryResolveProjectileSpellToPrefab(spell, out var projectilePrefab))
            {
                if (projectilePrefab == null)
                {
                    throw new System.Exception($"spell not resolved {spell}");
                }
                //Debug.Log(projectilePrefab);
                var nextNode = new ProjectileTreeNode(projectilePrefab, instantiator, curNode);
                curNode.AddChild(nextNode);

                curNode.OnProjectileEvent += (info) =>
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

                curNode = nextNode;
                continue;
            }

            if (TryResolveModifierSpellToModifier(spell, out var modifier)) {
                curNode.AddModifier(modifier);
                continue;
            }

            throw new System.Exception($"spell {spell} not found");
        }

        return fakeRoot.Children.ToList();
    }

    public List<IProjectileTreeNode> AssembleProjectileForest(List<Spell> spells)
    {
        return AssembleForest(spells);
    }

/*    public IProjectileTreeNode AssembleProjectileTree(List<Spell> spells)
    {
        ProjectileTreeNode parentNode = null;
        ProjectileTreeNode root = null;
        foreach (var spell in spells)
        {
            var projectilePrefab = ResolveProjectileSpellToPrefab(spell);

            if (projectilePrefab != null)
            {
                var nextNode = new ProjectileTreeNode(projectilePrefab, instantiator, parentNode);
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

            var modifier = ResolveModifierSpellToModifier(spell);
            if (modifier != null)
            {
                parentNode.AddModifiers(new[] { modifier });      //kinda bad
                continue;
            }

            throw new System.Exception($"spell {spell} not found");
        }
        return root;
    }*/

    private GameObject ResolveProjectileSpellToPrefab(Spell spell)
    {
        if (spellToPrefabMap.TryGetValue(spell, out var prefab))
        {
            return prefab;
        }
        return null;
    }

    private bool TryResolveProjectileSpellToPrefab(Spell spell, out GameObject prefab)
    {
        //Debug.Log(spellToPrefabMap.TryGetValue(spell, out prefab));
        return spellToPrefabMap.TryGetValue(spell, out prefab);
    }

    private bool TryResolveModifierSpellToModifier(Spell spell, out IModifier modifier)
    {
        modifier = ResolveModifierSpellToModifier(spell);
        return modifier != null;
    }

    private IModifier ResolveModifierSpellToModifier(Spell spell)
    {
        if (spell == PiercingModifier.Spell)
        {
            return new PiercingModifier();
        }
        if (spell == ConstantDamageIncreaseModifier.Spell)
        {
            return new ConstantDamageIncreaseModifier();
        }
        if (spell == BouncyModifier.Spell)
        {
            return new BouncyModifier();
        }

        return null;
    }
}
