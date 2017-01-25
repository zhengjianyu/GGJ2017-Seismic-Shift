using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TileManager : MonoBehaviour
{

    /// <summary>
    /// A list of the tiles, that we can spawn
    /// </summary>
    public GameObject[] tilePrefabs;

    /// <summary>
    /// Wave simulator
    /// </summary>
    public GameObject wavePrefab;

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
    private List<GameObject> activeTiles;

    /// <summary>
    /// Property for accessing the left tiles
    /// </summary>

    public GameObject UI;

    public int straightPathCount = 1;
    public int rotationAngle = 0;
    public int rightTurnCount = 0;
    public int leftTurnCount = 0;

    public char direction = 'F';

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
    void Start()
    {
        //here to add time / lose time
        UI = GameObject.Find("UI");
        //UI.GetComponent<ShowPanels>().losetime(.5f);
        //UI.GetComponent<ShowPanels>().addScore(10);

        activeTiles = new List<GameObject>();
        activeTiles.Add(currentTile);

        //Spawns 3 tiles when the game starts
        for (int i = 0; i < 3; i++)
        {
            SpawnTile();
        }
    }


    /// <summary>
    /// Spawns a tile in the gameworld
    /// </summary>
    public void SpawnTile()
    {
        //Generating a random number between 0 and the number of prefabs
        int randomIndex = Random.Range(0, tilePrefabs.Length);

        //Possibly Intersection
        currentTile = (GameObject)Instantiate(tilePrefabs[randomIndex], currentTile.transform.GetChild(0).position, Quaternion.Euler(new Vector3(0, rotationAngle, 0)));
        if (activeTiles.Count > 10)
            DeleteTile();
        activeTiles.Add(currentTile);


        //If intersection
        if (randomIndex == (tilePrefabs.Length - 1)) //Intersection - If the random number is one then spawn a left tile
        {
            straightPathCount = 0;
            int randomDirection = Random.Range(0, 3);

            if (rightTurnCount >= 2)
            {
                while (randomDirection == 2)
                {
                    randomDirection = Random.Range(0, 3);
                }
            }
            if (leftTurnCount >= 2)
            {
                while (randomDirection == 1)
                {
                    randomDirection = Random.Range(0, 3);
                }
            }


            randomIndex = Random.Range(0, tilePrefabs.Length - 1);
            //currenty this will only spawn intersection after an intersection
            //straight
            if (randomDirection == 0)
            {
                currentTile.transform.Find("FallingBuildingLeft").gameObject.SetActive(true);
                currentTile.transform.Find("FallingBuildingRight").gameObject.SetActive(true);

                //Debug.Log("straight");
                currentTile = (GameObject)Instantiate(tilePrefabs[randomIndex], currentTile.transform.GetChild(randomDirection).position, Quaternion.Euler(new Vector3(0, rotationAngle, 0)));
                straightPathCount += 1;
            }
            //left
            else if (randomDirection == 1)
            {
                currentTile.transform.Find("FallingBuildingStraight").gameObject.SetActive(true);
                currentTile.transform.Find("FallingBuildingRight").gameObject.SetActive(true);

                //Debug.Log("left");
                rotationAngle += -90;
                currentTile = (GameObject)Instantiate(tilePrefabs[randomIndex], currentTile.transform.GetChild(randomDirection).position, Quaternion.Euler(new Vector3(0, rotationAngle, 0)));
                leftTurnCount += 1;
                if (!(rightTurnCount <= 0))
                {
                    rightTurnCount -= 1;
                }
                if (direction == 'F')
                    direction = 'L';
                else if (direction == 'R')
                    direction = 'F';
                else if (direction == 'B')
                    direction = 'R';
                else if (direction == 'L')
                    direction = 'B';
            }
            //right
            else if (randomDirection == 2)
            {
                currentTile.transform.Find("FallingBuildingStraight").gameObject.SetActive(true);
                currentTile.transform.Find("FallingBuildingLeft").gameObject.SetActive(true);

                //Debug.Log("right");
                rotationAngle += 90;
                currentTile = (GameObject)Instantiate(tilePrefabs[randomIndex], currentTile.transform.GetChild(randomDirection).position, Quaternion.Euler(new Vector3(0, rotationAngle, 0)));
                rightTurnCount += 1;
                if (!(leftTurnCount <= 0))
                {
                    leftTurnCount -= 1;
                }
                if (direction == 'F')
                    direction = 'R';
                else if (direction == 'R')
                    direction = 'B';
                else if (direction == 'B')
                    direction = 'L';
                else if (direction == 'L')
                    direction = 'F';
            }

            //Debug.Log(activeTiles.Count);
            if (activeTiles.Count > 10)
                DeleteTile();
            activeTiles.Add(currentTile);
        }
        else
        {
            straightPathCount += 1;
        }
           
        if (straightPathCount >= 3)
        {
            Debug.Log(direction);
            Vector3 offset = new Vector3(0, 0, 0);

            if (direction == 'F')
                offset = new Vector3(0, 0, -360);
            else if (direction == 'R')
                offset = new Vector3(-360, 0, 0);
            else if (direction == 'B')
                offset = new Vector3(0, 0, 360);
            else if (direction == 'L')
                offset = new Vector3(360, 0, 0);

            Debug.Log("Wave");
            Instantiate(wavePrefab, currentTile.transform.GetChild(1).position + offset, Quaternion.Euler(new Vector3(0, rotationAngle-90, 0)));
            straightPathCount = 0;
        }
    }

    public void DeleteTile()
    {
        Destroy(activeTiles[0]);
        activeTiles.RemoveAt(0);
    }
}