using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building1RayDetect : MonoBehaviour {

    public GameObject DebrisTrigger;

    void HitByRay () {
        Debug.Log("I was hit by a ray");
        DebrisTrigger.GetComponent<DebrisSpawn>().Debris();
    }
}
