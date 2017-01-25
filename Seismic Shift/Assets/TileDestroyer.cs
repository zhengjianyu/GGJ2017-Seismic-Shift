using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileDestroyer : MonoBehaviour {

	void OnTriggerEnter(Collider other)
	{
		//If the player exits the tile
		if (other.tag == "Player")
		{
			//Spawns a new tile
			TileManager.Instance.DeleteTile();
		}
	}
}
