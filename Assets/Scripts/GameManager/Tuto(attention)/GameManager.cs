using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	public static bool tutoFish;
	public static bool tutoHunt;
	public static bool tutoDeath;

	public static bool tutoShot;

	public GameObject sheep;
	public GameObject farmer;

	public GameObject HUDGO;
	public GameObject BlackBackground;

	public static bool endTuto;

	void Awake () 
	{
		PauseManager.onDialog = true;
		PauseManager.onTuto = true;
		//tutoHunt = true;
	}

	void FixedUpdate()
	{
		if (tutoHunt) 
		{
			if (sheep != null) 
			{
				sheep.SetActive (true);
			}
		}

		if (tutoDeath) 
		{
			if (farmer != null) 
			{
				farmer.SetActive (true);
			}
		}

		if (endTuto) 
		{
			HUDGO.SetActive (false);
			BlackBackground.SetActive (true);
		}
	}
}
