using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pad : MonoBehaviour
{
    public bool _mouseIn;

    private void OnMouseEnter()
    {
        _mouseIn = true;
    }
    private void OnMouseExit()
    {
        _mouseIn = false;
    }

    void Update()
    {
        if (Input.GetKeyUp(0))
        {
            Game.Instance.ProppelerController.CallTheBallBack();
        }
    }
}
