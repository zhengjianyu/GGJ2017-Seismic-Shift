using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveStarterProcedural : MonoBehaviour {

    BezierMeshExtruderProcedural bme;
	// Use this for initialization
	void Start () {
        bme = GetComponent<BezierMeshExtruderProcedural>();

        Debug.Log("Starting wave from wave starter...");
        bme.SendWave();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
