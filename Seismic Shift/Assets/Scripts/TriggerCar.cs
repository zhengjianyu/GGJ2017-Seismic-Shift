using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerCar : MonoBehaviour {

    public GameObject prefab;

	void OnTriggerEnter(Collider collider)
    {
        if(collider.gameObject.tag == "Player")
        {
            prefab.GetComponent<GoToward>().Triggered();
            Destroy(gameObject);
        }
    }
}
