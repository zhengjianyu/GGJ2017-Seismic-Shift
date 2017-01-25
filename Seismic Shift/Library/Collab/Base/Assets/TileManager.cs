using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour {

    public GameObject[] tilePrefabs;
    public GameObject currentTile;

	// Use this for initialization
	void Start () {
        SpawnTile();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SpawnTile()
    {
       currentTile = (GameObject)Instantiate(tilePrefabs[0], currentTile.transform.GetChild(0).transform.GetChild(0).position, Quaternion.identity);
    }
}
