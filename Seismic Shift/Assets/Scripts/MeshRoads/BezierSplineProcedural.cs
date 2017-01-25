using UnityEngine;
using System;
using System.Collections.Generic;

public class BezierSplineProcedural : MonoBehaviour {

	[SerializeField]
    private List<Vector3> points = new List<Vector3>();
    
    //Not implemented
    [SerializeField]
    private List<GameObject> EndPoints = new List<GameObject>();

    [SerializeField]
	private BezierControlPointMode[] modes;

    //For connecting hazards to the road
    public List<AttachmentPoint> attachmentPoints = new List<AttachmentPoint>();
    

	[SerializeField]
	private bool loop;

	public bool Loop {
		get {
			return loop;
		}
		set {
			loop = value;
			if (value == true) {
				modes[modes.Length - 1] = modes[0];
				SetControlPoint(0, points[0]);
			}
		}
	}

	public int ControlPointCount {
		get {
			return points.Count;
		}
	}

	public Vector3 GetControlPoint (int index) {
        return points[index];
	}

	public void SetControlPoint (int index, Vector3 point) {
		if (index % 3 == 0) {
			Vector3 delta = point - points[index];
			if (loop) {
				if (index == 0) {
					points[1] += delta;
					points[points.Count - 2] += delta;
					points[points.Count - 1] = point;
				}
				else if (index == points.Count - 1) {
					points[0] = point;
					points[1] += delta;
					points[index - 1] += delta;
				}
				else {
					points[index - 1] += delta;
					points[index + 1] += delta;
				}
			}
			else {
				if (index > 0) {
					points[index - 1] += delta;
				}
				if (index + 1 < points.Count) {
					points[index + 1] += delta;
				}
			}
		}
		points[index] = point;
		EnforceMode(index);
	}

	public BezierControlPointMode GetControlPointMode (int index) {
		return modes[(index + 1) / 3];
	}

	public void SetControlPointMode (int index, BezierControlPointMode mode) {
		int modeIndex = (index + 1) / 3;
		modes[modeIndex] = mode;
		if (loop) {
			if (modeIndex == 0) {
				modes[modes.Length - 1] = mode;
			}
			else if (modeIndex == modes.Length - 1) {
				modes[0] = mode;
			}
		}
		EnforceMode(index);
	}

	private void EnforceMode (int index) {
		int modeIndex = (index + 1) / 3;
		BezierControlPointMode mode = modes[modeIndex];
		if (mode == BezierControlPointMode.Free || !loop && (modeIndex == 0 || modeIndex == modes.Length - 1)) {
			return;
		}

		int middleIndex = modeIndex * 3;
		int fixedIndex, enforcedIndex;
		if (index <= middleIndex) {
			fixedIndex = middleIndex - 1;
			if (fixedIndex < 0) {
				fixedIndex = points.Count - 2;
			}
			enforcedIndex = middleIndex + 1;
			if (enforcedIndex >= points.Count) {
				enforcedIndex = 1;
			}
		}
		else {
			fixedIndex = middleIndex + 1;
			if (fixedIndex >= points.Count) {
				fixedIndex = 1;
			}
			enforcedIndex = middleIndex - 1;
			if (enforcedIndex < 0) {
				enforcedIndex = points.Count - 2;
			}
		}

		Vector3 middle = points[middleIndex];
		Vector3 enforcedTangent = middle - points[fixedIndex];
		if (mode == BezierControlPointMode.Aligned) {
			enforcedTangent = enforcedTangent.normalized * Vector3.Distance(middle, points[enforcedIndex]);
		}
		points[enforcedIndex] = middle + enforcedTangent;
	}

	public int CurveCount {
		get {
			return (points.Count - 1) / 3;
		}
	}

	public Vector3 GetPoint (float t) {
		int i;
		if (t >= 1f) {
			t = 1f;
			i = points.Count - 4;
		}
		else {
			t = Mathf.Clamp01(t) * CurveCount;
			i = (int)t;
			t -= i;
			i *= 3;
		}
		return transform.TransformPoint(Bezier.GetPoint(points[i], points[i + 1], points[i + 2], points[i + 3], t));
	}
	
	public Vector3 GetVelocity (float t) {
		int i;
		if (t >= 1f) {
			t = 1f;
			i = points.Count - 4;
		}
		else {
			t = Mathf.Clamp01(t) * CurveCount;
			i = (int)t;
			t -= i;
			i *= 3;
		}
		return transform.TransformPoint(Bezier.GetFirstDerivative(points[i], points[i + 1], points[i + 2], points[i + 3], t)) - transform.position;
	}
	
	public Vector3 GetDirection (float t) {
		return GetVelocity(t).normalized;
	}

	public void AddCurve () {
		Vector3 point = points[points.Count - 1];
        
        point.x += 1f;
        points.Insert(points.Count, point);
        point.x += 1f;
        points.Insert(points.Count, point);
        point.x += 1f;
        points.Insert(points.Count, point);    

        Array.Resize(ref modes, modes.Length + 1);
		modes[modes.Length - 1] = modes[modes.Length - 2];
		EnforceMode(points.Count - 4);

		if (loop) {
			points[points.Count - 1] = points[0];
			modes[modes.Length - 1] = modes[0];
			EnforceMode(0);
		}
	}

    public void RemoveOldestCurve()
    {
        points.RemoveAt(0);
        points.RemoveAt(0);
        points.RemoveAt(0);
    }
	
	public void Reset () {
        points.Clear();
        points.Add(new Vector3(1f, 0f, 0f));
        points.Add(new Vector3(2f, 0f, 0f));
        points.Add(new Vector3(3f, 0f, 0f));
        points.Add(new Vector3(4f, 0f, 0f));

		modes = new BezierControlPointMode[] {
			BezierControlPointMode.Free,
			BezierControlPointMode.Free
		};
	}

    public void AddAttachmentPoint()
    {
        GameObject go = new GameObject();
        //go.transform.position = position;
        //go.transform.rotation = Quaternion.LookRotation(rotation);
        go.transform.SetParent(transform.parent);
        go.name = "Attach" + attachmentPoints.Count;
        AttachmentPoint attach = new AttachmentPoint();
        attach.go = go;
        attachmentPoints.Add(attach);
        //Debug.Log("Added. Count: " + attachmentPoints.Count);
    }

    public OrientedPoint[] GetPath()
    {
        //higher frequency makes better quality mesh
        int frequency = 100;
        int itemsLength = 1;

        OrientedPoint[] path = new OrientedPoint[frequency];

        float stepSize = frequency * itemsLength;

        stepSize = 1f / (stepSize - 1);

        for (int p = 0; p < path.Length; p++)
        {
            Vector3 position = GetPoint(p * stepSize);
            

           
            //position = Quaternion.AngleAxis(0, Vector3.up) * position;

            Quaternion rotation = Quaternion.LookRotation(GetDirection(p * stepSize));

            //rotation = Quaternion.Euler(rotation.eulerAngles - gameObject.GetComponentInParent<Transform>().rotation.eulerAngles);
            //Weird offset line that needs to be here if both path and roadModel are not at origin.
            position = new Vector3(position.x - gameObject.GetComponentInParent<Transform>().position.x, position.y - gameObject.GetComponentInParent<Transform>().position.y, position.z - gameObject.GetComponentInParent<Transform>().position.z);

            OrientedPoint point = new OrientedPoint(position, rotation);
            path[p] = point;
        }
        return path;
    }


}

[Serializable]
public struct AttachmentPoint
{
    //t isn't used. Too lazy to just make this an array of gameobjects tho.
    public float t;
    public GameObject go;
}