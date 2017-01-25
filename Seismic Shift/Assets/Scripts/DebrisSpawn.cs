using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebrisSpawn : MonoBehaviour {

	public GameObject debrisObstacle;
    public float spawnRate = 6f;

    bool debrisSpawn = false;
    float spawnDelay = 0f;

    public void Update()
    {
        if(debrisSpawn)
        {
            Debug.Log("Lol");
            if (spawnDelay > 0)
            {
                spawnDelay -= Time.deltaTime;
            }
            else
            {
                GameObject obj = Instantiate(debrisObstacle, transform.position, transform.rotation);
                spawnDelay += spawnRate;
            }
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            debrisSpawn = true;
        }
    }
    public void Debris()
    {
        Debug.Log("yo");
        debrisSpawn = true;
    }

}
