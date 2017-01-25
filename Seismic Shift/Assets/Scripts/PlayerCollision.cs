using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;

public class PlayerCollision : MonoBehaviour {

	public int playerhealth = 3;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Obstacle"))
        {
			playerhealth -= 1;
			Debug.Log ("HIT");
			if (playerhealth == 0) 
			{
				GetComponent<CarUserControl>().setInput(false);
				GetComponent<Rigidbody>().velocity = Vector3.zero;
				Debug.Log ("DEAD");
			}
        }
    }
 
}
