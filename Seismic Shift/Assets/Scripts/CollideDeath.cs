using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;

public class CollideDeath : MonoBehaviour {

    public float speedThreshold;
    public int damage;

    bool crashOnce = true;

	void OnTriggerEnter(Collider collider)
    {
        if(collider.gameObject.tag == "Player" || (collider.gameObject.tag == "AICar" && collider.GetType() == typeof(SphereCollider)))
        {
            if(collider.gameObject.GetComponent<Rigidbody>().velocity.magnitude > speedThreshold)
            {
                //Collision death 
                Instantiate(Resources.Load("Explosion01"), transform.position, transform.rotation);
                
                if (collider.gameObject.tag == "Player" && crashOnce)
                {
                    //collider.gameObject.GetComponent<CarUserControl>().setInput(false);
                    //collider.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                    //Debug.Log("Collided");
                    crashOnce = false;
                    collider.gameObject.GetComponent<PlayerResources>().takeDamage(damage);


                }
                    
                else if (collider.gameObject.tag == "AICar")
                    collider.gameObject.GetComponent<AICar>().Brake();
                collider.gameObject.GetComponent<CarController>().Move(0, 0, 1, 0);
            }
        }
    }
}
