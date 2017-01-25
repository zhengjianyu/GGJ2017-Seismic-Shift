using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BezierMeshExtruderProcedural : UniqueMesh
{
    public BezierSplineProcedural spline;

    private OrientedPoint[] flatPath;
    private OrientedPoint[] wavePath;

    private MeshCollider mc;

    public bool waveRunning;

    public int numAttachmentPoints;


    //======Wave Settings=========
    public float amplitude = 2f;
    public float period = 3f;
    public float speed = 5f;
    //============================

    //======Shape Settings========
    public float roadWidth = 3f;
    public float curbWidth = 1f;
    public float curbHeight = 0.3f;
    //============================

    // Use this for initialization
    void Start()
    {
        spline.attachmentPoints.Clear();
        mc = GetComponent<MeshCollider>();
        flatPath = spline.GetPath();
        wavePath = new OrientedPoint[flatPath.Length];
        CopyPath(flatPath, wavePath);   //make a copy of the flat path that we can mess with.
        MakeMesh();

        for (int i = 0; i < numAttachmentPoints; i++)
        {
            spline.AddAttachmentPoint();
        }
    }


    void CopyPath(OrientedPoint[] src, OrientedPoint[] dst)
    {
        Array.Resize(ref dst, src.Length);
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

        transform.rotation = Quaternion.Euler(Vector3.zero );   //this line is stupid. it fixes rotating the whole builder game object messing up the road.

        if (waveRunning)
        {
            MeshPathExtruder.Extrude(mesh, GetRoadExtrudeShape(), wavePath);    //Extrude the wave path to build that. To keep the original road shape extrude flatpath instead.
        }
        else
        {
            //StartCoroutine(WavePathCoroutine());
            flatPath = spline.GetPath();
            PlaceAttachmentPoints(flatPath);
            MeshPathExtruder.Extrude(mesh, GetRoadExtrudeShape(), flatPath);
        }
    }

    private ExtrudeShape GetFlatExtrudeShape()
    {
        ExtrudeShape shape = new ExtrudeShape();
        shape.verts = new Vector2[]
        {
            new Vector2(-roadWidth,0),
            new Vector2(roadWidth,0)
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
            new Vector2(-(roadWidth)/2 -curbWidth,curbHeight),
            new Vector2(-roadWidth/2,curbHeight),
            new Vector2(-roadWidth/2,0),
            new Vector2(roadWidth/2,0),
            new Vector2(roadWidth/2,curbHeight),
            new Vector2(roadWidth/2 + curbWidth,curbHeight)
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
            1,
            0,
            1,
            1,
            0,
            1
        };

        return shape;
    }

    public void SendWave()
    {
        //StartCoroutine(SendWaveCoroutine());
        if (!waveRunning)
        {
            CopyPath(flatPath, wavePath);
            StartCoroutine(WavePathCoroutine());
        }

    }

    public void PlaceAttachmentPoints(OrientedPoint[] path)
    {
        for (int i = 0; i < spline.attachmentPoints.Count; i++)
        {
            AttachmentPoint attach = spline.attachmentPoints[i];
            attach.go.transform.position = path[(int)(((float)i / spline.attachmentPoints.Count) * path.Length)].position + transform.parent.position;
            attach.go.transform.position = Quaternion.AngleAxis(0, Vector3.up) * attach.go.transform.position;
            attach.go.transform.rotation = Quaternion.Euler(path[(int)(((float)i / spline.attachmentPoints.Count) * path.Length)].rotation.eulerAngles + transform.parent.rotation.eulerAngles);
        }
    }

    IEnumerator WavePathCoroutine()
    {
        waveRunning = true;
        yield return new WaitForSeconds(1);

        float waveLength = period * 6;
        float radius = waveLength/2;

        float t = 0;

        float peakPos = wavePath.Length + radius;

        int attempts = 0;
        while (((Mathf.Cos(((t * speed + peakPos) / period))) * amplitude < amplitude - 0.05) && attempts < 100)
        {
            peakPos += 0.5f;
            attempts++;
        }

        while (peakPos > 0 - waveLength)
        {
            for (int i = 0; i < wavePath.Length; i++)
            {
                
                if (i > peakPos - radius && i < peakPos + radius)
                {
                    wavePath[i].position.y = flatPath[i].position.y + (Mathf.Cos(((t * speed + i) / period)) * amplitude) + amplitude;
                    PlaceAttachmentPoints(wavePath);
                }
                else
                {
                    wavePath[i].position.y = flatPath[i].position.y;
                }
            }
            peakPos -= Time.deltaTime * speed;
            t += Time.deltaTime;
            yield return 0;
        }
        waveRunning = false;
    }

    //Call this in Update() to have continuous wavy roads weee.
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

