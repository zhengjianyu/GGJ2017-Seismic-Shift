using UnityEngine;
using System.Collections;

public class ControlGUIScript : MonoBehaviour {

	public float Value = 0.5f;
	public float Fade = 0.01f;

	public GUIBarScript GBS;

	public Vector2 Offset;

	public Vector2 LabelOffSet;

	//public string playText = "Play";
	public bool IsPlaying = false;

    public bool inmenu = true;

	void Start()
	{
		GBS = GetComponent<GUIBarScript>();
	}

    public void NotPlaying()
    {
        IsPlaying = false;
    }

    public void Playing()
    {
        IsPlaying = true;
    }

    public void AddTime(float x)
    {
        GBS.Value += x;
        if (GBS.Value > 1)
        {
            GBS.Value = 1;
        }
    }

    public void LoseTime(float x)
    {
        GBS.Value -= x;
        if (GBS.Value < 0.0f)
        {
            IsPlaying = false;
            Dead();
        }
    }

	void OnGUI() 
	{
		if (GBS == null)
		{
			return;
		}

        //if (IsPlaying != true)
        //{
        //    GUIStyle LabelStyle = new GUIStyle();
        //    LabelStyle.normal.textColor = Color.black;
        //    LabelStyle.fontSize = 18;
        //    GUI.Label(new Rect(GBS.Position.x + Offset.x + LabelOffSet.x, GBS.Position.y + Offset.y + LabelOffSet.y - 40, 100, 25), "Value", LabelStyle);
        //    Value = GUI.HorizontalSlider(new Rect(GBS.Position.x + Offset.x, GBS.Position.y + Offset.y - 40, 180, 25), Value, 0.0F, 1F);
        //}

        //if (GUI.Button(new Rect(GBS.Position.x + Offset.x + LabelOffSet.x, GBS.Position.y + Offset.y + LabelOffSet.y - 80, 100, 25),playText ))
        //{
        //if (IsPlaying == true)
        //{
        //	IsPlaying = false;
        //	//playText = "Play";
        //}
        //else
        //{
        //	IsPlaying = true;
        //	//playText = "Stop";
        //         }
        //if (IsPlaying == false)
        //    IsPlaying = true;
        //}

    }


    void Dead()
    {
        if (inmenu == false)
        {
            Time.timeScale = 0;
            transform.parent.GetComponent<ShowPanels>().ShowLosePanel();
        }
    }

	void Update () 
	{
		if (GBS == null)
		{
			return;
		}

		if (IsPlaying == true)
		{
            if (transform.parent.GetComponent<Pause>().isPaused == false)
			    GBS.Value -= .07f * Time.deltaTime;
            //Debug.Log(GBS.Value);
            if (GBS.Value < 0.0f)
            {
                IsPlaying = false;
                Dead();
            }
        }
		else
		{
			GBS.Value = 1f;
		}

	}
}
