using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class SingletonInputManager
{
    public static readonly SingletonInputManager instance = new();

    public readonly InputMap InputMap;

    private SingletonInputManager()
    {
        InputMap = new InputMap();
        InputMap.Enable();
    }
}
