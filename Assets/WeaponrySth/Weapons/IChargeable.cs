using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// does not mean that right click == recharge; means that it has some charge that can be displayed
/// </summary>
public interface IChargeable
{
    public ChargeInfo ChargeInfo { get; }

    public event Action<ChargeInfo> OnChargeChanged;
}
