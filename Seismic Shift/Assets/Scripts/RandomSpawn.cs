using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSpawn : MonoBehaviour {

    public float width;
    public float length;
    public float spawnRate;
    public GameObject obstacle;

    Transform pos;
    bool spawn = false;
    float spawnDelay = 0;


	// Use this for initialization
	void Start () {
        pos = transform;
	}
	
	// Update is called once per frame
	void Update () {
        if (spawn)
        {
            if (spawnDelay > 0)
            {
                spawnDelay -= Time.deltaTime;
            }
            else
            {
                GameObject obj = Instantiate(obstacle, transform.position, transform.rotation);
                obj.transform.position += new Vector3(Random.Range(-width / 2, width / 2), 0, Random.Range(-length / 2, length / 2));
                spawnDelay += spawnRate;
            }
        }

        if(Input.GetKeyDown(KeyCode.T))
        {
            spawn = true;
        }

	}        

    public void Triggered()
    {
        spawn = true;
    }
}
