using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerWavesStart : MonoBehaviour {

    public BezierMeshExtruderProcedural bme;

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            StartCoroutine(StartWaves());
        }
    }

    IEnumerator StartWaves()
    {
        if (!bme.waveRunning)
        {
            bme.SendWave();
        }
        yield return 0;
    }
}