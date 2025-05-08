using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IModifiable
{
    public bool TryGetModificationInterface<T>(out T modifiable) where T : class;
}
