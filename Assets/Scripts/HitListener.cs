using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitListener : MonoBehaviour
{
    public delegate void OnHit();
    public delegate Block GetParentBlock();

    GetParentBlock GetParentBlockDelegate;
    OnHit OnHitDelegate;

    public void Init(OnHit onHit, GetParentBlock getParentBlock)
    {
        OnHitDelegate = onHit;
        GetParentBlockDelegate = getParentBlock;
    }

    void OnCollisionEnter(Collision collision)
    {
        OnHitDelegate();
    }

    internal Block GetBlock()
    {
        return GetParentBlockDelegate();
    }
}
