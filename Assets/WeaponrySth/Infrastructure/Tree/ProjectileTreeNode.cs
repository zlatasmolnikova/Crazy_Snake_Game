using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProjectileTreeNode : IProjectileTreeNode
{
    public IProjectileTreeNode Parent { get; }

    private List<IProjectileTreeNode> children = new List<IProjectileTreeNode>();
    public IEnumerable<IProjectileTreeNode> Children => children;

    public IEnumerable<IModifier> Modifiers => _modifiers;

    private List<IModifier> _modifiers = new List<IModifier>();

    private readonly Instantiator instantiator;

    private readonly GameObject projectilePrefab;

    public event Action<IProjectileInfo> OnProjectileEvent;

    public ProjectileTreeNode(GameObject projectilePrefab, Instantiator instantiator,
        IProjectileTreeNode parent = null)
    {
        this.projectilePrefab = projectilePrefab;
        this.instantiator = instantiator;
        Parent = parent;
    }

    public void AddModifiers(IEnumerable<IModifier> modifiers)
    {
        
        foreach (var mod in modifiers)
        {
            _modifiers.Add(mod);
        }
    }

    public void AddModifier(IModifier modifier)
    {
        _modifiers.Add(modifier);
    }

    public GameObject InstantiateProjectile()
    {
        var resultGameobject = instantiator.InstantiatePrefab(projectilePrefab);
        if (resultGameobject.TryGetComponent<IModifiable>(out var resultModifiable))
        {
            // this part might be changed if we want modifiers to "modify" modifiers
            foreach (var modifier in _modifiers)
            {
                modifier.TryModify(resultModifiable);
            }
            resultModifiable.TryGetModificationInterface<ISubscriptable>(out var subscriptable);
            //Debug.Log(subscriptable);
            subscriptable.OnProjectileEvent += OnProjectileEvent;
        }
        return resultGameobject;
    }

    public GameObject InstantiateProjectile(Transform parent)
    {
        var result = InstantiateProjectile();
        result.transform.parent = parent;
        return result;
    }

    public GameObject InstantiateProjectile(Vector3 position)
    {
        var result = InstantiateProjectile();
        result.transform.position = position;
        return result;
    }

    public void AddChild(IProjectileTreeNode child)
    {
        children.Add(child);
    }
}
