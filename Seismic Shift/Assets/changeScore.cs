using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class changeScore : MonoBehaviour {
    private string score = "Score: 0";
    private int IntScore = 0;
    private Text scoretext;
    public bool inmenu = true;

    // Use this for initialization
    void Start () {
        scoretext = GetComponent<UnityEngine.UI.Text>();
    }
	
	// Update is called once per frame
	void Update () {
        if (inmenu == true)
        {
            score = "Score: 0";
        }
        scoretext.text = score;
    }

    public void addscore(int x)
    {
        IntScore += x;
        score = "Score: " + IntScore.ToString();
        //Debug.Log(score);
    }

    public void setzero()
    {
        score = "Score: 0";
    }
}
