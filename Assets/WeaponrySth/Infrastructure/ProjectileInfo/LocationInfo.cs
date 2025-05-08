using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationInfo : ILocationInfo
{
    public LocationInfo(Vector3 posion, Vector3 direction)
    {
        Position = posion;
        Direction = direction;
    }

    public Vector3 Position { get; set; }

    public Vector3 Direction { get; set; }
}
