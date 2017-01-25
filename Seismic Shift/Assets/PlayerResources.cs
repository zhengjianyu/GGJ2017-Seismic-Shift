using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;

public class PlayerResources : MonoBehaviour {

    public int lives;

    public void takeDamage(int value)
    {
        if (lives - value > 0)
            lives -= value;
        else
        {
            lives = 0;
            gameObject.GetComponent<CarUserControl>().setInput(false);
            Instantiate(Resources.Load("LoopFire"), transform.position, transform.rotation);
            gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            Debug.Log("You died");
        }
            
    }

    void OnGUI()
    {
        GUI.Label(new Rect(10, 235, 100, 30), "Lives: " + lives);

    }
}
