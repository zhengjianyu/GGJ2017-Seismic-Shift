using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.Vehicles.Car;

public class GoToward : MonoBehaviour {

    public GameObject dest;

    CarController car;
    bool triggered = false;

	// Use this for initialization
	void Start () {
        car = GetComponent<CarController>();
    }
	
	// Update is called once per frame
	void Update () {
		
        if(triggered)
        {
            
            car.Move(0, 1, 0, 0);
        }
	}

    public void Triggered()
    {
        triggered = true; 
    }
}
