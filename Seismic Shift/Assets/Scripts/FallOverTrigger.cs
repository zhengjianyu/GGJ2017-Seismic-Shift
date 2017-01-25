using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallOverTrigger : MonoBehaviour
{
    public GameObject building;
    public GameObject goal;
    Quaternion initRotation;
    Quaternion finalRotation;
    Vector3 initPosition;
    Vector3 finalPosition;
    bool alreadyplayed = false;

    // Use this for initialization
    void Start()
    {
        initPosition = building.transform.position;
        initRotation = building.transform.rotation;
        
        finalRotation = goal.transform.rotation;
        finalPosition = goal.transform.position;
    }

    IEnumerator FallOverAnimation()
    {
        float t = 0;

        while (t < 1)
        {
            building.transform.rotation = Quaternion.Slerp(initRotation, finalRotation, t);
            building.transform.position = Vector3.Lerp(initPosition, finalPosition, t);
            t+=0.01f;
            yield return 0;
        }
    }

    void OnTriggerEnter(Collider x)
    {
        if (!alreadyplayed && x.tag == "Player")
        {
            //Audio.PlayOneShot(FallingSound, volume);
            alreadyplayed = true;
            StartCoroutine(FallOverAnimation());
        }
    }
}