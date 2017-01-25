using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftRayCast : MonoBehaviour {

    private float range = 20f;

    Ray LeftDestructRay;
    RaycastHit LeftDestructHit;
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
        LeftDestructRay.origin = transform.position;
        LeftDestructRay.direction = transform.forward;

        if (Physics.Raycast(LeftDestructRay, out LeftDestructHit, range, destructibleMask))
        {
            LeftDestructHit.transform.SendMessage("HitByRay");
        }
    }
}
