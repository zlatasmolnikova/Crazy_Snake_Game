using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILocationInfo
{
    public Vector3 Position { get; }

    public Vector3 Direction { get; }
}
