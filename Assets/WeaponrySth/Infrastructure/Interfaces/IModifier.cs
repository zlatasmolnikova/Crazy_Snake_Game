using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IModifier
{
    public bool TryModify(IModifiable modifiable);
}
