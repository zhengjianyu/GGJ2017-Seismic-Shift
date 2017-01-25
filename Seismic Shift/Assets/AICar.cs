using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;

public class AICar : MonoBehaviour {

    CarController controller;
    bool move;
    bool brake;
    float brakeDirection;

	// Use this for initialization
	void Start () {
        controller = GetComponent<CarController>();
	}
	
	// Update is called once per frame
	void Update () {
        if(brake)
        {
            controller.Move(brakeDirection, 0, 1, 1);
        }
		else if(move)
        {
            controller.Move(0, 1, 0, 0);
        }
	}

    void OnTriggerEnter(Collider collider)
    {
        if(collider.gameObject.tag == "Obstacle" || collider.gameObject.tag == "Player")
        {
            brakeDirection = Random.Range(-0.5f, 0.5f);
            brake = true;
        }
    }

    public void Drive()
    {
        move = true;
    }

    public void Brake()
    {
        brakeDirection = Random.Range(-0.5f, 0.5f);
        brake = true;
    }
}
