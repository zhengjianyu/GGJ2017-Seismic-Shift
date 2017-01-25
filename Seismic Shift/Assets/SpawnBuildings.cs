using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;
using UnityEngine.AI;

public class SpawnBuildings : MonoBehaviour {

    public List<GameObject> buildings = new List<GameObject>();
    public GameObject explosionPrefab;
    public GameObject intersectionPrefab;
    public GameObject carPrefab;
    public GameObject sameCarPrefab;

    int[] table = new int[5];
    int[] explosionTable = new int[10];
    int[] intersectionTable = new int[3];
    int[] carTable = new int[3];
    int[] sameCarTable = new int[3];
    int[] crossTable = new int[2];

    public BoxCollider boxCol;

    /// <summary>
    /// When an objects exit's the tile
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerEnter(Collider other)
    {
        //If the player exits the tile
        if (other.tag == "Player")
        {
            for (int i = 0; i < table.Length; i++)
            {
                table[i] = 0;
            }
            for (int i = 0; i < explosionTable.Length; i++)
            {
                explosionTable[i] = 0;
            }
            for (int i = 0; i < carTable.Length; i++)
            {
                carTable[i] = 0;
                sameCarTable[i] = 0;
            }
            for (int i = 0; i < crossTable.Length; i++)
            {
                crossTable[i] = 0;
            }

            //Spawn buildings randomly
            GameObject buildingsSpawns = gameObject.transform.Find("SpawnPoints").transform.Find("BuildingSpawnPoints").gameObject;
            int spawnPoints = buildingsSpawns.transform.childCount;
            for (int i = 0; i < spawnPoints; i++)
            {
                bool check = true;
                while (check)
                {
                    int count = 1;
                    for (int j = 0; j < table.Length; j++)
                    {
                        count += table[j];
                    }
                    if (count == table.Length)
                    {
                        for (int j = 0; j < table.Length; j++)
                        {
                            table[j] = 0;
                        }
                    }
                    int val = Random.Range(0, table.Length);
                    if (table[val] == 0)
                    {
                        GameObject structure = Instantiate(buildings[val], buildingsSpawns.transform.GetChild(i).transform.position, transform.rotation);
                        structure.transform.SetParent(gameObject.transform);
                        table[val] = 1;
                        check = false;
                    }
                }

            }

            //Spawn explosions randomly
            GameObject explosionSpawns = gameObject.transform.Find("SpawnPoints").transform.Find("ExplosionSpawnPoints").gameObject;
            spawnPoints = explosionSpawns.transform.childCount;

            //TODO This is an example of how you could implement a difficulty slider
            int difficulty = 5;
            int numberOfExplosions = 0;

            for (int i = 0; i < spawnPoints; i++)
            {
                //Spawn an explosion 50% of the time on a new tile
                if ((Random.Range(0, 2) == 1) && (numberOfExplosions < difficulty))
                {
                    bool check = true;
                    while (check)
                    {
                        int val = Random.Range(0, explosionTable.Length);
                        if (explosionTable[val] == 0)
                        {
                            GameObject explosion = Instantiate(explosionPrefab, explosionSpawns.transform.GetChild(i).transform.position, transform.rotation);
                            explosion.transform.SetParent(gameObject.transform);
                            explosionTable[val] = 1;
                            check = false;
                        }
                    }
                }
            }

            //Spawn cars randomly
            GameObject carSpawns = gameObject.transform.Find("SpawnPoints").transform.Find("CarSpawnPoints").gameObject;
            spawnPoints = carSpawns.transform.childCount;
            for (int i = 0; i < spawnPoints; i++)
            {
                //Spawn an AI car 50% of the time on a new tile
                if (Random.Range(0, 2) == 1)
                {
                    bool check = true;
                    while (check)
                    {
                        int val = Random.Range(0, carTable.Length);
                        if (carTable[val] == 0)
                        {
                            GameObject car = (GameObject)Instantiate(carPrefab, carSpawns.transform.GetChild(i).transform.position, transform.rotation);
                            car.transform.SetParent(gameObject.transform);
                            car.transform.Rotate(new Vector3(0, 180, 0));
                            car.GetComponent<AICar>().Drive();
                            carTable[val] = 1;
                            check = false;
                        }
                    }
                }
            }

            //Spawn same-way car randomly
            GameObject sameSpawns = gameObject.transform.Find("SpawnPoints").transform.Find("ConstantCarSpawnPoints").gameObject;
            spawnPoints = sameSpawns.transform.childCount;
            for (int i = 0; i < spawnPoints; i++)
            {
                //Spawn an AI car 50% of the time on a new tile
                if (Random.Range(0, 2) == 1)
                {
                    bool check = true;
                    while (check)
                    {
                        int val = Random.Range(0, sameCarTable.Length);
                        if (sameCarTable[val] == 0)
                        {
                            GameObject car = (GameObject)Instantiate(carPrefab, sameSpawns.transform.GetChild(i).transform.position, transform.rotation);
                            car.transform.SetParent(gameObject.transform);
                            car.GetComponent<AICar>().Drive();
                            //car.GetComponent<NavMeshAgent>().SetDestination(sameSpawns.transform.GetChild(i).transform.GetChild(0).transform.position);
                            sameCarTable[val] = 1;
                            check = false;
                        }
                    }
                }
            }

            //Spawn intersections randomly
            //GameObject intersectionSpawns = gameObject.transform.Find("SpawnPoints").transform.Find("IntersectionSpawnPoints").gameObject;
            //spawnPoints = intersectionSpawns.transform.childCount;
            //for (int i = 0; i < spawnPoints; i++)
            //{
            //    int count = 1;
            //    for(int j = 0; j < intersectionTable.Length; j++)
            //    {
            //        count += intersectionTable[j];
            //    }
            //    if(!(count > 1))
            //    {
            //        bool check = true;
            //        while (check)
            //        {
            //            int val = Random.Range(0, intersectionTable.Length);
            //            if (intersectionTable[val] == 0)
            //            {
            //                GameObject road = (GameObject)Instantiate(intersectionPrefab, intersectionSpawns.transform.GetChild(val).transform.position, transform.rotation);
            //                road.transform.Rotate(Vector3.up, 90);
            //                road.transform.position = new Vector3(road.transform.position.x, 0, road.transform.position.z);
            //                intersectionTable[val] = 1;
            //                check = false;
            //            }
            //        }
            //    }
            //}
            boxCol.enabled = false;
        }
    }

    // Use this for initialization
    void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
