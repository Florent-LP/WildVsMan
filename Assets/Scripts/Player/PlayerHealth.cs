using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour {

	public Slider healthBar;
	public float health;
	public GameObject blackBackground;
	public GameObject gameOverHUD;


	void FixedUpdate () 
	{
		//if(GetComponent<PlayerHunger>().hungerBar.value > 0 && GetComponent<PlayerThirst>().thirstBar.value > 0)
			healthBar.value = health;
		if (healthBar.value <= 0) 
		{
			blackBackground.SetActive (true);
			gameOverHUD.SetActive (true);
		}
	}

}
