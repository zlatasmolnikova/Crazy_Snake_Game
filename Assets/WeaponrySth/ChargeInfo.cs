using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeInfo
{
    public int CurrentCharge { get; set; }

    public int MaxCharge { get; set; }

    public ChargeInfo(int maxCharge)
    {
        MaxCharge = maxCharge;
        CurrentCharge = maxCharge;
    }
}
