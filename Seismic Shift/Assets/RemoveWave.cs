using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveWave : MonoBehaviour {

    /// <summary>
    /// When an objects exit's the tile
    /// </summary>
    /// <param name="other"></param>
	void OnTriggerExit(Collider other)
    {
        //If the player exits the tile
        if (other.tag == "Player")
        {
            Destroy(gameObject);
        }
    }
}
