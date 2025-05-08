using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// why it inherits ISubscriptable? -> to use same interface !
public interface IProjectileTreeNode : ISubscriptable
{
    public IProjectileTreeNode Parent { get; }

    public IEnumerable<IProjectileTreeNode> Children { get; }

    public void AddChild(IProjectileTreeNode child);

    public void AddModifier(IModifier modifier);

    public IEnumerable<IModifier> Modifiers { get; }

    public GameObject InstantiateProjectile();

    public GameObject InstantiateProjectile(Transform parent);

    public GameObject InstantiateProjectile(Vector3 position);
}
