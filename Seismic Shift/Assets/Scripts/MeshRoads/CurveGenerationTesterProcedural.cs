using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurveGenerationTesterProcedural : MonoBehaviour {

    public BezierSplineProcedural spline;
    BezierMeshExtruderProcedural bme;

    public float waitTime = 10f;

	// Use this for initialization
	void Start () {
        bme = GetComponent<BezierMeshExtruderProcedural>();
        //AddStartingCurves(5);
        StartCoroutine(GenerateCurves());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void AddStartingCurves(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            spline.AddCurve();
        }
    }

    IEnumerator GenerateCurves()
    {
        yield return new WaitForSeconds(waitTime);


        int i = 0;
        while (true)
        {
            //spline.AddCurve();
            //spline.RemoveOldestCurve();
            if (i % 4 == 0)
            {
                bme.SendWave();
            }
            yield return new WaitForSeconds(waitTime);
            i++;
        }
    }
}
