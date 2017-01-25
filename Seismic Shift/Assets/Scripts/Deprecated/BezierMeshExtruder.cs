using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BezierMeshExtruder : UniqueMesh
{
    public BezierSpline spline;

    private OrientedPoint[] flatPath;
    private OrientedPoint[] wavePath;

    private MeshCollider mc;

    public bool waveRunning;


    //======Wave Settings=========
    public float amplitude = 2f;
    public float period = 3f;
    public float speed = 5f;
    //============================

    // Use this for initialization
    void Start()
    {
        mc = GetComponent<MeshCollider>();
        flatPath = spline.GetPath();
        wavePath = new OrientedPoint[flatPath.Length];
        CopyPath(flatPath, wavePath);   //make a copy of the flat path that we can mess with.
        MakeMesh();
    }


    void CopyPath(OrientedPoint[] src, OrientedPoint[] dst)
    {
        for (int i = 0; i < src.Length; i++)
        {
            dst[i] = new OrientedPoint(src[i].position, src[i].rotation);        
        }
        
    }

    void Update()
    {
        //WavePath();
        MakeMesh();                      //If no changes to path at runtime, can just do this once in Start().
        mc.sharedMesh = mesh;
    }

    void MakeMesh()
    {
        if (waveRunning)
        {
            MeshPathExtruder.Extrude(mesh, GetRoadExtrudeShape(), wavePath);    //Extrude the wave path to build that. To keep the original road shape extrude flatpath instead.
        }
        else
        {
            //StartCoroutine(WavePathCoroutine());
            flatPath = spline.GetPath();
            MeshPathExtruder.Extrude(mesh, GetRoadExtrudeShape(), flatPath);
        }
    }

    private ExtrudeShape GetFlatExtrudeShape()
    {
        ExtrudeShape shape = new ExtrudeShape();
        shape.verts = new Vector2[]
        {
            new Vector2(0,0),
            new Vector2(1,0)
        };
        shape.normals = new Vector2[]
        {
            new Vector2(0,1),
            new Vector2(0,1)
        };
        shape.uCoords = new float[]
        {
            0,
            1
        };
        

        return shape;
    }

    private ExtrudeShape GetRoadExtrudeShape()
    {
        ExtrudeShape shape = new ExtrudeShape();
        shape.verts = new Vector2[]
        {
            new Vector2(-3,0.3f),
            new Vector2(-2,0.3f),
            new Vector2(-2,0),
            new Vector2(2,0),
            new Vector2(2,0.3f),
            new Vector2(3,0.3f)
        };
        shape.normals = new Vector2[]
        {
            new Vector2(0,1),
            new Vector2(0,1),
            new Vector2(1,0),
            new Vector2(0,1),
            new Vector2(-1,0),
            new Vector2(0,1),
        };

        //U COORDS NOT DONE.
        shape.uCoords = new float[]
        {
            0,
            1,
            0,
            1,
            0,
            1
        };

        return shape;
    }

    public void SendWave()
    {
        //StartCoroutine(SendWaveCoroutine());
        StartCoroutine(WavePathCoroutine());

    }

    IEnumerator WavePathCoroutine()
    {
        waveRunning = true;
        yield return new WaitForSeconds(1);

        float waveLength = period * 6;
        float radius = waveLength/2;

        float t = 0;

        float peakPos = wavePath.Length;

        int attempts = 0;
        while (((Mathf.Cos(((t * speed + peakPos) / period))) * amplitude < amplitude - 0.1) && attempts < 20)
        {
            peakPos += 0.5f;
            attempts++;
        }


        Debug.Log("Init t : " + t);
        Debug.Log("Top : " + (t * speed + 0));
        Debug.Log("Cos of: " + (t * speed + 0) / period);
        Debug.Log("Cos: " + Mathf.Cos((t * speed + 0) / period));

        while (peakPos > 0 - waveLength)
        {
            for (int i = 0; i < wavePath.Length; i++)
            {
                

                if (i > peakPos - radius && i < peakPos + radius)
                {
                    wavePath[i].position.y = flatPath[i].position.y + (Mathf.Cos(((t * speed + i) / period)) * amplitude) + amplitude;
                }
                else
                {
                    wavePath[i].position.y = flatPath[i].position.y;
                }
            }
            if (t == 0)
            {
                Debug.Log("First height: " + wavePath[wavePath.Length - 1].position.y);
                Debug.Log("Wavelength: " + waveLength);
                Debug.Log("Radius:" + radius);
                Debug.Log("Peakpos: " + peakPos);
                Debug.Log("Height at peakpos: " + (Mathf.Cos(((t * speed + peakPos) / period)) * amplitude));
            }
            peakPos -= Time.deltaTime * speed;
            t += Time.deltaTime;
            yield return 0;
        }
        waveRunning = false;
    }

    IEnumerator WavePathCoroutineGuessNums()
        //working best with
        //amp = 2
        //period = 3
        //speed = 5
    {
        yield return new WaitForSeconds(1);

        float waveLength = (speed / (1f / period)) + 4;
        float radius = waveLength/2;
        float peakPos = wavePath.Length + period/2;

        float initT = Time.time;
        float t = initT;


        while (true) { 
            for (int i = wavePath.Length - 1; i >= 0; i--)
            {
                if (i > peakPos - radius && i < peakPos + radius)
                {
                    wavePath[i].position.y = flatPath[i].position.y + (Mathf.Cos((t * speed + i) / period) * amplitude);
                }
                else
                {
                    wavePath[i].position.y = flatPath[i].position.y - amplitude;
                }
            }
            peakPos -= Time.deltaTime * speed;
            t +=Time.deltaTime;
            yield return 0;
        }
    }


    //Call this in Update() to have continuous wavy roads wee.
    private void WavePath()
    {
        float amplitude = 2f;
        float period = 3f;
        float speed = 5f;
        

        for (int i = 0; i < wavePath.Length; i++)
        {
            wavePath[i].position.y = flatPath[i].position.y + (Mathf.Sin((Time.time * speed + i)/period) * amplitude);

            //float slope = Mathf.Abs(Mathf.Cos(Time.time + (i / 5)) * 1f);
            //Vector3 flatRot = flatPath[i].rotation.eulerAngles;
            //Vector3 waveRot = new Vector3(flatRot.x + slope * 45, flatRot.y, flatRot.z);

            //wavePath[i].rotation = Quaternion.Euler(waveRot);
        }
    }

   
}

