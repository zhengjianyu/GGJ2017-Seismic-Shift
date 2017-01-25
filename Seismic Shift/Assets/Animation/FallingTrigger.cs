using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingTrigger : MonoBehaviour {

    public Animator FallingBuilding;
    public AudioClip FallingSound;
    public float volume;
    public AudioSource Audio;
    public bool alreadyplayed = false;
    private Component[] children;
    //private ParticleSystem ps;
    //private ParticleSystem.EmissionModule em;



    // Use this for initialization
    void Start () {
        Audio = GetComponent<AudioSource>();
        children = GetComponentsInChildren<ParticleSystem>();
    }

    void OnTriggerEnter(Collider x)
    {
        if (!alreadyplayed && x.tag == "Player")
        {
            Audio.PlayOneShot(FallingSound, volume);
            alreadyplayed = true;
            FallingBuilding.SetTrigger("fall");
        }
    }


    // Update is called once per frame
    void Update () {
		
	}
}
