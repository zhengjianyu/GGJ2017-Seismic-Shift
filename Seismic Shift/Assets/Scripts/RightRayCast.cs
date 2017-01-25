using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightRayCast : MonoBehaviour {

    private float range = 20f;

    Ray RightDestructRay;
    RaycastHit RightDestructHit;
    private int destructibleMask;

    void Awake()
    {
        destructibleMask = LayerMask.GetMask("Destructible");
    }

    void FixedUpdate()
    {
        Ray();
    }

    void Ray()
    {
        RightDestructRay.origin = transform.position;
        RightDestructRay.direction = transform.forward;

        if (Physics.Raycast(RightDestructRay, out RightDestructHit, range, destructibleMask))
        {
            RightDestructHit.transform.SendMessage("HitByRay");
        }
    }
}
