using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OldTileManager : MonoBehaviour 
{

    /// <summary>
    /// A list of the tiles, that we can spawn
    /// </summary>
    public GameObject[] tilePrefabs;

    /// <summary>
    /// The last tile that we spawned
    /// This is used as a reference for the next tile, that we are spawning
    /// </summary>
    public GameObject currentTile;

    /// <summary>
    /// A singleton instance for this Manager
    /// </summary>
    private static TileManager instance;

    /// <summary>
    /// A stack, that contains all the left tiles, this is used for recycling
    /// </summary>
    private Stack<GameObject> activeTiles = new Stack<GameObject>();

    /// <summary>
    /// Property for accessing the left tiles
    /// </summary>
    public Stack<GameObject> ActiveTiles
    {
        get { return activeTiles; }
        set { activeTiles = value; }
    }

    /// <summary>
    /// A property for accessing the singleton instance 
    /// </summary>
    public static TileManager Instance
    {
        get 
        {
            if (instance == null) //Finds the instance if it doesn't exist
            {
                instance = GameObject.FindObjectOfType<TileManager>();
            }

            return instance; 
        
        }

    }


	// Use this for initialization
	void Start () 
    {

        //Spawns 50 tiles when the game starts
        for (int i = 0; i < 10; i++)
        {
            SpawnTile();
        }
        
	}

    void Update() {

    }


    /// <summary>
    /// Spawns a tile in the gameworld
    /// </summary>
    public void SpawnTile()
    {
        //Generating a random number between 0 and 1
        int randomIndex = Random.Range(0, tilePrefabs.Length);

        currentTile = (GameObject)Instantiate(tilePrefabs[randomIndex], currentTile.transform.GetChild(0).position, Quaternion.identity);
    }
}
