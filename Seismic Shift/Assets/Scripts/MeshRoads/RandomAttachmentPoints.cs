using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomAttachmentPoints : MonoBehaviour {

    public BezierSplineProcedural spline;
    public float spawnChance = 0.5f;
    public float randomOffsetRange = 7.5f;


    public GameObject[] obstacles;

    // Use this for initialization
    void Start() {
        StartCoroutine(WaitAndSpawn());
	}

    IEnumerator WaitAndSpawn()
    {
        yield return new WaitForSeconds(0.1f);
        for (int i = 0; i < spline.attachmentPoints.Count; i++)
        {
            AttachmentPoint attach = spline.attachmentPoints[i];
            if (Random.value < spawnChance) { 
                GameObject obstacle = (GameObject)Instantiate(pickObstacle(), attach.go.transform.position, attach.go.transform.rotation);
                //GameObject obstacle = (GameObject)Instantiate(pickObstacle(), new Vector3(0,0,0), Quaternion.identity);

                obstacle.transform.parent = attach.go.transform;
                obstacle.transform.position = obstacle.transform.position + (new Vector3(Random.Range(-randomOffsetRange, randomOffsetRange), 0, Random.Range(-randomOffsetRange, randomOffsetRange)));
            }
        }
    }

    private GameObject pickObstacle()
    {
        return obstacles[0];
    }
}
