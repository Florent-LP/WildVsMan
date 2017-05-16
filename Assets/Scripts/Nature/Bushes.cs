using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bushes : MonoBehaviour {

	private AudioSource audioSource;
	public float bushVolume = 0.4f;
	private GameObject windZoneObj;
	private GameObject detectionArea;
	private float timer;
	public float timerValue = 5.0f;
	private float turbulenceValue;
	private bool isPlayerIn;
	private bool isPlayerOut;

	void Start()
	{
		audioSource = GetComponent<AudioSource> ();
		windZoneObj = transform.FindChild ("Wind").gameObject;
		detectionArea = transform.FindChild ("DetectionArea").gameObject;
		turbulenceValue = windZoneObj.GetComponent<WindZone> ().windTurbulence;
	}

	void Update()
	{
		if (isPlayerIn || isPlayerOut) 
		{
			timer -= Time.deltaTime;
			windZoneObj.GetComponent<WindZone> ().windTurbulence -= 2.6f*Time.deltaTime;
		}
		if (timer < 0) 
		{
			windZoneObj.GetComponent<WindZone> ().windTurbulence = turbulenceValue;
			windZoneObj.SetActive (false);
			detectionArea.SetActive (false);
		}
	}

	void ShakeBush()
	{
		timer = timerValue;
		windZoneObj.SetActive (true);
		detectionArea.SetActive (true);
	}

	void OnTriggerEnter(Collider other)
	{
		if(other.transform.gameObject.tag == "Player")
		{
			//Debug.Log ("Player touched me!!!!!");
			ShakeBush ();
			isPlayerOut = false;
			isPlayerIn = true;
			audioSource.PlayOneShot (audioSource.clip, bushVolume);
		}
	}

	void OnTriggerExit(Collider other)
    {
        if (other.transform.gameObject.tag == "Player")
        {
            //Debug.Log ("Player get out of here!!!!!");
            ShakeBush();
            isPlayerIn = false;
            isPlayerOut = true;
            audioSource.PlayOneShot(audioSource.clip, bushVolume);
        }
	}
}
