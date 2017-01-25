using UnityEngine;
using System.Collections;

public class Done_BGScroller : MonoBehaviour
{
    public string XYZ;

	public float scrollSpeed;
	public float tileSizeZ;

	private Vector3 startPosition;

	void Start ()
	{
		startPosition = transform.position;
	}

	void Update ()
	{
        if (XYZ == "X")
        {
            float newPosition = Mathf.Repeat(Time.time * scrollSpeed, tileSizeZ);
            transform.position = startPosition + Vector3.right * newPosition;
        }
        else if (XYZ == "Z")
        {
            float newPosition = Mathf.Repeat(Time.time * scrollSpeed, tileSizeZ);
            transform.position = startPosition + Vector3.forward * newPosition;
        }

	}
}