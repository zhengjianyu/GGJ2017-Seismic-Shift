using UnityEngine;
using System.Collections;

public class TileScript : MonoBehaviour
{
    public BoxCollider boxCol;
    private GameObject UI;

    /// <summary>
    /// When an objects exit's the tile
    /// </summary>
    /// <param name="other"></param>
	void OnTriggerExit(Collider other)
    {
        //If the player exits the tile
        if (other.tag == "Player") {
            //Spawns a new tile
            //Debug.Log ("collision");


            TileManager.Instance.SpawnTile ();

            TileManager.Instance.UI.GetComponent<ShowPanels>().addtime(.5f);
            TileManager.Instance.UI.GetComponent<ShowPanels>().sc += 50;
            TileManager.Instance.UI.GetComponent<ShowPanels>().addScore(TileManager.Instance.UI.GetComponent<ShowPanels>().sc);

            boxCol.enabled = false;
        }
    }
}